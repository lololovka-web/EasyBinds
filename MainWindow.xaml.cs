using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using EasyBinds.Models;
using EasyBinds.Services;
using Microsoft.Win32;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;

namespace EasyBinds;

public partial class MainWindow : Window
{
    private readonly SettingsService _settingsService = new();
    private readonly ActionExecutor _actionExecutor = new();
    private HotKeyManager? _hotKeyManager;
    private NotifyIconWrapper? _trayIcon;
    private List<HotkeyBinding> _bindings = new();
    private string _currentTheme = "dark";

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            _hotKeyManager = new HotKeyManager(hwnd);
            _hotKeyManager.HotKeyPressed += OnHotKeyPressed;

            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);

            var (theme, lang) = _settingsService.LoadSettings();
            _currentTheme = theme;
            L10n.Lang = lang;
            UpdateLanguageUI();
            UpdateThemeUI();
            App.ApplyLanguage(lang);
            App.ApplyTheme(theme);

            LoadBindings();
            ApplyLanguageToWindow();
            InitTrayIcon();
            UpdateStartupCheckbox();

            if (App.StartMinimized)
            {
                WindowState = WindowState.Minimized;
                Hide();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Init error:\n" + ex, "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == HotKeyManager.WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            _hotKeyManager?.ProcessHotKey(id);
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void OnHotKeyPressed(HotkeyBinding binding)
    {
        Dispatcher.Invoke(() =>
        {
            _actionExecutor.Execute(binding);
            StatusText.Text = string.Format(L10n._("executed"),
                binding.HotkeyDisplay, binding.ActionTypeDisplay);
        });
    }

    private void InitTrayIcon()
    {
        try
        {
            _trayIcon = new NotifyIconWrapper();
            _trayIcon.ShowClicked += () => Dispatcher.Invoke(ShowWindow);
            _trayIcon.ExitClicked += () => Dispatcher.Invoke(() =>
            {
                _trayIcon?.Dispose();
                Application.Current.Shutdown();
            });
            _trayIcon.Show();
        }
        catch { }
    }

    private void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
            Hide();
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        WindowState = WindowState.Minimized;
        Hide();
    }

    private void LoadBindings()
    {
        _bindings = _settingsService.Load();
        RefreshList();
        _hotKeyManager?.ReregisterAll(_bindings);
        StatusText.Text = string.Format(L10n._("loaded_bindings"), _bindings.Count);
    }

    private void RefreshList()
    {
        BindingsList.ItemsSource = null;
        BindingsList.ItemsSource = _bindings;
        EmptyText.Visibility = _bindings.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SaveBindings()
    {
        _settingsService.Save(_bindings);
        _hotKeyManager?.ReregisterAll(_bindings);
    }

    private HotkeyBinding? GetSelectedBinding()
    {
        return BindingsList.SelectedItem as HotkeyBinding;
    }

    private void AddBinding_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new AddBindingWindow
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true && dialog.Binding != null)
            {
                _bindings.Add(dialog.Binding);
                SaveBindings();
                RefreshList();
                StatusText.Text = string.Format(L10n._("added"), dialog.Binding.HotkeyDisplay);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditBinding_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var binding = GetSelectedBinding();
            if (binding == null) { StatusText.Text = L10n._("select_to_edit"); return; }

            var dialog = new AddBindingWindow(binding)
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true && dialog.Binding != null)
            {
                var idx = _bindings.IndexOf(binding);
                _bindings[idx] = dialog.Binding;
                SaveBindings();
                RefreshList();
                StatusText.Text = string.Format(L10n._("edited"), dialog.Binding.HotkeyDisplay);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteBinding_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var binding = GetSelectedBinding();
            if (binding == null) { StatusText.Text = L10n._("select_to_delete"); return; }

            var result = MessageBox.Show(
                string.Format(L10n._("confirm_delete"), binding.HotkeyDisplay),
                L10n._("confirm"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _bindings.Remove(binding);
                SaveBindings();
                RefreshList();
                StatusText.Text = L10n._("deleted");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "EasyBinds",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BindingsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        EditBinding_Click(sender, e);
    }

    private void EnableAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var b in _bindings) b.IsEnabled = true;
        SaveBindings();
        RefreshList();
        StatusText.Text = L10n._("all_enabled");
    }

    private void DisableAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var b in _bindings) b.IsEnabled = false;
        SaveBindings();
        RefreshList();
        StatusText.Text = L10n._("all_disabled");
    }

    private void StartWithWindows_Changed(object sender, RoutedEventArgs e)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null) return;

            if (StartWithWindowsCheck.IsChecked == true)
                key.SetValue("EasyBinds", $"\"{Environment.ProcessPath}\" --minimized");
            else
                key.DeleteValue("EasyBinds", false);
        }
        catch { }
    }

    private void UpdateStartupCheckbox()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run");
            var val = key?.GetValue("EasyBinds");
            StartWithWindowsCheck.IsChecked = val != null;
        }
        catch { }
    }

    private void AdminBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var exePath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exePath)) return;

            StatusText.Text = L10n._("admin_restart");
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Verb = "runas",
                UseShellExecute = true
            };
            Process.Start(psi);
            _trayIcon?.Dispose();
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "EasyBinds", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void WhyAdminBtn_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(L10n._("admin_msg"), L10n._("admin_title"),
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _currentTheme = _currentTheme == "dark" ? "light" : "dark";
            App.ApplyTheme(_currentTheme);
            _settingsService.SaveTheme(_currentTheme);
            UpdateThemeUI();
        }
        catch { }
    }

    private void UpdateThemeUI()
    {
        ThemeToggle.Content = _currentTheme == "dark" ? "☀" : "☾";
    }

    private void LangToggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            L10n.Lang = L10n.Lang == "en" ? "ru" : "en";
            _settingsService.SaveLanguage(L10n.Lang);
            UpdateLanguageUI();
            ApplyLanguageToWindow();
        }
        catch { }
    }

    private void UpdateLanguageUI()
    {
        LangToggle.Content = L10n.Lang == "en" ? "EN" : "RU";
    }

    private void ApplyLanguageToWindow()
    {
        Title = L10n._("app_title");
        TitleBlock.Text = L10n._("app_title");
        SubtitleText.Text = $"v1.0 | {_bindings.Count} {L10n._("ready").ToLower()}";
        AddBtn.Content = L10n._("add_binding");
        EditBtn.Content = L10n._("edit");
        DelBtn.Content = L10n._("delete");
        EnableAllBtn.Content = L10n._("enable_all");
        DisableAllBtn.Content = L10n._("disable_all");
        StartWithWindowsCheck.Content = L10n._("run_at_startup");
        StatusText.Text = L10n._("ready");
        EmptyText.Text = string.Format(L10n._("empty_list"), L10n._("add_binding"));
        AdminBtn.ToolTip = L10n._("request_admin");
        WhyAdminBtn.ToolTip = L10n._("why_admin");
        AdminBtn.Content = "\u2B06";
        WhyAdminBtn.Content = "?";

        var gridView = BindingsList.View as System.Windows.Controls.GridView;
        if (gridView != null && gridView.Columns.Count >= 4)
        {
            gridView.Columns[0].Header = L10n._("enabled");
            gridView.Columns[1].Header = L10n._("hotkey");
            gridView.Columns[2].Header = L10n._("action");
            gridView.Columns[3].Header = L10n._("parameter");
        }
    }
}

