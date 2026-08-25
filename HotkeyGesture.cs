using System.Windows.Input;

namespace FolderLens;

public readonly record struct HotkeyGesture(int VirtualKey, HotkeyModifiers Modifiers)
{
    public static HotkeyGesture Default => new(0x20, HotkeyModifiers.Alt);

    public string DisplayText => Format(VirtualKey, Modifiers);

    public static HotkeyGesture FromSettings(AppSettings settings)
    {
        var gesture = new HotkeyGesture(settings.OpenHotkeyVirtualKey, settings.OpenHotkeyModifiers);
        return IsValid(gesture.VirtualKey, gesture.Modifiers) ? gesture : Default;
    }

    public static bool TryCreate(Key key, ModifierKeys keyboardModifiers, out HotkeyGesture gesture, out string message)
    {
        gesture = Default;
        message = string.Empty;

        if ((keyboardModifiers & ModifierKeys.Windows) != 0 || key is Key.LWin or Key.RWin)
        {
            message = Localization.Get("hotkey.windowsReserved");
            return false;
        }

        if (IsModifierKey(key))
        {
            message = Localization.Get("hotkey.modifierOnly");
            return false;
        }

        var modifiers = HotkeyModifiers.None;
        if ((keyboardModifiers & ModifierKeys.Control) != 0) modifiers |= HotkeyModifiers.Control;
        if ((keyboardModifiers & ModifierKeys.Alt) != 0) modifiers |= HotkeyModifiers.Alt;
        if ((keyboardModifiers & ModifierKeys.Shift) != 0) modifiers |= HotkeyModifiers.Shift;

        var virtualKey = KeyInterop.VirtualKeyFromKey(key);
        if (!IsValid(virtualKey, modifiers))
        {
            message = modifiers == HotkeyModifiers.None
                ? Localization.Get("hotkey.noModifier")
                : Localization.Get("hotkey.invalid");
            return false;
        }

        gesture = new HotkeyGesture(virtualKey, modifiers);
        return true;
    }

    public static bool IsModifierKey(Key key) => key is
        Key.LeftCtrl or Key.RightCtrl or
        Key.LeftAlt or Key.RightAlt or
        Key.LeftShift or Key.RightShift;

    public static bool IsValid(int virtualKey, HotkeyModifiers modifiers)
    {
        const HotkeyModifiers supported = HotkeyModifiers.Control | HotkeyModifiers.Alt | HotkeyModifiers.Shift;
        if (virtualKey is <= 0 or > 0xFF || (modifiers & ~supported) != 0) return false;

        var key = KeyInterop.KeyFromVirtualKey(virtualKey);
        if (key is Key.None or Key.Escape or Key.Tab or Key.LWin or Key.RWin || IsModifierKey(key)) return false;

        var isFunctionKey = virtualKey is >= 0x70 and <= 0x87;
        return modifiers != HotkeyModifiers.None || isFunctionKey;
    }

    public static string Format(int virtualKey, HotkeyModifiers modifiers)
    {
        if (!IsValid(virtualKey, modifiers)) return Default.DisplayText;

        var parts = new List<string>(4);
        if ((modifiers & HotkeyModifiers.Control) != 0) parts.Add("Ctrl");
        if ((modifiers & HotkeyModifiers.Alt) != 0) parts.Add("Alt");
        if ((modifiers & HotkeyModifiers.Shift) != 0) parts.Add("Shift");
        parts.Add(FormatKey(KeyInterop.KeyFromVirtualKey(virtualKey)));
        return string.Join(" + ", parts);
    }

    private static string FormatKey(Key key) => key switch
    {
        Key.Space => "Espacio",
        Key.Return => "Enter",
        Key.Back => "Retroceso",
        Key.Delete => "Supr",
        Key.Insert => "Insert",
        Key.Home => "Inicio",
        Key.End => "Fin",
        Key.Prior => "Re Pág",
        Key.Next => "Av Pág",
        Key.Left => "←",
        Key.Right => "→",
        Key.Up => "↑",
        Key.Down => "↓",
        Key.OemPlus => "+",
        Key.OemMinus => "−",
        Key.OemComma => ",",
        Key.OemPeriod => ".",
        >= Key.D0 and <= Key.D9 => ((int)key - (int)Key.D0).ToString(),
        >= Key.NumPad0 and <= Key.NumPad9 => $"Num {(int)key - (int)Key.NumPad0}",
        _ => key.ToString()
    };
}
