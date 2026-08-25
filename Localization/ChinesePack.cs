using System.Collections.Generic;

namespace FolderLens;

internal static class ChinesePack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "中文",
        ["main.searchHint"] = "搜索文件夹…",
        ["main.resultsHeader"] = "找到的文件夹",
        ["main.folder.one"] = "{0} 个文件夹",
        ["main.folder.many"] = "{0} 个文件夹",
        ["main.previewHover"] = "将鼠标移到文件夹上",
        ["main.previewNoPhotos"] = "没有可显示的照片",
        ["main.previewLoading"] = "正在加载照片…",
        ["main.previewChoose"] = "选择一个文件夹",
        ["main.previewPhotosHere"] = "照片会显示在这里",
        ["main.noPhotosFolder"] = "此文件夹中没有照片",
        ["main.emptyChooseTitle"] = "选择要搜索的文件夹",
        ["main.emptyChooseMessage"] = "从“设置”中添加文件夹。FolderLens 只会搜索其中的内容，不会触碰电脑的其他位置。",
        ["main.emptyNoMatchTitle"] = "找不到该文件夹",
        ["main.emptyNoMatchMessage"] = "请尝试输入名称或位置的其他部分。",
        ["main.emptyTypeTitle"] = "输入内容开始搜索",
        ["main.emptyTypeMessage"] = "结果会显示在下方。",
        ["folder.configured"] = "已配置的文件夹",
        ["tray.tooltip"] = "FolderLens · 文件夹搜索",
        ["tray.open"] = "打开搜索",
        ["tray.refresh"] = "刷新索引",
        ["tray.settings"] = "设置",
        ["tray.exit"] = "退出",
        ["settings.title"] = "设置 · FolderLens",
        ["settings.eyebrow"] = "设置",
        ["settings.heading"] = "你想在哪里搜索？",
        ["settings.description"] = "FolderLens 只会检查这些文件夹及其内容。",
        ["settings.addFolder"] = "＋  添加文件夹",
        ["settings.removeFolder"] = "－  删除",
        ["settings.hotkeyTitle"] = "打开快捷键",
        ["settings.hotkey.initialHelp"] = "点击快捷键，然后按下包含 Ctrl、Alt 或 Shift 的组合键。",
        ["settings.hotkey.capturePrompt"] = "按下按键…",
        ["settings.hotkey.captureHelp"] = "按住 Ctrl、Alt 或 Shift，再按另一个键。Esc 取消。",
        ["settings.hotkey.applied"] = "保存后将应用 {0}。",
        ["settings.startup"] = "Windows 启动时打开 FolderLens",
        ["settings.footer"] = "可通过系统托盘图标使用搜索。",
        ["settings.cancel"] = "取消",
        ["settings.save"] = "保存更改",
        ["settings.languageDetected"] = "语言：{0} · 自动检测",
        ["dialog.folderDescription"] = "选择要搜索的文件夹",
        ["hotkey.windowsReserved"] = "请使用 Ctrl、Alt 或 Shift；Windows 键已保留。",
        ["hotkey.modifierOnly"] = "很好。请保持按住，再按另一个键。",
        ["hotkey.invalid"] = "无法使用此按键。请尝试其他组合。",
        ["hotkey.noModifier"] = "请将 Ctrl、Alt 或 Shift 与另一个键一起使用。F1–F24 也可单独使用。"
    };
}