internal class NotifyIconWrapper : IDisposable
{
    private readonly Forms.NotifyIcon _icon;

    public event Action? ShowClicked;
    public event Action? ExitClicked;

    public NotifyIconWrapper()
    {
        _icon = new Forms.NotifyIcon
        {
            Text = L10n._("tray_text"),
            Visible = false
        };

        try
        {
            var bmp = new Drawing.Bitmap(16, 16);
            using var g = Drawing.Graphics.FromImage(bmp);
            g.Clear(Drawing.Color.Transparent);
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new Drawing.SolidBrush(Drawing.Color.FromArgb(14, 165, 233));
            g.FillRectangle(brush, 1, 1, 14, 14);
            g.DrawString("EB", new Drawing.Font("Segoe UI", 7, Drawing.FontStyle.Bold),
                Drawing.Brushes.White, 2, 2);
            _icon.Icon = Drawing.Icon.FromHandle(bmp.GetHicon());
        }
        catch { }

        _icon.DoubleClick += (_, _) => ShowClicked?.Invoke();

        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add(L10n._("tray_show"), null, (_, _) => ShowClicked?.Invoke());
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add(L10n._("tray_exit"), null, (_, _) => ExitClicked?.Invoke());
        _icon.ContextMenuStrip = menu;
    }

    public void Show()
    {
        _icon.Visible = true;
    }

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
    }
}
