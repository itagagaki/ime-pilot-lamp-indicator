namespace ImePilotLamp;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();

        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(28, 28, 28);
        ClientSize = new Size(68, 76);
        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.None;
        Name = "MainForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "IME Pilot Lamp";
        TopMost = true;

        // Default position: top-right corner of the primary screen's working area
        var workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
        Location = new Point(workArea.Right - ClientSize.Width - 12, workArea.Top + 12);

        ResumeLayout(false);
    }
}
