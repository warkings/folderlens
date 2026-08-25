using System.Collections.ObjectModel;
using System.Windows;

namespace FolderLens;

public partial class SettingsWindow : Window
{
    private readonly ObservableCollection<string> _roots = [];
    public AppSettings? Result { get; private set; }

    public SettingsWindow(Window owner)
    {
        InitializeComponent();
        Owner = owner;
        foreach (var root in App.Settings.Current.SearchRoots) _roots.Add(root);
        RootsList.ItemsSource = _roots;
        StartWithWindows.IsChecked = App.Settings.Current.StartWithWindows;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Elegí una carpeta para buscar",
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

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        Result = new AppSettings
        {
            SearchRoots = _roots.ToList(),
            StartWithWindows = StartWithWindows.IsChecked == true
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
