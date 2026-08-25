using System.Collections.Generic;

namespace FolderLens;

internal static class RussianPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "Русский",
        ["main.searchHint"] = "Поиск папки…",
        ["main.resultsHeader"] = "НАЙДЕННЫЕ ПАПКИ",
        ["main.folder.one"] = "Папок: {0}",
        ["main.folder.many"] = "Папок: {0}",
        ["main.previewHover"] = "Наведите курсор на папку",
        ["main.previewNoPhotos"] = "Нет фотографий для показа",
        ["main.previewLoading"] = "Загрузка фотографий…",
        ["main.previewChoose"] = "Выберите папку",
        ["main.previewPhotosHere"] = "Фотографии появятся здесь",
        ["main.noPhotosFolder"] = "В этой папке нет фотографий",
        ["main.emptyChooseTitle"] = "Выберите папки для поиска",
        ["main.emptyChooseMessage"] = "Добавьте папку в настройках. FolderLens будет искать внутри неё, не затрагивая остальную часть компьютера.",
        ["main.emptyNoMatchTitle"] = "Папка не найдена",
        ["main.emptyNoMatchMessage"] = "Попробуйте другую часть имени или расположения.",
        ["main.emptyTypeTitle"] = "Введите запрос для поиска",
        ["main.emptyTypeMessage"] = "Результаты появятся ниже.",
        ["folder.configured"] = "Настроенная папка",
        ["tray.tooltip"] = "FolderLens · поиск папок",
        ["tray.open"] = "Открыть поиск",
        ["tray.refresh"] = "Обновить индекс",
        ["tray.settings"] = "Настройки",
        ["tray.exit"] = "Выйти",
        ["settings.title"] = "Настройки · FolderLens",
        ["settings.eyebrow"] = "НАСТРОЙКИ",
        ["settings.heading"] = "Где искать?",
        ["settings.description"] = "FolderLens будет проверять только эти папки и всё внутри них.",
        ["settings.addFolder"] = "＋  Добавить папку",
        ["settings.removeFolder"] = "－  Удалить",
        ["settings.hotkeyTitle"] = "Сочетание для открытия",
        ["settings.hotkey.initialHelp"] = "Нажмите на сочетание и задайте комбинацию с Ctrl, Alt или Shift.",
        ["settings.hotkey.capturePrompt"] = "Нажмите клавиши…",
        ["settings.hotkey.captureHelp"] = "Удерживайте Ctrl, Alt или Shift и нажмите другую клавишу. Esc отменяет действие.",
        ["settings.hotkey.applied"] = "Сочетание {0} будет применено после сохранения.",
        ["settings.startup"] = "Открывать FolderLens при запуске Windows",
        ["settings.footer"] = "Поиск доступен через значок в области уведомлений.",
        ["settings.cancel"] = "Отмена",
        ["settings.save"] = "Сохранить изменения",
        ["settings.languageDetected"] = "Язык: {0} · определён автоматически",
        ["dialog.folderDescription"] = "Выберите папку для поиска",
        ["hotkey.windowsReserved"] = "Используйте Ctrl, Alt или Shift; клавиша Windows зарезервирована.",
        ["hotkey.modifierOnly"] = "Хорошо. Удерживайте её и нажмите другую клавишу.",
        ["hotkey.invalid"] = "Эту клавишу нельзя использовать. Попробуйте другую комбинацию.",
        ["hotkey.noModifier"] = "Используйте Ctrl, Alt или Shift с другой клавишей. F1–F24 также можно использовать отдельно."
    };
}
