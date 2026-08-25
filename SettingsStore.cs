using System.Text.Json;
using System.IO;
using Microsoft.Win32;

namespace FolderLens;

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FolderLens");
    private readonly string _filePath;

    public AppSettings Current { get; private set; } = new();

    public SettingsStore()
    {
        _filePath = Path.Combine(_directory, "settings.json");
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_filePath), JsonOptions) ?? new AppSettings();
            }
        }
        catch
        {
            Current = new AppSettings();
        }

        Current.SearchRoots = Current.SearchRoots
            .Where(Directory.Exists)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void Save(AppSettings settings)
    {
        Current = settings;
        Directory.CreateDirectory(_directory);
        File.WriteAllText(_filePath, JsonSerializer.Serialize(Current, JsonOptions));
        SetStartup(Current.StartWithWindows);
    }

    private static void SetStartup(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
            if (key is null) return;

            const string appName = "FolderLens";
            if (enabled)
            {
                var executable = Environment.ProcessPath;
                if (!string.IsNullOrWhiteSpace(executable))
                    key.SetValue(appName, $"\"{executable}\"");
            }
            else
            {
                key.DeleteValue(appName, throwOnMissingValue: false);
            }
        }
        catch
        {
            // El inicio automático es opcional; la aplicación sigue funcionando aunque Windows lo deniegue.
        }
    }
}
