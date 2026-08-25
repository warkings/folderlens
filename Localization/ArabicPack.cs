using System.Collections.Generic;

namespace FolderLens;

internal static class ArabicPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "العربية",
        ["main.searchHint"] = "ابحث عن مجلد…",
        ["main.resultsHeader"] = "المجلدات التي تم العثور عليها",
        ["main.folder.one"] = "{0} مجلد",
        ["main.folder.many"] = "{0} مجلدات",
        ["main.previewHover"] = "مرّر المؤشر فوق مجلد",
        ["main.previewNoPhotos"] = "لا توجد صور لعرضها",
        ["main.previewLoading"] = "جارٍ تحميل الصور…",
        ["main.previewChoose"] = "اختر مجلدًا",
        ["main.previewPhotosHere"] = "ستظهر الصور هنا",
        ["main.noPhotosFolder"] = "لا توجد صور في هذا المجلد",
        ["main.emptyChooseTitle"] = "اختر مجلدات البحث",
        ["main.emptyChooseMessage"] = "أضف مجلدًا من الإعدادات. سيبحث FolderLens بداخله من دون لمس بقية الكمبيوتر.",
        ["main.emptyNoMatchTitle"] = "لم نعثر على هذا المجلد",
        ["main.emptyNoMatchMessage"] = "جرّب جزءًا آخر من الاسم أو الموقع.",
        ["main.emptyTypeTitle"] = "اكتب للبحث",
        ["main.emptyTypeMessage"] = "ستظهر النتائج أدناه.",
        ["folder.configured"] = "مجلد مُعدّ",
        ["tray.tooltip"] = "FolderLens · البحث عن المجلدات",
        ["tray.open"] = "فتح البحث",
        ["tray.refresh"] = "تحديث الفهرس",
        ["tray.settings"] = "الإعدادات",
        ["tray.exit"] = "خروج",
        ["settings.title"] = "الإعدادات · FolderLens",
        ["settings.eyebrow"] = "الإعدادات",
        ["settings.heading"] = "أين تريد البحث؟",
        ["settings.description"] = "سيفحص FolderLens هذه المجلدات وكل ما بداخلها فقط.",
        ["settings.addFolder"] = "＋  إضافة مجلد",
        ["settings.removeFolder"] = "－  إزالة",
        ["settings.hotkeyTitle"] = "اختصار الفتح",
        ["settings.hotkey.initialHelp"] = "انقر على الاختصار واضغط تركيبة تحتوي على Ctrl أو Alt أو Shift.",
        ["settings.hotkey.capturePrompt"] = "اضغط المفاتيح…",
        ["settings.hotkey.captureHelp"] = "اضغط باستمرار على Ctrl أو Alt أو Shift ثم اضغط مفتاحًا آخر. يلغي Esc العملية.",
        ["settings.hotkey.applied"] = "سيتم تطبيق {0} عند الحفظ.",
        ["settings.startup"] = "فتح FolderLens عند بدء تشغيل Windows",
        ["settings.footer"] = "يبقى البحث متاحًا من أيقونة شريط النظام.",
        ["settings.cancel"] = "إلغاء",
        ["settings.save"] = "حفظ التغييرات",
        ["settings.languageDetected"] = "اللغة: {0} · تم اكتشافها تلقائيًا",
        ["dialog.folderDescription"] = "اختر مجلدًا للبحث",
        ["hotkey.windowsReserved"] = "استخدم Ctrl أو Alt أو Shift؛ مفتاح Windows محجوز.",
        ["hotkey.modifierOnly"] = "جيد. أبقِه مضغوطًا واضغط مفتاحًا آخر.",
        ["hotkey.invalid"] = "لا يمكن استخدام هذا المفتاح. جرّب تركيبة أخرى.",
        ["hotkey.noModifier"] = "استخدم Ctrl أو Alt أو Shift مع مفتاح آخر. يمكن استخدام F1–F24 منفردة أيضًا."
    };
}
