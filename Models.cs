using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace FolderLens;

[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Control = 1,
    Alt = 2,
    Shift = 4
}

public sealed class AppSettings
{
    public List<string> SearchRoots { get; set; } = [];
    public bool StartWithWindows { get; set; }
    public int OpenHotkeyVirtualKey { get; set; } = 0x20;
    public HotkeyModifiers OpenHotkeyModifiers { get; set; } = HotkeyModifiers.Alt;
}

public sealed class FolderEntry
{
    public required string Path { get; init; }
    public required string Name { get; init; }
    public required string RelativePath { get; init; }
    [JsonIgnore]
    public string LocationLabel => string.IsNullOrWhiteSpace(RelativePath) ? Path : RelativePath;
}

public sealed class PreviewState
{
    public FolderEntry? Folder { get; init; }
    public ObservableCollection<ThumbnailItem> Images { get; } = [];
    public bool IsLoading { get; init; }
    public string? Message { get; init; }
}

public sealed class ThumbnailItem
{
    public required System.Windows.Media.ImageSource Image { get; init; }
    public required string FileName { get; init; }
}
