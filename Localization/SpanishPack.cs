using System.Collections.Generic;

namespace FolderLens;

internal static class SpanishPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "Español",
        ["main.searchHint"] = "Buscar una carpeta…",
        ["main.resultsHeader"] = "CARPETAS ENCONTRADAS",
        ["main.folder.one"] = "{0} carpeta",
        ["main.folder.many"] = "{0} carpetas",
        ["main.previewHover"] = "Pasá el mouse por una carpeta",
        ["main.previewNoPhotos"] = "No hay fotos para mostrar",
        ["main.previewLoading"] = "Cargando fotos…",
        ["main.previewChoose"] = "Elegí una carpeta",
        ["main.previewPhotosHere"] = "Las fotos aparecen acá",
        ["main.noPhotosFolder"] = "No hay fotos en esta carpeta",
        ["main.emptyChooseTitle"] = "Elegí tus carpetas de búsqueda",
        ["main.emptyChooseMessage"] = "Agregá una carpeta desde Configuración. FolderLens buscará dentro de ella sin tocar el resto de tu PC.",
        ["main.emptyNoMatchTitle"] = "No encontramos esa carpeta",
        ["main.emptyNoMatchMessage"] = "Probá con otra parte del nombre o de su ubicación.",
        ["main.emptyTypeTitle"] = "Escribí para buscar",
        ["main.emptyTypeMessage"] = "Los resultados aparecen acá debajo.",
        ["folder.configured"] = "Carpeta configurada",
        ["tray.tooltip"] = "FolderLens · buscador de carpetas",
        ["tray.open"] = "Abrir buscador",
        ["tray.refresh"] = "Actualizar índice",
        ["tray.settings"] = "Configuración",
        ["tray.exit"] = "Salir",
        ["settings.title"] = "Configuración · FolderLens",
        ["settings.eyebrow"] = "CONFIGURACIÓN",
        ["settings.heading"] = "¿Dónde querés buscar?",
        ["settings.description"] = "FolderLens solo revisará estas carpetas y todo lo que haya dentro.",
        ["settings.addFolder"] = "＋  Agregar carpeta",
        ["settings.removeFolder"] = "－  Quitar",
        ["settings.hotkeyTitle"] = "Atajo para abrir",
        ["settings.hotkey.initialHelp"] = "Hacé clic en el atajo y presioná una combinación con Ctrl, Alt o Shift.",
        ["settings.hotkey.capturePrompt"] = "Presioná las teclas…",
        ["settings.hotkey.captureHelp"] = "Mantené Ctrl, Alt o Shift y presioná otra tecla. Esc cancela.",
        ["settings.hotkey.applied"] = "{0} se aplicará al guardar.",
        ["settings.startup"] = "Abrir FolderLens al iniciar Windows",
        ["settings.footer"] = "El buscador queda disponible desde el ícono junto al reloj.",
        ["settings.cancel"] = "Cancelar",
        ["settings.save"] = "Guardar cambios",
        ["settings.languageDetected"] = "Idioma: {0} · detectado automáticamente",
        ["dialog.folderDescription"] = "Elegí una carpeta para buscar",
        ["hotkey.windowsReserved"] = "Usá Ctrl, Alt o Shift; la tecla Windows está reservada.",
        ["hotkey.modifierOnly"] = "Bien. Sin soltarla, presioná otra tecla.",
        ["hotkey.invalid"] = "Esa tecla no se puede usar. Probá otra combinación.",
        ["hotkey.noModifier"] = "Usá Ctrl, Alt o Shift junto con otra tecla. F1–F24 también funcionan solas."
    };
}
