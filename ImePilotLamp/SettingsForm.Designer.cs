namespace ImePilotLamp;

partial class SettingsForm
{
    private System.ComponentModel.IContainer? components = null;
    private System.Windows.Forms.GroupBox _grpFollowFocus;
    private System.Windows.Forms.Label _lblDescription;
    private System.Windows.Forms.CheckBox _chkFollowFocus;
    private System.Windows.Forms.Button _btnOk;
    private System.Windows.Forms.Button _btnCancel;
    private System.Windows.Forms.GroupBox _grpFadeOut;
    private System.Windows.Forms.Label _lblFadeDescription;
    private System.Windows.Forms.TrackBar _trkFadeOut;
    private System.Windows.Forms.Label _lblFadeValue;
    private System.Windows.Forms.GroupBox _grpBackground;
    private System.Windows.Forms.Panel _pnlBackColor;
    private System.Windows.Forms.Button _btnPickColor;
    private System.Windows.Forms.CheckBox _chkTransparent;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _grpFollowFocus = new System.Windows.Forms.GroupBox();
        _lblDescription = new System.Windows.Forms.Label();
        _chkFollowFocus = new System.Windows.Forms.CheckBox();
        _grpFadeOut = new System.Windows.Forms.GroupBox();
        _lblFadeDescription = new System.Windows.Forms.Label();
        _trkFadeOut = new System.Windows.Forms.TrackBar();
        _lblFadeValue = new System.Windows.Forms.Label();
        _grpBackground = new System.Windows.Forms.GroupBox();
        _pnlBackColor = new System.Windows.Forms.Panel();
        _btnPickColor = new System.Windows.Forms.Button();
        _chkTransparent = new System.Windows.Forms.CheckBox();
        _btnOk = new System.Windows.Forms.Button();
        _btnCancel = new System.Windows.Forms.Button();
        _grpFollowFocus.SuspendLayout();
        _grpFadeOut.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_trkFadeOut).BeginInit();
        _grpBackground.SuspendLayout();
        SuspendLayout();

        // _grpFollowFocus
        _grpFollowFocus.Controls.Add(_lblDescription);
        _grpFollowFocus.Controls.Add(_chkFollowFocus);
        _grpFollowFocus.Location = new System.Drawing.Point(12, 8);
        _grpFollowFocus.Name = "_grpFollowFocus";
        _grpFollowFocus.Size = new System.Drawing.Size(400, 108);
        _grpFollowFocus.TabStop = false;
        _grpFollowFocus.Text = "フォーカス追従";

        // _lblDescription
        _lblDescription.Location = new System.Drawing.Point(12, 22);
        _lblDescription.Name = "_lblDescription";
        _lblDescription.Size = new System.Drawing.Size(376, 46);
        _lblDescription.Text = "マウスクリックによってアクティブなウィンドウが移動したとき、IMEパイロットランプウィンドウをマウスポインターの近くへ自動的に移動します。";

        // _chkFollowFocus
        _chkFollowFocus.AutoSize = true;
        _chkFollowFocus.Location = new System.Drawing.Point(12, 74);
        _chkFollowFocus.Name = "_chkFollowFocus";
        _chkFollowFocus.TabIndex = 0;
        _chkFollowFocus.Text = "フォーカス追従を有効にする(&F)";

        // _grpFadeOut
        _grpFadeOut.Controls.Add(_lblFadeDescription);
        _grpFadeOut.Controls.Add(_trkFadeOut);
        _grpFadeOut.Controls.Add(_lblFadeValue);
        _grpFadeOut.Location = new System.Drawing.Point(12, 124);
        _grpFadeOut.Name = "_grpFadeOut";
        _grpFadeOut.Size = new System.Drawing.Size(400, 112);
        _grpFadeOut.TabStop = false;
        _grpFadeOut.Text = "フェードアウト";

        // _lblFadeDescription
        _lblFadeDescription.AutoSize = false;
        _lblFadeDescription.Location = new System.Drawing.Point(12, 20);
        _lblFadeDescription.Name = "_lblFadeDescription";
        _lblFadeDescription.Size = new System.Drawing.Size(376, 32);
        _lblFadeDescription.Text = "フォーカス追従で移動した後、ウィンドウを徐々に半透明にする時間を指定します。";

        // _trkFadeOut
        _trkFadeOut.AutoSize = false;
        _trkFadeOut.LargeChange = 5;
        _trkFadeOut.Location = new System.Drawing.Point(12, 58);
        _trkFadeOut.Maximum = 30;
        _trkFadeOut.Name = "_trkFadeOut";
        _trkFadeOut.Size = new System.Drawing.Size(278, 45);
        _trkFadeOut.SmallChange = 1;
        _trkFadeOut.TabIndex = 1;
        _trkFadeOut.TickFrequency = 5;

        // _lblFadeValue
        _lblFadeValue.AutoSize = false;
        _lblFadeValue.Location = new System.Drawing.Point(294, 71);
        _lblFadeValue.Name = "_lblFadeValue";
        _lblFadeValue.Size = new System.Drawing.Size(94, 20);
        _lblFadeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        // _grpBackground
        _grpBackground.Controls.Add(_pnlBackColor);
        _grpBackground.Controls.Add(_btnPickColor);
        _grpBackground.Controls.Add(_chkTransparent);
        _grpBackground.Location = new System.Drawing.Point(12, 244);
        _grpBackground.Name = "_grpBackground";
        _grpBackground.Size = new System.Drawing.Size(400, 62);
        _grpBackground.TabStop = false;
        _grpBackground.Text = "背景色";

        // _pnlBackColor
        _pnlBackColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        _pnlBackColor.Location = new System.Drawing.Point(12, 22);
        _pnlBackColor.Name = "_pnlBackColor";
        _pnlBackColor.Size = new System.Drawing.Size(30, 28);
        _pnlBackColor.TabStop = false;

        // _btnPickColor
        _btnPickColor.Location = new System.Drawing.Point(50, 20);
        _btnPickColor.Name = "_btnPickColor";
        _btnPickColor.Size = new System.Drawing.Size(110, 28);
        _btnPickColor.TabIndex = 4;
        _btnPickColor.Text = "色を選択(&C)...";

        // _chkTransparent
        _chkTransparent.AutoSize = true;
        _chkTransparent.Location = new System.Drawing.Point(174, 24);
        _chkTransparent.Name = "_chkTransparent";
        _chkTransparent.TabIndex = 5;
        _chkTransparent.Text = "透過（背景なし）(&T)";

        // _btnOk
        _btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        _btnOk.Location = new System.Drawing.Point(228, 318);
        _btnOk.Name = "_btnOk";
        _btnOk.Size = new System.Drawing.Size(80, 26);
        _btnOk.TabIndex = 2;
        _btnOk.Text = "OK";

        // _btnCancel
        _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        _btnCancel.Location = new System.Drawing.Point(316, 318);
        _btnCancel.Name = "_btnCancel";
        _btnCancel.Size = new System.Drawing.Size(96, 26);
        _btnCancel.TabIndex = 3;
        _btnCancel.Text = "キャンセル";

        // SettingsForm
        AcceptButton = _btnOk;
        CancelButton = _btnCancel;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(424, 356);
        Controls.Add(_grpFollowFocus);
        Controls.Add(_grpFadeOut);
        Controls.Add(_grpBackground);
        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        ShowInTaskbar = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "設定 - IME Pilot Lamp";
        _grpFollowFocus.ResumeLayout(false);
        _grpFollowFocus.PerformLayout();
        _grpFadeOut.ResumeLayout(false);
        _grpFadeOut.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_trkFadeOut).EndInit();
        _grpBackground.ResumeLayout(false);
        _grpBackground.PerformLayout();
        ResumeLayout(false);
    }
}
