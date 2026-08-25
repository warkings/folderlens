using System.IO;
using System.Text.Json;

namespace FolderLens;

public sealed class IndexCacheStore
{
    private static readonly JsonSerializerOptions Options = new();
    private readonly string _cachePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FolderLens",
        "folder-index.json");

    public IReadOnlyList<FolderEntry> TryLoad(IEnumerable<string> roots)
    {
        try
        {
            if (!File.Exists(_cachePath)) return [];
            var cache = JsonSerializer.Deserialize<IndexCache>(File.ReadAllText(_cachePath), Options);
            if (cache is null) return [];

            var normalizedRoots = Normalize(roots);
            if (!normalizedRoots.SequenceEqual(cache.Roots, StringComparer.OrdinalIgnoreCase)) return [];
            return cache.Folders.Where(folder => Directory.Exists(folder.Path)).ToArray();
        }
        catch
        {
            return [];
        }
    }

    public void Save(IEnumerable<string> roots, IEnumerable<FolderEntry> folders)
    {
        try
        {
            var directory = Path.GetDirectoryName(_cachePath);
            if (directory is not null) Directory.CreateDirectory(directory);
            var cache = new IndexCache
            {
                Roots = Normalize(roots),
                Folders = folders.Take(50000).ToList()
            };
            File.WriteAllText(_cachePath, JsonSerializer.Serialize(cache, Options));
        }
        catch
        {
            // El caché es opcional: una falla de escritura nunca bloquea la búsqueda.
        }
    }

    private static List<string> Normalize(IEnumerable<string> paths) => paths
        .Where(path => !string.IsNullOrWhiteSpace(path))
        .Select(Path.GetFullPath)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
        .ToList();

    private sealed class IndexCache
    {
        public List<string> Roots { get; set; } = [];
        public List<FolderEntry> Folders { get; set; } = [];
    }
}
