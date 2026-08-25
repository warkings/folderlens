using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MediaBrush = System.Windows.Media.Brush;

namespace FolderLens;

public partial class SettingsWindow : Window
{
    private readonly ObservableCollection<string> _roots = [];
    private HotkeyGesture _hotkey;
    private bool _capturingHotkey;
    public AppSettings? Result { get; private set; }

    public SettingsWindow(Window owner)
    {
        InitializeComponent();
        ApplyLocalization();
        Owner = owner;
        foreach (var root in App.Settings.Current.SearchRoots) _roots.Add(root);
        RootsList.ItemsSource = _roots;
        StartWithWindows.IsChecked = App.Settings.Current.StartWithWindows;
        _hotkey = HotkeyGesture.FromSettings(App.Settings.Current);
        UpdateHotkeyPresentation();
    }

    private void ApplyLocalization()
    {
        Title = Localization.Get("settings.title");
        SettingsEyebrow.Text = Localization.Get("settings.eyebrow");
        SettingsHeading.Text = Localization.Get("settings.heading");
        SettingsDescription.Text = Localization.Get("settings.description");
        AddFolderButton.Content = Localization.Get("settings.addFolder");
        RemoveFolderButton.Content = Localization.Get("settings.removeFolder");
        HotkeyTitle.Text = Localization.Get("settings.hotkeyTitle");
        HotkeyHelpText.Text = Localization.Get("settings.hotkey.initialHelp");
        StartWithWindows.Content = Localization.Get("settings.startup");
        LanguageStatusText.Text = Localization.Format("settings.languageDetected", Localization.LanguageName);
        SettingsFooterText.Text = Localization.Get("settings.footer");
        CancelButton.Content = Localization.Get("settings.cancel");
        SaveButton.Content = Localization.Get("settings.save");
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = Localization.Get("dialog.folderDescription"),
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false
        };
        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
        if (!_roots.Contains(dialog.SelectedPath, StringComparer.OrdinalIgnoreCase)) _roots.Add(dialog.SelectedPath);
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (RootsList.SelectedItem is string selected) _roots.Remove(selected);
    }

    private void Hotkey_Click(object sender, RoutedEventArgs e)
    {
        _capturingHotkey = true;
        HotkeyButton.Content = Localization.Get("settings.hotkey.capturePrompt");
        HotkeyButton.Background = (MediaBrush)FindResource("WarmSoftBrush");
        HotkeyButton.BorderBrush = (MediaBrush)FindResource("WarmBrush");
        HotkeyHelpText.Text = Localization.Get("settings.hotkey.captureHelp");
        HotkeyHelpText.Foreground = (MediaBrush)FindResource("WarmBrush");
        HotkeyButton.Focus();
        Keyboard.Focus(HotkeyButton);
    }

    private void HotkeyButton_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (!_capturingHotkey) return;
        e.Handled = true;

        var key = e.Key switch
        {
            Key.System => e.SystemKey,
            Key.ImeProcessed => e.ImeProcessedKey,
            Key.DeadCharProcessed => e.DeadCharProcessedKey,
            _ => e.Key
        };

        if (key == Key.Escape)
        {
            FinishHotkeyCapture(false);
            return;
        }

        if (!HotkeyGesture.TryCreate(key, Keyboard.Modifiers, out var hotkey, out var message))
        {
            HotkeyHelpText.Text = message;
            HotkeyHelpText.Foreground = (MediaBrush)FindResource("WarmBrush");
            return;
        }

        _hotkey = hotkey;
        FinishHotkeyCapture(true);
    }

    private void HotkeyButton_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_capturingHotkey) FinishHotkeyCapture(false);
    }

    private void FinishHotkeyCapture(bool accepted)
    {
        _capturingHotkey = false;
        UpdateHotkeyPresentation();
        HotkeyHelpText.Text = accepted
            ? Localization.Format("settings.hotkey.applied", _hotkey.DisplayText)
            : Localization.Get("settings.hotkey.initialHelp");
        HotkeyHelpText.Foreground = accepted
            ? (MediaBrush)FindResource("AccentBrush")
            : (MediaBrush)FindResource("MutedBrush");
    }

    private void UpdateHotkeyPresentation()
    {
        HotkeyButton.Content = _hotkey.DisplayText;
        HotkeyButton.Background = (MediaBrush)FindResource("AccentSoftBrush");
        HotkeyButton.BorderBrush = (MediaBrush)FindResource("AccentBrush");
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_capturingHotkey) FinishHotkeyCapture(false);
        Result = new AppSettings
        {
            SearchRoots = _roots.ToList(),
            StartWithWindows = StartWithWindows.IsChecked == true,
            OpenHotkeyVirtualKey = _hotkey.VirtualKey,
            OpenHotkeyModifiers = _hotkey.Modifiers,
        };
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
