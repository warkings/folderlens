using System.Collections.Generic;

namespace FolderLens;

internal static class HindiPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "हिन्दी",
        ["main.searchHint"] = "फ़ोल्डर खोजें…",
        ["main.resultsHeader"] = "मिले हुए फ़ोल्डर",
        ["main.folder.one"] = "{0} फ़ोल्डर",
        ["main.folder.many"] = "{0} फ़ोल्डर",
        ["main.previewHover"] = "किसी फ़ोल्डर पर माउस ले जाएँ",
        ["main.previewNoPhotos"] = "दिखाने के लिए कोई फ़ोटो नहीं",
        ["main.previewLoading"] = "फ़ोटो लोड हो रही हैं…",
        ["main.previewChoose"] = "कोई फ़ोल्डर चुनें",
        ["main.previewPhotosHere"] = "फ़ोटो यहाँ दिखाई देंगी",
        ["main.noPhotosFolder"] = "इस फ़ोल्डर में कोई फ़ोटो नहीं है",
        ["main.emptyChooseTitle"] = "अपने खोज फ़ोल्डर चुनें",
        ["main.emptyChooseMessage"] = "सेटिंग्स से कोई फ़ोल्डर जोड़ें। FolderLens आपके PC के बाकी हिस्से को छुए बिना उसमें खोज करेगा।",
        ["main.emptyNoMatchTitle"] = "वह फ़ोल्डर नहीं मिला",
        ["main.emptyNoMatchMessage"] = "नाम या स्थान का कोई दूसरा हिस्सा आज़माएँ।",
        ["main.emptyTypeTitle"] = "खोजने के लिए लिखें",
        ["main.emptyTypeMessage"] = "नतीजे नीचे दिखाई देंगे।",
        ["folder.configured"] = "कॉन्फ़िगर किया गया फ़ोल्डर",
        ["tray.tooltip"] = "FolderLens · फ़ोल्डर खोज",
        ["tray.open"] = "खोज खोलें",
        ["tray.refresh"] = "इंडेक्स रीफ़्रेश करें",
        ["tray.settings"] = "सेटिंग्स",
        ["tray.exit"] = "बाहर निकलें",
        ["settings.title"] = "सेटिंग्स · FolderLens",
        ["settings.eyebrow"] = "सेटिंग्स",
        ["settings.heading"] = "आप कहाँ खोजना चाहते हैं?",
        ["settings.description"] = "FolderLens केवल इन फ़ोल्डरों और इनके अंदर की चीज़ों को जाँचेगा।",
        ["settings.addFolder"] = "＋  फ़ोल्डर जोड़ें",
        ["settings.removeFolder"] = "－  हटाएँ",
        ["settings.hotkeyTitle"] = "खोलने का शॉर्टकट",
        ["settings.hotkey.initialHelp"] = "शॉर्टकट पर क्लिक करें और Ctrl, Alt या Shift के साथ कोई संयोजन दबाएँ।",
        ["settings.hotkey.capturePrompt"] = "कुंजियाँ दबाएँ…",
        ["settings.hotkey.captureHelp"] = "Ctrl, Alt या Shift दबाकर रखें और दूसरी कुंजी दबाएँ। Esc रद्द करता है।",
        ["settings.hotkey.applied"] = "सहेजने पर {0} लागू होगा।",
        ["settings.startup"] = "Windows शुरू होने पर FolderLens खोलें",
        ["settings.footer"] = "सर्चर सिस्टम ट्रे आइकन से उपलब्ध रहेगा।",
        ["settings.cancel"] = "रद्द करें",
        ["settings.save"] = "बदलाव सहेजें",
        ["settings.languageDetected"] = "भाषा: {0} · अपने आप पहचानी गई",
        ["dialog.folderDescription"] = "खोजने के लिए फ़ोल्डर चुनें",
        ["hotkey.windowsReserved"] = "Ctrl, Alt या Shift का उपयोग करें; Windows कुंजी आरक्षित है।",
        ["hotkey.modifierOnly"] = "अच्छा। इसे दबाए रखें और दूसरी कुंजी दबाएँ।",
        ["hotkey.invalid"] = "इस कुंजी का उपयोग नहीं किया जा सकता। कोई दूसरा संयोजन आज़माएँ।",
        ["hotkey.noModifier"] = "Ctrl, Alt या Shift को किसी दूसरी कुंजी के साथ उपयोग करें। F1–F24 अकेले भी चल सकते हैं।"
    };
}
