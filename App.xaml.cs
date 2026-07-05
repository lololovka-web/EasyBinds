using System.Windows;
using System.Windows.Media;
using EasyBinds.Services;

namespace EasyBinds;

public partial class App : System.Windows.Application
{
    private static readonly Dictionary<string, (byte r, byte g, byte b)> DarkColors = new()
    {
        ["Bg"] = (0x0d, 0x11, 0x17),
        ["Surface"] = (0x16, 0x1b, 0x22),
        ["SurfaceAlt"] = (0x1c, 0x23, 0x33),
        ["Text"] = (0xe6, 0xed, 0xf3),
        ["DimText"] = (0x8b, 0x94, 0x9e),
        ["Accent"] = (0x0e, 0xa5, 0xe9),
        ["AccentHover"] = (0x38, 0xbd, 0xf8),
        ["Border"] = (0x30, 0x36, 0x3d),
        ["ButtonBg"] = (0x21, 0x26, 0x2d),
        ["ButtonHover"] = (0x30, 0x36, 0x3d),
        ["ListAlt"] = (0x16, 0x1b, 0x22),
        ["ListSelected"] = (0x2a, 0x1f, 0x5e),
        ["StatusBar"] = (0x16, 0x1b, 0x22),
        ["Error"] = (0xf8, 0x51, 0x49),
        ["Success"] = (0x3f, 0xb9, 0x50),
    };

    private static readonly Dictionary<string, (byte r, byte g, byte b)> LightColors = new()
    {
        ["Bg"] = (0xf0, 0xf2, 0xf5),
        ["Surface"] = (0xff, 0xff, 0xff),
        ["SurfaceAlt"] = (0xf6, 0xf8, 0xfa),
        ["Text"] = (0x1f, 0x23, 0x28),
        ["DimText"] = (0x65, 0x6d, 0x76),
        ["Accent"] = (0x02, 0x84, 0xc7),
        ["AccentHover"] = (0x03, 0x69, 0xa8),
        ["Border"] = (0xd0, 0xd7, 0xde),
        ["ButtonBg"] = (0xf6, 0xf8, 0xfa),
        ["ButtonHover"] = (0xea, 0xee, 0xf2),
        ["ListAlt"] = (0xf6, 0xf8, 0xfa),
        ["ListSelected"] = (0xd4, 0xd0, 0xf0),
        ["StatusBar"] = (0xff, 0xff, 0xff),
        ["Error"] = (0xcf, 0x22, 0x2e),
        ["Success"] = (0x1a, 0x7f, 0x37),
    };

    public static void ApplyTheme(string theme)
    {
        var colors = theme == "light" ? LightColors : DarkColors;
        foreach (var kvp in colors)
        {
            var c = kvp.Value;
            Current.Resources[kvp.Key + "Brush"] = new SolidColorBrush(Color.FromRgb(c.r, c.g, c.b));
        }
    }

    public static void ApplyLanguage(string lang)
    {
        L10n.Lang = lang;
    }

    public static bool StartMinimized { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args.Contains("--minimized"))
            StartMinimized = true;

        var theme = "dark";
        var lang = "en";

        try
        {
            var path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasyBinds", "settings.json");
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Theme", out var t))
                    theme = t.GetString() ?? "dark";
                if (doc.RootElement.TryGetProperty("Language", out var l))
                    lang = l.GetString() ?? "en";
            }
        }
        catch { }

        L10n.Lang = lang;
        ApplyTheme(theme);
    }
}
