using System.Collections.Generic;

namespace FolderLens;

internal static class IndonesianPack
{
    public static IReadOnlyDictionary<string, string> Create() => new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["language.name"] = "Bahasa Indonesia",
        ["main.searchHint"] = "Cari folder…",
        ["main.resultsHeader"] = "FOLDER DITEMUKAN",
        ["main.folder.one"] = "{0} folder",
        ["main.folder.many"] = "{0} folder",
        ["main.previewHover"] = "Arahkan mouse ke folder",
        ["main.previewNoPhotos"] = "Tidak ada foto untuk ditampilkan",
        ["main.previewLoading"] = "Memuat foto…",
        ["main.previewChoose"] = "Pilih folder",
        ["main.previewPhotosHere"] = "Foto akan muncul di sini",
        ["main.noPhotosFolder"] = "Tidak ada foto di folder ini",
        ["main.emptyChooseTitle"] = "Pilih folder pencarian",
        ["main.emptyChooseMessage"] = "Tambahkan folder dari Pengaturan. FolderLens akan mencari di dalamnya tanpa menyentuh bagian PC lainnya.",
        ["main.emptyNoMatchTitle"] = "Folder tidak ditemukan",
        ["main.emptyNoMatchMessage"] = "Coba bagian lain dari nama atau lokasinya.",
        ["main.emptyTypeTitle"] = "Ketik untuk mencari",
        ["main.emptyTypeMessage"] = "Hasil akan muncul di bawah.",
        ["folder.configured"] = "Folder terkonfigurasi",
        ["tray.tooltip"] = "FolderLens · pencarian folder",
        ["tray.open"] = "Buka pencarian",
        ["tray.refresh"] = "Perbarui indeks",
        ["tray.settings"] = "Pengaturan",
        ["tray.exit"] = "Keluar",
        ["settings.title"] = "Pengaturan · FolderLens",
        ["settings.eyebrow"] = "PENGATURAN",
        ["settings.heading"] = "Di mana Anda ingin mencari?",
        ["settings.description"] = "FolderLens hanya akan memeriksa folder ini dan semua isinya.",
        ["settings.addFolder"] = "＋  Tambah folder",
        ["settings.removeFolder"] = "－  Hapus",
        ["settings.hotkeyTitle"] = "Pintasan untuk membuka",
        ["settings.hotkey.initialHelp"] = "Klik pintasan lalu tekan kombinasi dengan Ctrl, Alt, atau Shift.",
        ["settings.hotkey.capturePrompt"] = "Tekan tombol…",
        ["settings.hotkey.captureHelp"] = "Tahan Ctrl, Alt, atau Shift lalu tekan tombol lain. Esc membatalkan.",
        ["settings.hotkey.applied"] = "{0} akan diterapkan saat disimpan.",
        ["settings.startup"] = "Buka FolderLens saat Windows dimulai",
        ["settings.footer"] = "Pencarian tersedia dari ikon baki sistem.",
        ["settings.cancel"] = "Batal",
        ["settings.save"] = "Simpan perubahan",
        ["settings.languageDetected"] = "Bahasa: {0} · terdeteksi otomatis",
        ["dialog.folderDescription"] = "Pilih folder untuk dicari",
        ["hotkey.windowsReserved"] = "Gunakan Ctrl, Alt, atau Shift; tombol Windows dicadangkan.",
        ["hotkey.modifierOnly"] = "Bagus. Tetap tahan lalu tekan tombol lain.",
        ["hotkey.invalid"] = "Tombol itu tidak dapat digunakan. Coba kombinasi lain.",
        ["hotkey.noModifier"] = "Gunakan Ctrl, Alt, atau Shift dengan tombol lain. F1–F24 juga dapat digunakan sendiri."
    };
}
