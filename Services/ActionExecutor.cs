using System.Diagnostics;
using System.Runtime.InteropServices;
using EasyBinds.Models;

namespace EasyBinds.Services;

public class ActionExecutor
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetShellWindow();

    private const uint WM_CLOSE = 0x0010;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const int SW_MAXIMIZE = 3;
    private const int SW_MINIMIZE = 6;
    private const int SW_RESTORE = 9;
    private const int SW_SHOWMINNOACTIVE = 7;

    public void Execute(HotkeyBinding binding)
    {
        switch (binding.ActionType)
        {
            case ActionType.LaunchProgram:
                LaunchProgram(binding.ActionParameter);
                break;
            case ActionType.CloseActiveWindow:
                CloseActiveWindow();
                break;
            case ActionType.RunCommand:
                RunCommand(binding.ActionParameter);
                break;
            case ActionType.OpenWebsite:
                OpenWebsite(binding.ActionParameter);
                break;
            case ActionType.ShowMessage:
                ShowMessage(binding.ActionParameter);
                break;
            case ActionType.VolumeMute:
                ToggleMute();
                break;
            case ActionType.MaximizeWindow:
                MaximizeWindow();
                break;
            case ActionType.MinimizeWindow:
                MinimizeWindow();
                break;
            case ActionType.ShowDesktop:
                ShowDesktop();
                break;
            case ActionType.VolumeUp:
                VolumeUp();
                break;
            case ActionType.VolumeDown:
                VolumeDown();
                break;
        }
    }

    private static void LaunchProgram(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                string.Format(L10n._("launch_error"), path, ex.Message),
                L10n._("easybinds_error"),
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private static void CloseActiveWindow()
    {
        IntPtr hWnd = GetForegroundWindow();
        if (hWnd != IntPtr.Zero)
        {
            GetWindowThreadProcessId(hWnd, out uint pid);
            if (pid != Environment.ProcessId)
                PostMessage(hWnd, WM_CLOSE, 0, 0);
        }
    }

    private static void RunCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
        catch { }
    }

    private static void OpenWebsite(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        try
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }

    private static void ShowMessage(string text)
    {
        System.Windows.MessageBox.Show(text, "EasyBinds",
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private static void ToggleMute()
    {
        keybd_event(0xAD, 0, 0, UIntPtr.Zero);
        keybd_event(0xAD, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private static void MaximizeWindow()
    {
        IntPtr hWnd = GetForegroundWindow();
        if (hWnd != IntPtr.Zero && hWnd != GetShellWindow())
            ShowWindow(hWnd, SW_MAXIMIZE);
    }

    private static void MinimizeWindow()
    {
        IntPtr hWnd = GetForegroundWindow();
        if (hWnd != IntPtr.Zero && hWnd != GetShellWindow())
            ShowWindow(hWnd, SW_MINIMIZE);
    }

    private static void ShowDesktop()
    {
        keybd_event(0x5B, 0, 0, UIntPtr.Zero);
        keybd_event(0x44, 0, 0, UIntPtr.Zero);
        keybd_event(0x44, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        keybd_event(0x5B, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private static void VolumeUp()
    {
        keybd_event(0xAF, 0, 0, UIntPtr.Zero);
        keybd_event(0xAF, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    private static void VolumeDown()
    {
        keybd_event(0xAE, 0, 0, UIntPtr.Zero);
        keybd_event(0xAE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }
}
