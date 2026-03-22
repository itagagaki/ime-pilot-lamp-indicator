namespace ImePilotLamp;

partial class SettingsForm
{
    private System.ComponentModel.IContainer? components = null;
    private System.Windows.Forms.GroupBox _grpFollowFocus;
    private System.Windows.Forms.Label _lblDescription;
    private System.Windows.Forms.CheckBox _chkFollowFocus;
    private System.Windows.Forms.Button _btnOk;
    private System.Windows.Forms.Button _btnCancel;

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
        _btnOk = new System.Windows.Forms.Button();
        _btnCancel = new System.Windows.Forms.Button();
        _grpFollowFocus.SuspendLayout();
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
        _lblDescription.Text = "マウスクリックによってキーボードフォーカスが移動したとき、パイロットランプウィンドウをカーソルの近くへ自動的に移動します。";

        // _chkFollowFocus
        _chkFollowFocus.AutoSize = true;
        _chkFollowFocus.Location = new System.Drawing.Point(12, 74);
        _chkFollowFocus.Name = "_chkFollowFocus";
        _chkFollowFocus.TabIndex = 0;
        _chkFollowFocus.Text = "フォーカス追従を有効にする(&F)";

        // _btnOk
        _btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        _btnOk.Location = new System.Drawing.Point(228, 128);
        _btnOk.Name = "_btnOk";
        _btnOk.Size = new System.Drawing.Size(80, 26);
        _btnOk.TabIndex = 1;
        _btnOk.Text = "OK";

        // _btnCancel
        _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        _btnCancel.Location = new System.Drawing.Point(316, 128);
        _btnCancel.Name = "_btnCancel";
        _btnCancel.Size = new System.Drawing.Size(96, 26);
        _btnCancel.TabIndex = 2;
        _btnCancel.Text = "キャンセル";

        // SettingsForm
        AcceptButton = _btnOk;
        CancelButton = _btnCancel;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(424, 166);
        Controls.Add(_grpFollowFocus);
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
        ResumeLayout(false);
    }
}
