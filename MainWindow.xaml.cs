using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    private const int VkLeftControl = 0xA2;
    private const int VkRightControl = 0xA3;
    private const int VkLeftShift = 0xA0;
    private const int VkRightShift = 0xA1;
    private const int VkLeftWindows = 0x5B;
    private const int VkRightWindows = 0x5C;
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
    private readonly DispatcherTimer _previewHoverTimer;
    private FolderEntry? _pendingPreviewFolder;
    private readonly List<FileSystemWatcher> _indexWatchers = [];
    private DispatcherTimer? _indexRefreshTimer;
    private bool _allowExit;
    private bool _loaded;
    private bool _suppressDeactivate;
    private bool _opening;
    private bool _hotkeyHandled;

    public MainWindow()
    {
        InitializeComponent();
        ApplyLocalization();
        ResultsListBox.ItemsSource = _visibleFolders;
        _previewHoverTimer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(120)
        };
        _previewHoverTimer.Tick += PreviewHoverTimer_Tick;
        _keyboardProc = KeyboardHookCallback;
        _mouseProc = MouseHookCallback;
    }

    private void ApplyLocalization()
    {
        SearchHint.Text = Localization.Get("main.searchHint");
        ResultsHeaderText.Text = Localization.Get("main.resultsHeader");
        EmptyTitle.Text = Localization.Get("main.emptyNoMatchTitle");
        EmptyMessage.Text = Localization.Get("main.emptyNoMatchMessage");
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_loaded) return;
        _loaded = true;
        ConfigureTrayIcon();
        InstallKeyboardHook();
        InstallMouseHook();
        var arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
        var showSettingsOnStartup = arguments.Any(argument => string.Equals(argument, "--settings", StringComparison.OrdinalIgnoreCase));
        var showOnStartup = arguments.Any(argument => string.Equals(argument, "--show", StringComparison.OrdinalIgnoreCase));
        if (showSettingsOnStartup)
        {
            Hide();
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(OpenSettings));
        }
        else if (showOnStartup)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(ShowSearchWindow));
        }
        else
        {
            Hide();
        }
        var cachedFolders = _cache.TryLoad(App.Settings.Current.SearchRoots);
        if (cachedFolders.Count > 0)
        {
            _allFolders = cachedFolders;
            ApplyFilter();
        }
        ConfigureIndexWatchers();
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
            Text = Localization.Get("tray.tooltip"),
            Visible = true
        };
        _trayIcon.MouseClick += TrayIcon_MouseClick;
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add(Localization.Get("tray.open"), null, (_, _) => ShowSearchWindow());
        menu.Items.Add(Localization.Get("tray.refresh"), null, (_, _) => RefreshIndexAsync());
        menu.Items.Add(Localization.Get("tray.settings"), null, (_, _) => OpenSettings());
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add(Localization.Get("tray.exit"), null, (_, _) => ExitApplication());
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
            if (IsAnyHotkeyModifierDown() && attempts < 32) return;

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

    private void ConfigureIndexWatchers()
    {
        foreach (var watcher in _indexWatchers) watcher.Dispose();
        _indexWatchers.Clear();
        foreach (var root in App.Settings.Current.SearchRoots.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var watcher = new FileSystemWatcher(root)
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.DirectoryName
                };
                watcher.Created += IndexWatcher_Changed;
                watcher.Deleted += IndexWatcher_Changed;
                watcher.Renamed += IndexWatcher_Renamed;
                watcher.EnableRaisingEvents = true;
                _indexWatchers.Add(watcher);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
    private void IndexWatcher_Changed(object sender, FileSystemEventArgs e) =>
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(ScheduleIndexRefresh));

    private void IndexWatcher_Renamed(object sender, RenamedEventArgs e) =>
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(ScheduleIndexRefresh));

    private void ScheduleIndexRefresh()
    {
        if (_indexRefreshTimer is null)
        {
            _indexRefreshTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(650)
            };
            _indexRefreshTimer.Tick += (_, _) =>
            {
                _indexRefreshTimer.Stop();
                RefreshIndexAsync();
            };
        }
        _indexRefreshTimer.Stop();
        _indexRefreshTimer.Start();
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
        UpdateResultsHeight(limited.Length);
        ResultCountText.Text = Localization.FolderCount(matches.Count);

        ResultsArea.Visibility = string.IsNullOrWhiteSpace(query) ? Visibility.Collapsed : Visibility.Visible;

        var hasResults = _visibleFolders.Count > 0;
        var showPreview = hasResults && !string.IsNullOrWhiteSpace(query);
        PreviewPanel.Visibility = showPreview ? Visibility.Visible : Visibility.Collapsed;
        PreviewGap.Visibility = showPreview ? Visibility.Visible : Visibility.Collapsed;
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(KeepWindowOnScreen));


        if (hasResults && !string.IsNullOrWhiteSpace(query))
        {
            ResultsListBox.SelectedIndex = 0;
            _ = LoadFolderPreviewAsync(_visibleFolders[0]);
        }
        else
        {
            _previewCts?.Cancel();
            SetPreviewState(new PreviewState
            {
                Message = string.IsNullOrWhiteSpace(query)
                    ? Localization.Get("main.previewHover")
                    : Localization.Get("main.previewNoPhotos")
            });
        }
        EmptyState.Visibility = hasResults ? Visibility.Collapsed : Visibility.Visible;
        ResultsListBox.Visibility = hasResults ? Visibility.Visible : Visibility.Collapsed;
        if (_allFolders.Count == 0)
        {
            EmptyTitle.Text = Localization.Get("main.emptyChooseTitle");
            EmptyMessage.Text = Localization.Get("main.emptyChooseMessage");
        }
        else if (!hasResults)
        {
            EmptyTitle.Text = Localization.Get("main.emptyNoMatchTitle");
            EmptyMessage.Text = Localization.Get("main.emptyNoMatchMessage");
        }
        else
        {
            EmptyTitle.Text = Localization.Get("main.emptyTypeTitle");
            EmptyMessage.Text = Localization.Get("main.emptyTypeMessage");
        }

    }
    private void UpdateResultsHeight(int resultCount)
    {
        const double rowHeight = 68;
        const double maxHeight = 390;
        var height = resultCount == 0
            ? 144
            : Math.Min(maxHeight, resultCount * rowHeight);
        ResultsListRow.Height = new GridLength(height);
    }

    private void KeepWindowOnScreen()
    {
        if (!IsVisible) return;
        UpdateLayout();
        var workArea = SystemParameters.WorkArea;
        const double edge = 16;
        if (Left + ActualWidth > workArea.Right - edge)
            Left = Math.Max(workArea.Left + edge, workArea.Right - ActualWidth - edge);
        if (Top + ActualHeight > workArea.Bottom - edge)
            Top = Math.Max(workArea.Top + edge, workArea.Bottom - ActualHeight - edge);
    }

    private void Folder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not FolderEntry folder) return;
        _pendingPreviewFolder = folder;
        _previewHoverTimer.Stop();
        _previewHoverTimer.Start();
    }

    private void Folder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is FolderEntry folder
            && string.Equals(_pendingPreviewFolder?.Path, folder.Path, StringComparison.OrdinalIgnoreCase))
        {
            _pendingPreviewFolder = null;
            _previewHoverTimer.Stop();
        }
    }
    private void PreviewHoverTimer_Tick(object? sender, EventArgs e)
    {
        _previewHoverTimer.Stop();
        var folder = _pendingPreviewFolder;
        _pendingPreviewFolder = null;
        if (folder is not null)
            _ = LoadFolderPreviewAsync(folder);
    }
    private async Task LoadFolderPreviewAsync(FolderEntry folder)
    {
        _previewCts?.Cancel();
        _previewCts = new CancellationTokenSource();
        var token = _previewCts.Token;
        SetPreviewState(new PreviewState { Folder = folder, Message = Localization.Get("main.previewLoading") });

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
        PreviewTitle.Text = state.Folder?.Name ?? Localization.Get("main.previewChoose");
        PreviewPath.Text = state.Folder?.LocationLabel ?? Localization.Get("main.previewPhotosHere");
        PreviewMessage.Text = state.Message ?? string.Empty;
        PreviewMessage.Visibility = Visibility.Collapsed;
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
            ConfigureIndexWatchers();
            _hotkeyHandled = false;
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

        _previewHoverTimer.Stop();
        _indexRefreshTimer?.Stop();
        foreach (var watcher in _indexWatchers) watcher.Dispose();
        _indexWatchers.Clear();
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
        if (code >= 0 && !_suppressDeactivate)
        {
            var virtualKey = Marshal.ReadInt32(lParam);
            var message = wParam.ToInt32();
            var hotkey = HotkeyGesture.FromSettings(App.Settings.Current);
            var isHotkeyDown = virtualKey == hotkey.VirtualKey && (message == WmKeyDown || message == WmSysKeyDown);
            var isHotkeyUp = virtualKey == hotkey.VirtualKey && (message == WmKeyUp || message == WmSysKeyUp);

            if (isHotkeyDown && !_hotkeyHandled && HotkeyModifiersMatch(hotkey.Modifiers))
            {
                _hotkeyHandled = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(ShowSearchWindow));
                return new IntPtr(1);
            }

            if (isHotkeyUp && _hotkeyHandled)
            {
                _hotkeyHandled = false;
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

    private static bool HotkeyModifiersMatch(HotkeyModifiers expected) =>
        GetPressedHotkeyModifiers() == expected
        && !IsVirtualKeyDown(VkLeftWindows)
        && !IsVirtualKeyDown(VkRightWindows);

    private static bool IsAnyHotkeyModifierDown() =>
        GetPressedHotkeyModifiers() != HotkeyModifiers.None
        || IsVirtualKeyDown(VkLeftWindows)
        || IsVirtualKeyDown(VkRightWindows);

    private static HotkeyModifiers GetPressedHotkeyModifiers()
    {
        var modifiers = HotkeyModifiers.None;
        if (IsVirtualKeyDown(VkLeftControl) || IsVirtualKeyDown(VkRightControl)) modifiers |= HotkeyModifiers.Control;
        if (IsVirtualKeyDown(VkLeftAlt) || IsVirtualKeyDown(VkRightAlt)) modifiers |= HotkeyModifiers.Alt;
        if (IsVirtualKeyDown(VkLeftShift) || IsVirtualKeyDown(VkRightShift)) modifiers |= HotkeyModifiers.Shift;
        return modifiers;
    }

    private static bool IsVirtualKeyDown(int virtualKey) =>
        (NativeMethods.GetAsyncKeyState(virtualKey) & 0x8000) != 0;

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
