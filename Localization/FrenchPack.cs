using System.Collections.Generic;

namespace FolderLens;

internal static class FrenchPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "Français",
        ["main.searchHint"] = "Rechercher un dossier…",
        ["main.resultsHeader"] = "DOSSIERS TROUVÉS",
        ["main.folder.one"] = "{0} dossier",
        ["main.folder.many"] = "{0} dossiers",
        ["main.previewHover"] = "Survolez un dossier",
        ["main.previewNoPhotos"] = "Aucune photo à afficher",
        ["main.previewLoading"] = "Chargement des photos…",
        ["main.previewChoose"] = "Choisissez un dossier",
        ["main.previewPhotosHere"] = "Les photos apparaîtront ici",
        ["main.noPhotosFolder"] = "Aucune photo dans ce dossier",
        ["main.emptyChooseTitle"] = "Choisissez vos dossiers de recherche",
        ["main.emptyChooseMessage"] = "Ajoutez un dossier dans les Paramètres. FolderLens cherchera à l'intérieur sans toucher au reste du PC.",
        ["main.emptyNoMatchTitle"] = "Dossier introuvable",
        ["main.emptyNoMatchMessage"] = "Essayez une autre partie du nom ou de l'emplacement.",
        ["main.emptyTypeTitle"] = "Écrivez pour rechercher",
        ["main.emptyTypeMessage"] = "Les résultats apparaîtront ci-dessous.",
        ["folder.configured"] = "Dossier configuré",
        ["tray.tooltip"] = "FolderLens · recherche de dossiers",
        ["tray.open"] = "Ouvrir la recherche",
        ["tray.refresh"] = "Actualiser l'index",
        ["tray.settings"] = "Paramètres",
        ["tray.exit"] = "Quitter",
        ["settings.title"] = "Paramètres · FolderLens",
        ["settings.eyebrow"] = "PARAMÈTRES",
        ["settings.heading"] = "Où voulez-vous rechercher ?",
        ["settings.description"] = "FolderLens vérifiera uniquement ces dossiers et tout ce qu'ils contiennent.",
        ["settings.addFolder"] = "＋  Ajouter un dossier",
        ["settings.removeFolder"] = "－  Supprimer",
        ["settings.hotkeyTitle"] = "Raccourci d'ouverture",
        ["settings.hotkey.initialHelp"] = "Cliquez sur le raccourci et appuyez sur une combinaison avec Ctrl, Alt ou Shift.",
        ["settings.hotkey.capturePrompt"] = "Appuyez sur les touches…",
        ["settings.hotkey.captureHelp"] = "Maintenez Ctrl, Alt ou Shift et appuyez sur une autre touche. Échap annule.",
        ["settings.hotkey.applied"] = "{0} sera appliqué à l'enregistrement.",
        ["settings.startup"] = "Ouvrir FolderLens au démarrage de Windows",
        ["settings.footer"] = "La recherche reste disponible depuis l'icône de la zone de notification.",
        ["settings.cancel"] = "Annuler",
        ["settings.save"] = "Enregistrer les modifications",
        ["settings.languageDetected"] = "Langue : {0} · détectée automatiquement",
        ["dialog.folderDescription"] = "Choisissez un dossier à rechercher",
        ["hotkey.windowsReserved"] = "Utilisez Ctrl, Alt ou Shift ; la touche Windows est réservée.",
        ["hotkey.modifierOnly"] = "Très bien. Maintenez-la et appuyez sur une autre touche.",
        ["hotkey.invalid"] = "Cette touche ne peut pas être utilisée. Essayez une autre combinaison.",
        ["hotkey.noModifier"] = "Utilisez Ctrl, Alt ou Shift avec une autre touche. F1–F24 peuvent aussi être utilisés seuls."
    };
}
