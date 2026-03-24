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

    /// <summary>Gets the selected background color as an ARGB integer.</summary>
    public int BackColorArgb => _pnlBackColor.BackColor.ToArgb();

    /// <summary>Gets whether the transparent background option is selected.</summary>
    public bool TransparentBackground => _chkTransparent.Checked;

    public SettingsForm(AppSettings settings)
    {
        InitializeComponent();
        using var iconStream = typeof(SettingsForm).Assembly.GetManifestResourceStream("ImePilotLamp.app.ico");
        if (iconStream is not null) Icon = new Icon(iconStream);
        var wa = Screen.FromPoint(Cursor.Position).WorkingArea;
        Location = new Point(wa.Left + (wa.Width - Width) / 2, wa.Top + (wa.Height - Height) / 2);
        _chkFollowFocus.Checked = settings.FollowFocus;
        _trkFadeOut.Value = (int)Math.Round(Math.Clamp(settings.FadeOutSeconds * 10, 0.0, 30.0));
        UpdateFadeLabel();
        _trkFadeOut.ValueChanged += (_, _) => UpdateFadeLabel();
        _chkFollowFocus.CheckedChanged += (_, _) => _grpFadeOut.Enabled = _chkFollowFocus.Checked;
        _grpFadeOut.Enabled = _chkFollowFocus.Checked;

        _pnlBackColor.BackColor = Color.FromArgb(settings.BackColorArgb);
        _chkTransparent.Checked = settings.TransparentBackground;
        UpdateBackColorControls();
        _chkTransparent.CheckedChanged += (_, _) => UpdateBackColorControls();
        _btnPickColor.Click += BtnPickColor_Click;
    }

    private void BtnPickColor_Click(object? sender, EventArgs e)
    {
        using var dlg = new ColorDialog { Color = _pnlBackColor.BackColor, FullOpen = true };
        if (dlg.ShowDialog(this) == DialogResult.OK)
            _pnlBackColor.BackColor = dlg.Color;
    }

    private void UpdateBackColorControls()
    {
        bool enabled = !_chkTransparent.Checked;
        _pnlBackColor.Enabled = enabled;
        _btnPickColor.Enabled = enabled;
    }

    private void UpdateFadeLabel()
    {
        _lblFadeValue.Text = _trkFadeOut.Value == 0 ? "なし" : $"{_trkFadeOut.Value * 0.1:0.0} 秒";
    }
}
