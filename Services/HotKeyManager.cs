using System.Linq;
using System.Runtime.InteropServices;
using EasyBinds.Models;

namespace EasyBinds.Services;

public class HotKeyManager : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public const int WM_HOTKEY = 0x0312;

    private readonly IntPtr _hWnd;
    private readonly Dictionary<int, HotkeyBinding> _bindings = new();
    private int _nextId = 1;

    public event Action<HotkeyBinding>? HotKeyPressed;

    public HotKeyManager(IntPtr hWnd)
    {
        _hWnd = hWnd;
    }

    public bool Register(HotkeyBinding binding)
    {
        if (!binding.IsEnabled) return false;

        int id = _nextId++;
        if (!RegisterHotKey(_hWnd, id, binding.Modifiers, binding.Key))
            return false;

        binding.Id = id;
        _bindings[id] = binding;
        return true;
    }

    public bool Unregister(HotkeyBinding binding)
    {
        if (_bindings.Remove(binding.Id, out _))
            return UnregisterHotKey(_hWnd, binding.Id);
        return false;
    }

    public void UnregisterAll()
    {
        foreach (var id in _bindings.Keys.ToList())
            UnregisterHotKey(_hWnd, id);
        _bindings.Clear();
    }

    public void ReregisterAll(IEnumerable<HotkeyBinding> bindings)
    {
        UnregisterAll();
        foreach (var b in bindings)
            Register(b);
    }

    public void ProcessHotKey(int id)
    {
        if (_bindings.TryGetValue(id, out var binding))
            HotKeyPressed?.Invoke(binding);
    }

    public void Dispose() => UnregisterAll();
}
