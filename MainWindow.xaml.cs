using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace FolderLens;

public partial class MainWindow : Window
{
    private const int VkLeftAlt = 0xA4;
    private const int VkRightAlt = 0xA5;
    private const uint VkSpace = 0x20;
    private const int WmKeyDown = 0x0100;
    private const int WmKeyUp = 0x0101;
    private const int WmSysKeyDown = 0x0104;
    private const int WmSysKeyUp = 0x0105;

    private readonly ObservableCollection<FolderEntry> _visibleFolders = [];
    private readonly IndexCacheStore _cache = new();
    private readonly NativeMethods.LowLevelKeyboardProc _keyboardProc;
    private readonly NativeMethods.LowLevelMouseProc _mouseProc;
    private IReadOnlyList<FolderEntry> _allFolders = [];
    private Forms.NotifyIcon? _trayIcon;
    private System.Drawing.Icon? _applicationIcon;
    private CancellationTokenSource? _indexCts;
    private CancellationTokenSource? _previewCts;
    private IntPtr _keyboardHook;
    private IntPtr _mouseHook;
    private PreviewState? _previewState;
    private bool _allowExit;
    private bool _loaded;
    private bool _suppressDeactivate;
    private bool _opening;
    private bool _spaceHandled;

    public MainWindow()
    {
        InitializeComponent();
        ResultsListBox.ItemsSource = _visibleFolders;
        _keyboardProc = KeyboardHookCallback;
        _mouseProc = MouseHookCallback;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_loaded) return;
        _loaded = true;
        ConfigureTrayIcon();
        InstallKeyboardHook();
        InstallMouseHook();
        Hide();
        var cachedFolders = _cache.TryLoad(App.Settings.Current.SearchRoots);
        if (cachedFolders.Count > 0)
        {
            _allFolders = cachedFolders;
            ApplyFilter();
        }
        RefreshIndexAsync();
    }

    private void ConfigureTrayIcon()
    {
        try
        {
            var executable = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(executable))
                _applicationIcon = System.Drawing.Icon.ExtractAssociatedIcon(executable);
        }
        catch { }

        _trayIcon = new Forms.NotifyIcon
        {
            Icon = _applicationIcon ?? System.Drawing.SystemIcons.Application,
            Text = "FolderLens · buscador de carpetas",
            Visible = true
        };
        _trayIcon.MouseClick += TrayIcon_MouseClick;
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Abrir buscador", null, (_, _) => ShowSearchWindow());
        menu.Items.Add("Actualizar índice", null, (_, _) => RefreshIndexAsync());
        menu.Items.Add("Configuración", null, (_, _) => OpenSettings());
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("Salir", null, (_, _) => ExitApplication());
        _trayIcon.ContextMenuStrip = menu;
    }

    private void TrayIcon_MouseClick(object? sender, Forms.MouseEventArgs e)
    {
        if (e.Button == Forms.MouseButtons.Left) ShowSearchWindow();
    }

    private void ShowSearchWindow()
    {
        _opening = true;
        Show();
        WindowState = WindowState.Normal;
        SearchBox.Clear();
        ApplyFilter();
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Left + (workArea.Width - ActualWidth) / 2;
            Top = workArea.Top + Math.Min(110, workArea.Height * 0.16);
            FocusSearchBoxWhenReady();
        }));
    }

    private void FocusSearchBoxWhenReady()
    {
        var attempts = 0;
        var timer = new DispatcherTimer(DispatcherPriority.Input)
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };
        timer.Tick += (_, _) =>
        {
            attempts++;
            if (IsAltDown() && attempts < 32) return;

            timer.Stop();
            ForceWindowToForeground();
            Activate();
            SearchBox.Focusable = true;
            SearchBox.Focus();
            Keyboard.Focus(SearchBox);
            SearchBox.CaretIndex = SearchBox.Text.Length;
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                if (!IsVisible) return;
                _opening = false;
                FocusSearchBox();
            }));
        };
        timer.Start();
    }

    private void Window_Activated(object? sender, EventArgs e)
    {
        if (!_opening) return;
        Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(FocusSearchBox));
    }

    private void FocusSearchBox()
    {
        if (!IsVisible) return;
        ForceWindowToForeground();
        SearchBox.Focusable = true;
        SearchBox.IsTabStop = true;
        SearchBox.Focus();
        Keyboard.Focus(SearchBox);
        SearchBox.CaretIndex = SearchBox.Text.Length;
    }

    private void ForceWindowToForeground()
    {
        var handle = new WindowInteropHelper(this).Handle;
        if (handle == IntPtr.Zero) return;

        var foreground = NativeMethods.GetForegroundWindow();
        var foregroundThread = foreground == IntPtr.Zero ? 0u : NativeMethods.GetWindowThreadProcessId(foreground, IntPtr.Zero);
        var currentThread = NativeMethods.GetCurrentThreadId();
        var attached = foregroundThread != 0 && foregroundThread != currentThread
            && NativeMethods.AttachThreadInput(currentThread, foregroundThread, true);
        try
        {
            NativeMethods.BringWindowToTop(handle);
            NativeMethods.SetForegroundWindow(handle);
            NativeMethods.SetFocus(handle);
        }
        finally
        {
            if (attached) NativeMethods.AttachThreadInput(currentThread, foregroundThread, false);
        }
    }

    private async void RefreshIndexAsync()
    {
        _indexCts?.Cancel();
        _indexCts = new CancellationTokenSource();
        var token = _indexCts.Token;

        try
        {
            _allFolders = await App.Index.BuildAsync(App.Settings.Current.SearchRoots, token);
            ApplyFilter();
            _ = Task.Run(() => _cache.Save(App.Settings.Current.SearchRoots, _allFolders));
        }
        catch (OperationCanceledException) { }
    }

    private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        SearchHint.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchBox?.Text?.Trim() ?? string.Empty;
        var matches = string.IsNullOrWhiteSpace(query)
            ? _allFolders
            : _allFolders.Where(folder => folder.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                                       || folder.LocationLabel.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToArray();

        var limited = matches.Take(250).ToArray();
        _visibleFolders.Clear();
        foreach (var folder in limited) _visibleFolders.Add(folder);
        ResultCountText.Text = matches.Count == 1 ? "1 carpeta" : $"{matches.Count:N0} carpetas";

        ResultsArea.Visibility = string.IsNullOrWhiteSpace(query) ? Visibility.Collapsed : Visibility.Visible;

        var hasResults = _visibleFolders.Count > 0;
        EmptyState.Visibility = hasResults ? Visibility.Collapsed : Visibility.Visible;
        ResultsListBox.Visibility = hasResults ? Visibility.Visible : Visibility.Collapsed;
        if (_allFolders.Count == 0)
        {
            EmptyTitle.Text = "Elegí tus carpetas de búsqueda";
            EmptyMessage.Text = "Agregá una carpeta desde Configuración. FolderLens buscará dentro de ella sin tocar el resto de tu PC.";
        }
        else if (!hasResults)
        {
            EmptyTitle.Text = "No encontramos esa carpeta";
            EmptyMessage.Text = "Probá con otra parte del nombre o de su ubicación.";
        }
        else
        {
            EmptyTitle.Text = "Escribí para buscar";
            EmptyMessage.Text = "Los resultados aparecen acá debajo.";
        }
    }

    private async void Folder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not FolderEntry folder) return;
        _previewCts?.Cancel();
        _previewCts = new CancellationTokenSource();
        var token = _previewCts.Token;
        SetPreviewState(new PreviewState { Folder = folder, Message = "Buscando fotos…" });

        try
        {
            var state = await App.Index.LoadPreviewAsync(folder, token);
            if (!token.IsCancellationRequested)
                SetPreviewState(state);
        }
        catch (OperationCanceledException) { }
    }

    private void SetPreviewState(PreviewState state)
    {
        _previewState = state;
        PreviewTitle.Text = state.Folder?.Name ?? "Elegí una carpeta";
        PreviewPath.Text = state.Folder?.LocationLabel ?? "Las fotos aparecen acá";
        PreviewMessage.Text = state.Message ?? string.Empty;
        PreviewMessage.Visibility = state.Images.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        PreviewThumbs.ItemsSource = state.Images;
        OpenPreviewButton.IsEnabled = state.Folder is not null;
    }

    private void Folder_Click(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) return;
        if (sender is FrameworkElement element && element.DataContext is FolderEntry folder)
        {
            OpenFolder(folder.Path);
            HideSearchWindow();
            e.Handled = true;
        }
    }

    private void OpenPreview_Click(object sender, RoutedEventArgs e)
    {
        if (_previewState?.Folder is not null) OpenFolder(_previewState.Folder.Path);
    }

    private static void OpenFolder(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = true });
        }
        catch { }
    }

    private void Settings_Click(object sender, RoutedEventArgs e) => OpenSettings();

    private void OpenSettings()
    {
        _suppressDeactivate = true;
        try
        {
            var dialog = new SettingsWindow(this);
            if (dialog.ShowDialog() != true || dialog.Result is null) return;
            App.Settings.Save(dialog.Result);
            App.Index.ClearPreviewCache();
            RefreshIndexAsync();
        }
        finally
        {
            _suppressDeactivate = false;
        }
    }

    private void CloseSearch_Click(object sender, RoutedEventArgs e) => HideSearchWindow();

    private void HideSearchWindow()
    {
        _opening = false;
        Hide();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            HideSearchWindow();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter && ResultsListBox.SelectedItem is FolderEntry folder)
        {
            OpenFolder(folder.Path);
            e.Handled = true;
        }
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (!_allowExit)
        {
            e.Cancel = true;
            HideSearchWindow();
            return;
        }

        UninstallKeyboardHook();
        UninstallMouseHook();
        _trayIcon?.Dispose();
        _applicationIcon?.Dispose();
    }

    private void ExitApplication()
    {
        _allowExit = true;
        Close();
    }

    private void InstallKeyboardHook()
    {
        if (_keyboardHook != IntPtr.Zero) return;
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        _keyboardHook = NativeMethods.SetWindowsHookEx(
            NativeMethods.WhKeyboardLl,
            _keyboardProc,
            NativeMethods.GetModuleHandle(module?.ModuleName),
            0);
    }

    private void InstallMouseHook()
    {
        if (_mouseHook != IntPtr.Zero) return;
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        _mouseHook = NativeMethods.SetWindowsMouseHookEx(
            NativeMethods.WhMouseLl,
            _mouseProc,
            NativeMethods.GetModuleHandle(module?.ModuleName),
            0);
    }

    private void UninstallKeyboardHook()
    {
        if (_keyboardHook == IntPtr.Zero) return;
        NativeMethods.UnhookWindowsHookEx(_keyboardHook);
        _keyboardHook = IntPtr.Zero;
    }

    private void UninstallMouseHook()
    {
        if (_mouseHook == IntPtr.Zero) return;
        NativeMethods.UnhookWindowsHookEx(_mouseHook);
        _mouseHook = IntPtr.Zero;
    }

    private IntPtr KeyboardHookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0)
        {
            var virtualKey = Marshal.ReadInt32(lParam);
            var message = wParam.ToInt32();
            var isSpaceDown = virtualKey == VkSpace && (message == WmKeyDown || message == WmSysKeyDown);
            var isSpaceUp = virtualKey == VkSpace && (message == WmKeyUp || message == WmSysKeyUp);

            if (isSpaceDown && !_spaceHandled && IsAltDown())
            {
                _spaceHandled = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(ShowSearchWindow));
                return new IntPtr(1);
            }

            if (isSpaceUp && _spaceHandled)
            {
                _spaceHandled = false;
                return new IntPtr(1);
            }
        }

        return NativeMethods.CallNextHookEx(_keyboardHook, code, wParam, lParam);
    }

    private IntPtr MouseHookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0 && IsOutsideClickMessage(wParam.ToInt32()) && IsVisible && !_suppressDeactivate && !_opening)
        {
            var point = Marshal.PtrToStructure<NativeMethods.MousePoint>(lParam);
            var handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero && NativeMethods.GetWindowRect(handle, out var bounds) && !bounds.Contains(point.X, point.Y))
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(HideSearchWindow));
        }

        return NativeMethods.CallNextHookEx(_mouseHook, code, wParam, lParam);
    }

    private static bool IsOutsideClickMessage(int message) =>
        message == NativeMethods.WmLeftButtonDown
        || message == NativeMethods.WmRightButtonDown
        || message == NativeMethods.WmMiddleButtonDown;

    private static bool IsAltDown() =>
        (NativeMethods.GetAsyncKeyState(VkLeftAlt) & 0x8000) != 0
        || (NativeMethods.GetAsyncKeyState(VkRightAlt) & 0x8000) != 0;

    private static class NativeMethods
    {
        public const int WhKeyboardLl = 13;
        public const int WhMouseLl = 14;
        public const int WmLeftButtonDown = 0x0201;
        public const int WmRightButtonDown = 0x0204;
        public const int WmMiddleButtonDown = 0x0207;

        public delegate IntPtr LowLevelKeyboardProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr LowLevelMouseProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr moduleHandle, uint threadId);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowsHookExW")]
        public static extern IntPtr SetWindowsMouseHookEx(int idHook, LowLevelMouseProc callback, IntPtr moduleHandle, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int virtualKey);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr handle, out WindowRect rect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr handle, IntPtr processId);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachThreadInput(uint sourceThreadId, uint targetThreadId, bool attach);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string? moduleName);

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public readonly bool Contains(int x, int y) => x >= Left && x < Right && y >= Top && y < Bottom;
        }
    }
}
