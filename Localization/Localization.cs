using System.Globalization;

namespace FolderLens;

public static class Localization
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Catalogs =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["es"] = SpanishPack.Create(),
            ["en"] = EnglishPack.Create(),
            ["zh"] = ChinesePack.Create(),
            ["hi"] = HindiPack.Create(),
            ["fr"] = FrenchPack.Create(),
            ["ar"] = ArabicPack.Create(),
            ["bn"] = BengaliPack.Create(),
            ["pt"] = PortuguesePack.Create(),
            ["ru"] = RussianPack.Create(),
            ["id"] = IndonesianPack.Create()
        };

    private static readonly CultureInfo DetectedCulture = DetectCulture();

    public static string Code => DetectedCulture.TwoLetterISOLanguageName.ToLowerInvariant();

    public static string LanguageName => Get("language.name");

    public static CultureInfo Culture => DetectedCulture;

    public static string Get(string key)
    {
        if (Catalogs.TryGetValue(Code, out var catalog) && catalog.TryGetValue(key, out var text)) return text;
        return Catalogs["es"].TryGetValue(key, out var fallback) ? fallback : key;
    }

    public static string Format(string key, params object[] args) =>
        string.Format(Culture, Get(key), args);

    public static string FolderCount(int count) =>
        Format(count == 1 ? "main.folder.one" : "main.folder.many", count);

    private static CultureInfo DetectCulture()
    {
        var current = CultureInfo.CurrentUICulture;
        if (Catalogs.ContainsKey(Normalize(current))) return current;

        var installed = CultureInfo.InstalledUICulture;
        if (Catalogs.ContainsKey(Normalize(installed))) return installed;

        return CultureInfo.GetCultureInfo("es");
    }

    private static string Normalize(CultureInfo culture) =>
        culture.TwoLetterISOLanguageName.ToLowerInvariant();
}
