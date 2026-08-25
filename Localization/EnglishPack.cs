using System.Collections.Generic;

namespace FolderLens;

internal static class EnglishPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "English",
        ["main.searchHint"] = "Search a folder…",
        ["main.resultsHeader"] = "FOLDERS FOUND",
        ["main.folder.one"] = "{0} folder",
        ["main.folder.many"] = "{0} folders",
        ["main.previewHover"] = "Hover over a folder",
        ["main.previewNoPhotos"] = "No photos to show",
        ["main.previewLoading"] = "Loading photos…",
        ["main.previewChoose"] = "Choose a folder",
        ["main.previewPhotosHere"] = "Photos appear here",
        ["main.noPhotosFolder"] = "No photos in this folder",
        ["main.emptyChooseTitle"] = "Choose your search folders",
        ["main.emptyChooseMessage"] = "Add a folder from Settings. FolderLens will search inside it without touching the rest of your PC.",
        ["main.emptyNoMatchTitle"] = "We couldn't find that folder",
        ["main.emptyNoMatchMessage"] = "Try another part of the name or location.",
        ["main.emptyTypeTitle"] = "Type to search",
        ["main.emptyTypeMessage"] = "Results will appear below.",
        ["folder.configured"] = "Configured folder",
        ["tray.tooltip"] = "FolderLens · folder search",
        ["tray.open"] = "Open search",
        ["tray.refresh"] = "Refresh index",
        ["tray.settings"] = "Settings",
        ["tray.exit"] = "Exit",
        ["settings.title"] = "Settings · FolderLens",
        ["settings.eyebrow"] = "SETTINGS",
        ["settings.heading"] = "Where do you want to search?",
        ["settings.description"] = "FolderLens will only check these folders and everything inside them.",
        ["settings.addFolder"] = "＋  Add folder",
        ["settings.removeFolder"] = "－  Remove",
        ["settings.hotkeyTitle"] = "Open shortcut",
        ["settings.hotkey.initialHelp"] = "Click the shortcut and press a combination with Ctrl, Alt, or Shift.",
        ["settings.hotkey.capturePrompt"] = "Press the keys…",
        ["settings.hotkey.captureHelp"] = "Hold Ctrl, Alt, or Shift and press another key. Esc cancels.",
        ["settings.hotkey.applied"] = "{0} will be applied when you save.",
        ["settings.startup"] = "Open FolderLens when Windows starts",
        ["settings.footer"] = "The searcher stays available from the system tray icon.",
        ["settings.cancel"] = "Cancel",
        ["settings.save"] = "Save changes",
        ["settings.languageDetected"] = "Language: {0} · detected automatically",
        ["dialog.folderDescription"] = "Choose a folder to search",
        ["hotkey.windowsReserved"] = "Use Ctrl, Alt, or Shift; the Windows key is reserved.",
        ["hotkey.modifierOnly"] = "Good. Keep holding it and press another key.",
        ["hotkey.invalid"] = "That key cannot be used. Try another combination.",
        ["hotkey.noModifier"] = "Use Ctrl, Alt, or Shift with another key. F1–F24 can also be used alone."
    };
}
