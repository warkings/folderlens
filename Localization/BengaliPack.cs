using System.Collections.Generic;

namespace FolderLens;

internal static class BengaliPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "বাংলা",
        ["main.searchHint"] = "ফোল্ডার খুঁজুন…",
        ["main.resultsHeader"] = "পাওয়া ফোল্ডার",
        ["main.folder.one"] = "{0}টি ফোল্ডার",
        ["main.folder.many"] = "{0}টি ফোল্ডার",
        ["main.previewHover"] = "একটি ফোল্ডারের ওপর মাউস রাখুন",
        ["main.previewNoPhotos"] = "দেখানোর মতো কোনো ছবি নেই",
        ["main.previewLoading"] = "ছবি লোড হচ্ছে…",
        ["main.previewChoose"] = "একটি ফোল্ডার বেছে নিন",
        ["main.previewPhotosHere"] = "ছবি এখানে দেখা যাবে",
        ["main.noPhotosFolder"] = "এই ফোল্ডারে কোনো ছবি নেই",
        ["main.emptyChooseTitle"] = "আপনার অনুসন্ধানের ফোল্ডার বেছে নিন",
        ["main.emptyChooseMessage"] = "সেটিংস থেকে একটি ফোল্ডার যোগ করুন। FolderLens আপনার PC-এর বাকি অংশ না ছুঁয়ে তার ভেতর অনুসন্ধান করবে।",
        ["main.emptyNoMatchTitle"] = "ফোল্ডারটি পাওয়া যায়নি",
        ["main.emptyNoMatchMessage"] = "নাম বা অবস্থানের অন্য অংশ দিয়ে চেষ্টা করুন।",
        ["main.emptyTypeTitle"] = "অনুসন্ধান করতে লিখুন",
        ["main.emptyTypeMessage"] = "ফলাফল নিচে দেখা যাবে।",
        ["folder.configured"] = "কনফিগার করা ফোল্ডার",
        ["tray.tooltip"] = "FolderLens · ফোল্ডার অনুসন্ধান",
        ["tray.open"] = "অনুসন্ধান খুলুন",
        ["tray.refresh"] = "ইনডেক্স রিফ্রেশ করুন",
        ["tray.settings"] = "সেটিংস",
        ["tray.exit"] = "প্রস্থান",
        ["settings.title"] = "সেটিংস · FolderLens",
        ["settings.eyebrow"] = "সেটিংস",
        ["settings.heading"] = "আপনি কোথায় অনুসন্ধান করতে চান?",
        ["settings.description"] = "FolderLens শুধু এই ফোল্ডারগুলো এবং এগুলোর ভেতরের সবকিছু পরীক্ষা করবে।",
        ["settings.addFolder"] = "＋  ফোল্ডার যোগ করুন",
        ["settings.removeFolder"] = "－  সরান",
        ["settings.hotkeyTitle"] = "খোলার শর্টকাট",
        ["settings.hotkey.initialHelp"] = "শর্টকাটে ক্লিক করে Ctrl, Alt বা Shift সহ একটি সমন্বয় চাপুন।",
        ["settings.hotkey.capturePrompt"] = "কীগুলো চাপুন…",
        ["settings.hotkey.captureHelp"] = "Ctrl, Alt বা Shift ধরে রেখে অন্য একটি কী চাপুন। Esc বাতিল করে।",
        ["settings.hotkey.applied"] = "সংরক্ষণ করলে {0} প্রয়োগ হবে।",
        ["settings.startup"] = "Windows চালু হলে FolderLens খুলুন",
        ["settings.footer"] = "সিস্টেম ট্রে আইকন থেকে অনুসন্ধানটি পাওয়া যাবে।",
        ["settings.cancel"] = "বাতিল",
        ["settings.save"] = "পরিবর্তন সংরক্ষণ করুন",
        ["settings.languageDetected"] = "ভাষা: {0} · স্বয়ংক্রিয়ভাবে শনাক্ত",
        ["dialog.folderDescription"] = "অনুসন্ধানের জন্য একটি ফোল্ডার বেছে নিন",
        ["hotkey.windowsReserved"] = "Ctrl, Alt বা Shift ব্যবহার করুন; Windows কী সংরক্ষিত।",
        ["hotkey.modifierOnly"] = "ভালো। এটি ধরে রেখে অন্য একটি কী চাপুন।",
        ["hotkey.invalid"] = "এই কী ব্যবহার করা যাবে না। অন্য সমন্বয় চেষ্টা করুন।",
        ["hotkey.noModifier"] = "Ctrl, Alt বা Shift অন্য একটি কীর সঙ্গে ব্যবহার করুন। F1–F24 একাও ব্যবহার করা যায়।"
    };
}
