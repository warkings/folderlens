using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace FolderLens;

public sealed class FolderIndexService : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, IReadOnlyList<ThumbnailItem>> _previewCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly LinkedList<string> _previewOrder = [];
    private const int PreviewCacheLimit = 28;

    public async Task<IReadOnlyList<FolderEntry>> BuildAsync(IEnumerable<string> roots, CancellationToken cancellationToken = default)
    {
        var cleanRoots = roots.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        return await Task.Run(() =>
        {
            var found = new List<FolderEntry>();
            foreach (var root in cleanRoots)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AddFolder(found, root, root, cancellationToken);

                try
                {
                    foreach (var directory in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        AddFolder(found, root, directory, cancellationToken);
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
            }

            return (IReadOnlyList<FolderEntry>)found
                .OrderBy(entry => entry.Name, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(entry => entry.Path, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }, cancellationToken);
    }

    private static void AddFolder(List<FolderEntry> target, string root, string path, CancellationToken token)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            var relative = string.Equals(root, path, StringComparison.OrdinalIgnoreCase)
                ? "Carpeta configurada"
                : Path.GetRelativePath(root, path);
            target.Add(new FolderEntry
            {
                Path = path,
                Name = new DirectoryInfo(path).Name,
                RelativePath = relative
            });
        }
        catch (UnauthorizedAccessException) { }
        catch (DirectoryNotFoundException) { }
    }

    public async Task<PreviewState> LoadPreviewAsync(FolderEntry folder, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (_previewCache.TryGetValue(folder.Path, out var cached))
                return CreateState(folder, cached, null);
        }

        var images = await Task.Run(() => LoadImages(folder.Path, cancellationToken), cancellationToken);
        lock (_gate)
        {
            _previewCache[folder.Path] = images;
            _previewOrder.Remove(folder.Path);
            _previewOrder.AddLast(folder.Path);
            while (_previewOrder.Count > PreviewCacheLimit)
            {
                var oldest = _previewOrder.First?.Value;
                if (oldest is null) break;
                _previewOrder.RemoveFirst();
                _previewCache.Remove(oldest);
            }
        }

        return CreateState(folder, images, images.Count == 0 ? "No hay fotos en esta carpeta" : null);
    }

    private static IReadOnlyList<ThumbnailItem> LoadImages(string folderPath, CancellationToken token)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif", ".tif", ".tiff" };
        var items = new List<ThumbnailItem>(5);
        try
        {
            foreach (var file in Directory.EnumerateFiles(folderPath, "*", SearchOption.TopDirectoryOnly))
            {
                token.ThrowIfCancellationRequested();
                if (!allowed.Contains(Path.GetExtension(file))) continue;
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.DecodePixelWidth = 172;
                    bitmap.UriSource = new Uri(file);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    items.Add(new ThumbnailItem { Image = bitmap, FileName = Path.GetFileName(file) });
                    if (items.Count >= 5) break;
                }
                catch { }
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (DirectoryNotFoundException) { }
        catch (IOException) { }

        return new ReadOnlyCollection<ThumbnailItem>(items);
    }

    private static PreviewState CreateState(FolderEntry folder, IReadOnlyList<ThumbnailItem> images, string? message)
    {
        var state = new PreviewState { Folder = folder, Message = message };
        foreach (var image in images) state.Images.Add(image);
        return state;
    }

    public void ClearPreviewCache()
    {
        lock (_gate)
        {
            _previewCache.Clear();
            _previewOrder.Clear();
        }
    }

    public void Dispose() { }
}
