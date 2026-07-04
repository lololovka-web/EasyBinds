using System.IO;
using System.Text.Json;
using EasyBinds.Models;

namespace EasyBinds.Services;

public class SettingsService
{
    private readonly string _filePath;

    public SettingsService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = System.IO.Path.Combine(appData, "EasyBinds");
        Directory.CreateDirectory(dir);
        _filePath = System.IO.Path.Combine(dir, "settings.json");
    }

    public List<HotkeyBinding> Load()
    {
        try
        {
            var data = LoadFull();
            return data.Bindings;
        }
        catch
        {
            return new List<HotkeyBinding>();
        }
    }

    public (string theme, string lang) LoadSettings()
    {
        try
        {
            var data = LoadFull();
            return (data.Theme, data.Language);
        }
        catch
        {
            return ("dark", "en");
        }
    }

    public void Save(List<HotkeyBinding> bindings)
    {
        try
        {
            var data = LoadFull();
            data.Bindings = bindings;
            WriteFull(data);
        }
        catch { }
    }

    public void SaveTheme(string theme)
    {
        try
        {
            var data = LoadFull();
            data.Theme = theme;
            WriteFull(data);
        }
        catch { }
    }

    public void SaveLanguage(string lang)
    {
        try
        {
            var data = LoadFull();
            data.Language = lang;
            WriteFull(data);
        }
        catch { }
    }

    private AppData LoadFull()
    {
        if (!File.Exists(_filePath))
            return new AppData();

        try
        {
            var json = File.ReadAllText(_filePath);

            // Try new format first
            try
            {
                return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
            catch (JsonException)
            {
                // Fallback: old format (array of bindings)
                var bindings = JsonSerializer.Deserialize<List<HotkeyBinding>>(json);
                return new AppData { Bindings = bindings ?? new List<HotkeyBinding>() };
            }
        }
        catch
        {
            return new AppData();
        }
    }

    private void WriteFull(AppData data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(_filePath, json);
    }

    private class AppData
    {
        public string Theme { get; set; } = "dark";
        public string Language { get; set; } = "en";
        public List<HotkeyBinding> Bindings { get; set; } = new();
    }
}
