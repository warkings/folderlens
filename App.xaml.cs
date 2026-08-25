using System.Windows;

namespace FolderLens;

public partial class App : System.Windows.Application
{
    public static readonly SettingsStore Settings = new();
    public static readonly FolderIndexService Index = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Settings.Load();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Index.Dispose();
        base.OnExit(e);
    }
}
