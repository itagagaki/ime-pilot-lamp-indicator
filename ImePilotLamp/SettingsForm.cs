namespace ImePilotLamp;

/// <summary>
/// Settings dialog for the IME Pilot Lamp application.
/// </summary>
internal partial class SettingsForm : Form
{
    /// <summary>Gets the Follow Focus setting value as entered by the user.</summary>
    public bool FollowFocus => _chkFollowFocus.Checked;

    public SettingsForm(AppSettings settings)
    {
        InitializeComponent();
        _chkFollowFocus.Checked = settings.FollowFocus;
    }
}
