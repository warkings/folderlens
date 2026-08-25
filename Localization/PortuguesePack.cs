using System.Collections.Generic;

namespace FolderLens;

internal static class PortuguesePack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "Português",
        ["main.searchHint"] = "Buscar uma pasta…",
        ["main.resultsHeader"] = "PASTAS ENCONTRADAS",
        ["main.folder.one"] = "{0} pasta",
        ["main.folder.many"] = "{0} pastas",
        ["main.previewHover"] = "Passe o mouse sobre uma pasta",
        ["main.previewNoPhotos"] = "Nenhuma foto para mostrar",
        ["main.previewLoading"] = "Carregando fotos…",
        ["main.previewChoose"] = "Escolha uma pasta",
        ["main.previewPhotosHere"] = "As fotos aparecerão aqui",
        ["main.noPhotosFolder"] = "Não há fotos nesta pasta",
        ["main.emptyChooseTitle"] = "Escolha suas pastas de busca",
        ["main.emptyChooseMessage"] = "Adicione uma pasta em Configurações. O FolderLens buscará dentro dela sem tocar no restante do PC.",
        ["main.emptyNoMatchTitle"] = "Não encontramos essa pasta",
        ["main.emptyNoMatchMessage"] = "Tente outra parte do nome ou do local.",
        ["main.emptyTypeTitle"] = "Digite para buscar",
        ["main.emptyTypeMessage"] = "Os resultados aparecerão abaixo.",
        ["folder.configured"] = "Pasta configurada",
        ["tray.tooltip"] = "FolderLens · busca de pastas",
        ["tray.open"] = "Abrir busca",
        ["tray.refresh"] = "Atualizar índice",
        ["tray.settings"] = "Configurações",
        ["tray.exit"] = "Sair",
        ["settings.title"] = "Configurações · FolderLens",
        ["settings.eyebrow"] = "CONFIGURAÇÕES",
        ["settings.heading"] = "Onde você quer buscar?",
        ["settings.description"] = "O FolderLens verificará apenas estas pastas e tudo o que estiver dentro delas.",
        ["settings.addFolder"] = "＋  Adicionar pasta",
        ["settings.removeFolder"] = "－  Remover",
        ["settings.hotkeyTitle"] = "Atalho para abrir",
        ["settings.hotkey.initialHelp"] = "Clique no atalho e pressione uma combinação com Ctrl, Alt ou Shift.",
        ["settings.hotkey.capturePrompt"] = "Pressione as teclas…",
        ["settings.hotkey.captureHelp"] = "Mantenha Ctrl, Alt ou Shift pressionado e pressione outra tecla. Esc cancela.",
        ["settings.hotkey.applied"] = "{0} será aplicado ao salvar.",
        ["settings.startup"] = "Abrir o FolderLens ao iniciar o Windows",
        ["settings.footer"] = "A busca fica disponível pelo ícone na bandeja do sistema.",
        ["settings.cancel"] = "Cancelar",
        ["settings.save"] = "Salvar alterações",
        ["settings.languageDetected"] = "Idioma: {0} · detectado automaticamente",
        ["dialog.folderDescription"] = "Escolha uma pasta para buscar",
        ["hotkey.windowsReserved"] = "Use Ctrl, Alt ou Shift; a tecla Windows é reservada.",
        ["hotkey.modifierOnly"] = "Ótimo. Mantenha-a pressionada e pressione outra tecla.",
        ["hotkey.invalid"] = "Essa tecla não pode ser usada. Tente outra combinação.",
        ["hotkey.noModifier"] = "Use Ctrl, Alt ou Shift com outra tecla. F1–F24 também podem ser usados sozinhos."
    };
}
