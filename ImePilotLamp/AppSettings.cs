using System.Text.Json;

namespace ImePilotLamp;

/// <summary>
/// Application settings persisted as JSON in the user's AppData folder.
/// </summary>
internal class AppSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ImePilotLamp", "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// When true, the pilot lamp window automatically moves near the mouse cursor
    /// when keyboard focus changes via a mouse click.
    /// </summary>
    public bool FollowFocus { get; set; } = false;

    /// <summary>
    /// Last saved window position. Null means "use the default position".
    /// </summary>
    public int? WindowX { get; set; } = null;
    public int? WindowY { get; set; } = null;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, JsonOptions));
        }
        catch { }
    }
}
