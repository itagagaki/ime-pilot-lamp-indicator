namespace ImePilotLamp;

/// <summary>
/// Settings dialog for the IME Pilot Lamp application.
/// </summary>
internal partial class SettingsForm : Form
{
    /// <summary>Gets the Follow Focus setting value as entered by the user.</summary>
    public bool FollowFocus => _chkFollowFocus.Checked;

    /// <summary>Gets the fade-out duration in seconds (0 = disabled).</summary>
    public double FadeOutSeconds => _trkFadeOut.Value * 0.1;

    public SettingsForm(AppSettings settings)
    {
        InitializeComponent();
        _chkFollowFocus.Checked = settings.FollowFocus;
        _trkFadeOut.Value = (int)Math.Round(Math.Clamp(settings.FadeOutSeconds * 10, 0.0, 30.0));
        UpdateFadeLabel();
        _trkFadeOut.ValueChanged += (_, _) => UpdateFadeLabel();
        _chkFollowFocus.CheckedChanged += (_, _) => _grpFadeOut.Enabled = _chkFollowFocus.Checked;
        _grpFadeOut.Enabled = _chkFollowFocus.Checked;
    }

    private void UpdateFadeLabel()
    {
        _lblFadeValue.Text = _trkFadeOut.Value == 0 ? "なし" : $"{_trkFadeOut.Value * 0.1:0.0} 秒";
    }
}
