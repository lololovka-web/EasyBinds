using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using EasyBinds.Models;
using EasyBinds.Services;

namespace EasyBinds;

public partial class AddBindingWindow : Window
{
    private ModifierKeys _selectedModifiers;
    private Key _selectedKey;
    private bool _hasKey;

    public HotkeyBinding? Binding { get; private set; }

    public AddBindingWindow() : this(null) { }

    public AddBindingWindow(HotkeyBinding? existing)
    {
        InitializeComponent();

        ApplyLocalization();
        BrowseButton.Visibility = Visibility.Collapsed;

        if (existing != null)
            LoadExisting(existing);
    }

    private void ApplyLocalization()
    {
        Title = L10n._("add_binding_title");
        PressKeysLabel.Text = L10n._("press_keys");
        HotkeyBox.Text = "(" + L10n._("press_keys").TrimEnd(':') + ")";
        ActionTypeLabel.Text = L10n._("action_type");
        BrowseButton.Content = L10n._("choose");
        OkBtn.Content = L10n._("ok");
        CancelBtn.Content = L10n._("cancel");
        ParamLabel.Text = L10n._("file_label");

        ActionTypeBox.Items.Clear();
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "LaunchProgram", Content = L10n._("launch_program") });
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "CloseActiveWindow", Content = L10n._("close_window") });
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "RunCommand", Content = L10n._("run_command") });
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "OpenWebsite", Content = L10n._("open_website") });
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "ShowMessage", Content = L10n._("show_message") });
        ActionTypeBox.Items.Add(new System.Windows.Controls.ComboBoxItem
            { Tag = "VolumeMute", Content = L10n._("toggle_mute") });
        ActionTypeBox.SelectedIndex = 0;
    }

    private void LoadExisting(HotkeyBinding existing)
    {
        Title = L10n._("edit_binding_title");
        var (mods, key) = Win32ToWpf(existing.Modifiers, existing.Key);
        _hasKey = true;
        _selectedModifiers = mods;
        _selectedKey = key;
        HotkeyBox.Text = HotkeyBinding.GetHotkeyDisplayString(existing.Modifiers, existing.Key);

        for (int i = 0; i < ActionTypeBox.Items.Count; i++)
        {
            if (ActionTypeBox.Items[i] is System.Windows.Controls.ComboBoxItem item &&
                item.Tag?.ToString() == existing.ActionType.ToString())
            {
                ActionTypeBox.SelectedIndex = i;
                break;
            }
        }

        ParamBox.Text = existing.ActionParameter;
        UpdateParamUI(existing.ActionType);
    }

    private static (ModifierKeys, Key) Win32ToWpf(uint modifiers, uint vk)
    {
        var mods = ModifierKeys.None;
        if ((modifiers & 0x0001) != 0) mods |= ModifierKeys.Alt;
        if ((modifiers & 0x0002) != 0) mods |= ModifierKeys.Control;
        if ((modifiers & 0x0004) != 0) mods |= ModifierKeys.Shift;
        if ((modifiers & 0x0008) != 0) mods |= ModifierKeys.Windows;

        if (vk >= 0x41 && vk <= 0x5A) return (mods, Key.A + (int)(vk - 0x41));
        if (vk >= 0x30 && vk <= 0x39) return (mods, Key.D0 + (int)(vk - 0x30));
        if (vk >= 0x70 && vk <= 0x7B) return (mods, Key.F1 + (int)(vk - 0x70));

        var key = vk switch
        {
            0x08 => Key.Back,
            0x09 => Key.Tab,
            0x0D => Key.Enter,
            0x1B => Key.Escape,
            0x20 => Key.Space,
            0x21 => Key.PageUp,
            0x22 => Key.PageDown,
            0x23 => Key.End,
            0x24 => Key.Home,
            0x25 => Key.Left,
            0x26 => Key.Up,
            0x27 => Key.Right,
            0x28 => Key.Down,
            0x2E => Key.Delete,
            0x2D => Key.Insert,
            0xBC => Key.OemComma,
            0xBE => Key.OemPeriod,
            0xBD => Key.OemMinus,
            0xBB => Key.OemPlus,
            0xBA => Key.Oem1,
            0xBF => Key.Oem2,
            0xC0 => Key.Oem3,
            0xDB => Key.Oem4,
            0xDC => Key.Oem5,
            0xDD => Key.Oem6,
            0xDE => Key.Oem7,
            0x6A => Key.Multiply,
            0x6B => Key.Add,
            0x6D => Key.Subtract,
            0x6F => Key.Divide,
            _ => Key.None
        };
        return (mods, key);
    }

    private void HotkeyBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        e.Handled = true;

        var mods = ModifierKeys.None;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            mods |= ModifierKeys.Control;
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            mods |= ModifierKeys.Shift;
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            mods |= ModifierKeys.Alt;
        if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            mods |= ModifierKeys.Windows;

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (key == Key.LeftCtrl || key == Key.RightCtrl ||
            key == Key.LeftShift || key == Key.RightShift ||
            key == Key.LeftAlt || key == Key.RightAlt ||
            key == Key.LWin || key == Key.RWin)
        {
            var vk = HotkeyBinding.GetVkCode(key);
            var modFlags = HotkeyBinding.GetModifierFlags(mods);
            HotkeyBox.Text = HotkeyBinding.GetHotkeyDisplayString(modFlags, vk);
            _hasKey = false;
            return;
        }

        _selectedModifiers = mods;
        _selectedKey = key;
        _hasKey = true;

        var modFlags2 = HotkeyBinding.GetModifierFlags(mods);
        var vkCode = HotkeyBinding.GetVkCode(key);
        HotkeyBox.Text = HotkeyBinding.GetHotkeyDisplayString(modFlags2, vkCode);
    }

    private void HotkeyBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (!_hasKey)
            HotkeyBox.Text = "";
        HotkeyBox.Background = System.Windows.Media.Brushes.Transparent;
    }

    private void HotkeyBox_LostFocus(object sender, RoutedEventArgs e)
    {
        HotkeyBox.Background = System.Windows.Media.Brushes.Transparent;
        if (!_hasKey)
            HotkeyBox.Text = "(" + L10n._("press_keys").TrimEnd(':') + ")";
    }

    private void ActionTypeBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var action = GetSelectedActionType();
        UpdateParamUI(action);
    }

    private ActionType GetSelectedActionType()
    {
        if (ActionTypeBox.SelectedItem is System.Windows.Controls.ComboBoxItem item &&
            Enum.TryParse<ActionType>(item.Tag?.ToString(), out var result))
            return result;
        return ActionType.LaunchProgram;
    }

    private void UpdateParamUI(ActionType action)
    {
        switch (action)
        {
            case ActionType.LaunchProgram:
                ParamLabel.Text = L10n._("file_label");
                ParamBox.Visibility = Visibility.Collapsed;
                BrowseButton.Visibility = Visibility.Visible;
                break;
            case ActionType.CloseActiveWindow:
            case ActionType.VolumeMute:
                ParamLabel.Text = L10n._("no_param");
                ParamBox.Visibility = Visibility.Collapsed;
                BrowseButton.Visibility = Visibility.Collapsed;
                break;
            case ActionType.RunCommand:
                ParamLabel.Text = L10n._("command");
                ParamBox.Visibility = Visibility.Visible;
                BrowseButton.Visibility = Visibility.Collapsed;
                break;
            case ActionType.OpenWebsite:
                ParamLabel.Text = L10n._("website_url");
                ParamBox.Visibility = Visibility.Visible;
                BrowseButton.Visibility = Visibility.Collapsed;
                break;
            case ActionType.ShowMessage:
                ParamLabel.Text = L10n._("message_text");
                ParamBox.Visibility = Visibility.Visible;
                BrowseButton.Visibility = Visibility.Collapsed;
                break;
        }
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new System.Windows.Forms.OpenFileDialog
        {
            Title = L10n._("select_program"),
            Filter = L10n._("exec_filter")
        };
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            ParamBox.Text = dlg.FileName;
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        if (!_hasKey)
        {
            MessageBox.Show(L10n._("err_no_keys"), "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var action = GetSelectedActionType();
        if (NeedsParameter(action) && string.IsNullOrWhiteSpace(ParamBox.Text))
        {
            MessageBox.Show(L10n._("err_no_param"), "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var modifiers = HotkeyBinding.GetModifierFlags(_selectedModifiers);
        var key = HotkeyBinding.GetVkCode(_selectedKey);

        Binding = new HotkeyBinding
        {
            Modifiers = modifiers,
            Key = key,
            ActionType = action,
            ActionParameter = ParamBox.Text.Trim(),
            IsEnabled = true
        };

        DialogResult = true;
        Close();
    }

    private static bool NeedsParameter(ActionType action)
    {
        return action switch
        {
            ActionType.CloseActiveWindow => false,
            ActionType.VolumeMute => false,
            _ => true
        };
    }
}
