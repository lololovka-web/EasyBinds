namespace EasyBinds.Models;

public enum ActionType
{
    LaunchProgram,
    CloseActiveWindow,
    RunCommand,
    OpenWebsite,
    ShowMessage,
    VolumeMute
}

public class HotkeyBinding
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint Modifiers { get; set; }
    public uint Key { get; set; }
    public ActionType ActionType { get; set; }
    public string ActionParameter { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    public string HotkeyDisplay => GetHotkeyDisplayString(Modifiers, Key);

    public string ActionTypeDisplay => ActionType switch
    {
        ActionType.LaunchProgram => "Launch Program",
        ActionType.CloseActiveWindow => "Close Active Window",
        ActionType.RunCommand => "Run Command",
        ActionType.OpenWebsite => "Open Website",
        ActionType.ShowMessage => "Show Message",
        ActionType.VolumeMute => "Toggle Mute",
        _ => ActionType.ToString()
    };

    public static string GetHotkeyDisplayString(uint modifiers, uint key)
    {
        var parts = new List<string>();
        if ((modifiers & 0x0008) != 0) parts.Add("Win");
        if ((modifiers & 0x0002) != 0) parts.Add("Ctrl");
        if ((modifiers & 0x0004) != 0) parts.Add("Shift");
        if ((modifiers & 0x0001) != 0) parts.Add("Alt");

        parts.Add(VkCodeToString(key));
        return string.Join(" + ", parts);
    }

    private static string VkCodeToString(uint vk)
    {
        if (vk >= 0x41 && vk <= 0x5A) return ((char)vk).ToString();
        if (vk >= 0x30 && vk <= 0x39) return ((char)vk).ToString();
        if (vk >= 0x70 && vk <= 0x7B) return $"F{vk - 0x6F}";

        return vk switch
        {
            0x08 => "Backspace",
            0x09 => "Tab",
            0x0D => "Enter",
            0x1B => "Escape",
            0x20 => "Space",
            0x21 => "PageUp",
            0x22 => "PageDown",
            0x23 => "End",
            0x24 => "Home",
            0x25 => "Left",
            0x26 => "Up",
            0x27 => "Right",
            0x28 => "Down",
            0x2E => "Delete",
            0x2D => "Insert",
            0x6B => "Num+",
            0x6D => "Num-",
            0x6A => "Num*",
            0x6F => "Num/",
            0xBC => ",",
            0xBE => ".",
            0xBA => ";",
            0xBF => "/",
            0xDB => "[",
            0xDD => "]",
            0xDC => "\\",
            0xBD => "-",
            0xBB => "=",
            0xDE => "'",
            0xAD => "VolumeMute",
            0xAE => "VolumeDown",
            0xAF => "VolumeUp",
            0xB0 => "NextTrack",
            0xB1 => "PrevTrack",
            0xB3 => "MediaPlay",
            _ => $"VK({vk})"
        };
    }

    public static uint GetModifierFlags(System.Windows.Input.ModifierKeys mods)
    {
        uint flags = 0;
        if ((mods & System.Windows.Input.ModifierKeys.Control) != 0) flags |= 0x0002;
        if ((mods & System.Windows.Input.ModifierKeys.Shift) != 0) flags |= 0x0004;
        if ((mods & System.Windows.Input.ModifierKeys.Alt) != 0) flags |= 0x0001;
        if ((mods & System.Windows.Input.ModifierKeys.Windows) != 0) flags |= 0x0008;
        return flags;
    }

    public static uint GetVkCode(System.Windows.Input.Key key)
    {
        if (key >= System.Windows.Input.Key.A && key <= System.Windows.Input.Key.Z)
            return (uint)(key - System.Windows.Input.Key.A + 0x41);
        if (key >= System.Windows.Input.Key.D0 && key <= System.Windows.Input.Key.D9)
            return (uint)(key - System.Windows.Input.Key.D0 + 0x30);
        if (key >= System.Windows.Input.Key.F1 && key <= System.Windows.Input.Key.F24)
            return (uint)(key - System.Windows.Input.Key.F1 + 0x70);
        if (key >= System.Windows.Input.Key.NumPad0 && key <= System.Windows.Input.Key.NumPad9)
            return (uint)(key - System.Windows.Input.Key.NumPad0 + 0x60);

        return key switch
        {
            System.Windows.Input.Key.Back => 0x08,
            System.Windows.Input.Key.Tab => 0x09,
            System.Windows.Input.Key.Enter => 0x0D,
            System.Windows.Input.Key.Escape => 0x1B,
            System.Windows.Input.Key.Space => 0x20,
            System.Windows.Input.Key.PageUp => 0x21,
            System.Windows.Input.Key.PageDown => 0x22,
            System.Windows.Input.Key.End => 0x23,
            System.Windows.Input.Key.Home => 0x24,
            System.Windows.Input.Key.Left => 0x25,
            System.Windows.Input.Key.Up => 0x26,
            System.Windows.Input.Key.Right => 0x27,
            System.Windows.Input.Key.Down => 0x28,
            System.Windows.Input.Key.Delete => 0x2E,
            System.Windows.Input.Key.Insert => 0x2D,
            System.Windows.Input.Key.OemComma => 0xBC,
            System.Windows.Input.Key.OemPeriod => 0xBE,
            System.Windows.Input.Key.OemMinus => 0xBD,
            System.Windows.Input.Key.OemPlus => 0xBB,
            System.Windows.Input.Key.Oem1 => 0xBA,
            System.Windows.Input.Key.Oem2 => 0xBF,
            System.Windows.Input.Key.Oem3 => 0xC0,
            System.Windows.Input.Key.Oem4 => 0xDB,
            System.Windows.Input.Key.Oem5 => 0xDC,
            System.Windows.Input.Key.Oem6 => 0xDD,
            System.Windows.Input.Key.Oem7 => 0xDE,
            System.Windows.Input.Key.Multiply => 0x6A,
            System.Windows.Input.Key.Add => 0x6B,
            System.Windows.Input.Key.Subtract => 0x6D,
            System.Windows.Input.Key.Divide => 0x6F,
            System.Windows.Input.Key.Decimal => 0x6E,
            System.Windows.Input.Key.VolumeMute => 0xAD,
            System.Windows.Input.Key.VolumeDown => 0xAE,
            System.Windows.Input.Key.VolumeUp => 0xAF,
            System.Windows.Input.Key.MediaNextTrack => 0xB0,
            System.Windows.Input.Key.MediaPreviousTrack => 0xB1,
            System.Windows.Input.Key.MediaPlayPause => 0xB3,
            _ => (uint)key
        };
    }
}
