using System.Drawing.Drawing2D;
using System.Text;

namespace ImePilotLamp;

/// <summary>
/// The main pilot lamp indicator form.
/// Displays a colored "lamp" that glows green when IME is ON and dims when IME is OFF.
/// The window is always on top, borderless, and can be dragged anywhere on screen.
/// Right-click shows a context menu with an Exit option.
/// </summary>
public partial class MainForm : Form
{
    private readonly System.Windows.Forms.Timer _pollTimer;
    private readonly System.Windows.Forms.Timer _fadeTimer;
    private readonly NotifyIcon _notifyIcon;
    private const int ToggleVisibilityHotkeyId = 1;

    private const int    WS_POPUP        = unchecked((int)0x80000000);
    private const int    WS_EX_NOACTIVATE = 0x08000000;
    private const double FadeMinOpacity  = 0.0;
    private const int    FadeIntervalMs  = 50;

    // Chroma-key colour used when TransparentBackground is active.
    // Chosen to be far from any colour that appears in the lamp drawing.
    private static readonly Color TransparencyKeyColor = Color.FromArgb(1, 0, 1);

    private bool _imeOn;
    private Point _dragOffset;
    private bool _dragging;
    private double _fadeStep;

    // Remember the last foreground window that wasn't our own indicator
    private IntPtr _lastForeignWindow = IntPtr.Zero;

    // Settings
    private readonly AppSettings _settings = AppSettings.Load();
    private SettingsForm? _settingsForm;

    public MainForm()
    {
        InitializeComponent();

        // Poll IME state every 100 ms
        _pollTimer = new System.Windows.Forms.Timer { Interval = 100 };
        _pollTimer.Tick += PollTimer_Tick;
        _pollTimer.Start();

        // Fade-out timer (fires every 50 ms to gradually reduce opacity after follow-focus move)
        _fadeTimer = new System.Windows.Forms.Timer { Interval = FadeIntervalMs };
        _fadeTimer.Tick += FadeTimer_Tick;

        // System tray icon so the app is accessible even when the window is hidden
        _notifyIcon = new NotifyIcon { Text = "IME Pilot Lamp Indicator", Visible = true };
        UpdateTrayIcon();

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Show / Hide  (Ctrl+Alt+L)", null, (_, _) => ToggleVisibility());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("設定...", null, (_, _) => OpenSettings());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Exit", null, (_, _) => ExitApplication());

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.MouseClick += (_, e) => { if (e.Button == MouseButtons.Left) ToggleIme(); };

        // Draw the initial (OFF) state
        UpdateImeState();
    }

    // -----------------------------------------------------------------------
    // IME state detection
    // -----------------------------------------------------------------------

    private void PollTimer_Tick(object? sender, EventArgs e) => UpdateImeState();

    private void UpdateImeState()
    {
        bool newState = GetImeOpenStatus();
        if (newState == _imeOn) return;

        _imeOn = newState;
        UpdateTrayIcon();
        if (Visible && Opacity < 1.0)
        {
            // Show and fade-out again
            _fadeTimer.Stop();
            Opacity = 1.0;
            _fadeTimer.Start();
        }
        Invalidate();
    }

    private bool GetImeOpenStatus()
    {
        IntPtr foreground = NativeMethods.GetForegroundWindow();

        // Skip windows that belong to this process (main form, settings dialog,
        // context menus, etc.) to avoid triggering spurious follow-focus moves.
        if (foreground == IntPtr.Zero || IsOwnWindow(foreground))
            foreground = _lastForeignWindow;
        else if (foreground != _lastForeignWindow)
        {
            if (_settings.FollowFocus && !IsDesktopOrTaskbar(foreground))
                MoveNearCursor(Cursor.Position);
            _lastForeignWindow = foreground;
        }

        if (foreground == IntPtr.Zero) return false;

        IntPtr imeWnd = NativeMethods.ImmGetDefaultIMEWnd(foreground);
        if (imeWnd == IntPtr.Zero) return false;

        IntPtr result = NativeMethods.SendMessage(
            imeWnd,
            NativeMethods.WM_IME_CONTROL,
            (IntPtr)NativeMethods.IMC_GETOPENSTATUS,
            IntPtr.Zero);

        return result != IntPtr.Zero;
    }

    // -----------------------------------------------------------------------
    // Painting – the pilot lamp
    // -----------------------------------------------------------------------

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        float scale = DeviceDpi / 96.0f;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // --- Background ---
        g.Clear(_settings.TransparentBackground
            ? TransparencyKeyColor
            : Color.FromArgb(_settings.BackColorArgb));

        // --- Lamp circle (centred horizontally, near the top) ---
        int lampDiameter = (int)(44 * scale);
        int lampX = (ClientSize.Width - lampDiameter) / 2;
        int lampY = (int)(10 * scale);
        var lampRect = new RectangleF(lampX, lampY, lampDiameter, lampDiameter);

        if (_imeOn)
        {
            // Outer glow – concentric opaque ellipses drawn from outer (background colour)
            // to inner (bright green), so the halo is brightest at the lamp rim and fades
            // outward without relying on alpha compositing (unreliable with AllowTransparency).
            {
                int maxGlowPad = (int)(10 * scale);
                Color glowBg = _settings.TransparentBackground
                    ? TransparencyKeyColor
                    : Color.FromArgb(_settings.BackColorArgb);
                Color glowFg = Color.FromArgb(120, 255, 120);
                for (int pad = maxGlowPad; pad >= 1; pad--)
                {
                    float t = (float)pad / maxGlowPad;   // 1.0 = outermost → bgColor, 0 → glowFg
                    int rVal = (int)(glowBg.R + (glowFg.R - glowBg.R) * (1f - t));
                    int gVal = (int)(glowBg.G + (glowFg.G - glowBg.G) * (1f - t));
                    int bVal = (int)(glowBg.B + (glowFg.B - glowBg.B) * (1f - t));
                    var layerRect = new RectangleF(lampX - pad, lampY - pad, lampDiameter + pad * 2, lampDiameter + pad * 2);
                    using var layerBrush = new SolidBrush(Color.FromArgb(
                        Math.Clamp(rVal, 0, 255), Math.Clamp(gVal, 0, 255), Math.Clamp(bVal, 0, 255)));
                    g.FillEllipse(layerBrush, layerRect);
                }
            }

            // Lamp body – radial gradient: white centre → bright green edge
            using var lampPath = new System.Drawing.Drawing2D.GraphicsPath();
            lampPath.AddEllipse(lampRect);
            using var lampBrush = new PathGradientBrush(lampPath)
            {
                CenterPoint = new PointF(lampX + lampDiameter * 0.38f, lampY + lampDiameter * 0.32f),
                CenterColor = Color.White,
                SurroundColors = new[] { Color.FromArgb(0, 180, 0) }
            };
            g.FillEllipse(lampBrush, lampRect);
        }
        else
        {
            // Dark, unlit lamp
            using var bodyBrush = new SolidBrush(Color.FromArgb(55, 55, 55));
            g.FillEllipse(bodyBrush, lampRect);
            using var rimPen = new Pen(Color.FromArgb(80, 80, 80), 1f);
            g.DrawEllipse(rimPen, lampRect);

            // Small highlight to give a "glass" feel even when off
            var hlRect = new RectangleF(lampX + 7 * scale, lampY + 6 * scale, lampDiameter * 0.35f, lampDiameter * 0.25f);
            using var hlBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255));
            g.FillEllipse(hlBrush, hlRect);
        }

        // --- Status text ---
        string statusText = _imeOn ? "ON" : "OFF";
        Color textColor = _imeOn ? Color.FromArgb(100, 255, 100) : Color.FromArgb(110, 110, 110);
        using var font = new Font("Segoe UI", 9.5f, FontStyle.Bold, GraphicsUnit.Point);
        using var textBrush = new SolidBrush(textColor);
        var textSize = g.MeasureString(statusText, font);
        float textX = (ClientSize.Width - textSize.Width) / 2f;
        float textY = lampY + lampDiameter + 5 * scale;
        g.DrawString(statusText, font, textBrush, textX, textY);

        // --- Thin border around the whole window ---
        if (!_settings.TransparentBackground)
        {
            using var borderPen = new Pen(Color.FromArgb(60, 60, 60), 1f);
            g.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }
    }

    // -----------------------------------------------------------------------
    // Tray icon – generated programmatically from current state
    // -----------------------------------------------------------------------

    private void UpdateTrayIcon()
    {
        var oldIcon = _notifyIcon.Icon;

        using var bmp = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            using var brush = new SolidBrush(_imeOn ? Color.LimeGreen : Color.FromArgb(90, 90, 90));
            g.FillEllipse(brush, 1, 1, 14, 14);
        }

        IntPtr hIcon = bmp.GetHicon();
        _notifyIcon.Icon = Icon.FromHandle(hIcon);
        NativeMethods.DestroyIcon(hIcon);
        _notifyIcon.Text = _imeOn ? "IME: ON" : "IME: OFF";

        oldIcon?.Dispose();
    }

    // -----------------------------------------------------------------------
    // Mouse – dragging the borderless window
    // -----------------------------------------------------------------------

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragging = true;
            _dragOffset = e.Location;
            _fadeTimer.Stop();
            Opacity = 1.0;
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_dragging)
            Location = new Point(
                Location.X + e.X - _dragOffset.X,
                Location.Y + e.Y - _dragOffset.Y);
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _dragging = false;
        base.OnMouseUp(e);
    }

    // -----------------------------------------------------------------------
    // Follow-focus
    // -----------------------------------------------------------------------

    private static bool IsDesktopOrTaskbar(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero) return false;
        var sb = new StringBuilder(64);
        NativeMethods.GetClassName(hwnd, sb, sb.Capacity);
        return sb.ToString() is "Progman" or "WorkerW" or "Shell_TrayWnd" or "Shell_SecondaryTrayWnd";
    }

    private static bool IsOwnWindow(IntPtr hwnd)
    {
        NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
        return pid == (uint)Environment.ProcessId;
    }

    private void MoveNearCursor(Point cursorPos)
    {
        _fadeTimer.Stop();
        Opacity = 1.0;
        const int offsetY = 20;

        var screen = Screen.FromPoint(cursorPos);
        var workArea = screen.WorkingArea;

        int x = cursorPos.X - Width / 2;
        int y = cursorPos.Y - Height - offsetY;

        // Clamp horizontally within the working area
        x = Math.Max(workArea.Left, Math.Min(x, workArea.Right - Width));

        // Prefer avobe the cursor; fall back to below if it would go off-screen
        if (y < workArea.Top)
            y = cursorPos.Y + offsetY;

        // Final vertical clamp
        y = Math.Max(workArea.Top, Math.Min(y, workArea.Bottom - Height));

        Location = new Point(x, y);
        if (!Visible) Visible = true;
        BringToFront();
        StartFadeOut();
    }

    // -----------------------------------------------------------------------
    // Fade-out after follow-focus move
    // -----------------------------------------------------------------------

    private void StartFadeOut()
    {
        if (_settings.FadeOutSeconds <= 0) return;
        int steps = (int)Math.Max(1, _settings.FadeOutSeconds * 1000 / FadeIntervalMs);
        _fadeStep = (1.0 - FadeMinOpacity) / steps;
        _fadeTimer.Start();
    }

    private void FadeTimer_Tick(object? sender, EventArgs e)
    {
        double newOpacity = Opacity - _fadeStep;
        if (newOpacity <= FadeMinOpacity)
        {
            Opacity = FadeMinOpacity;
            _fadeTimer.Stop();
        }
        else
        {
            Opacity = newOpacity;
        }
    }

#if false
    // Optional: cancel fade-out and restore full opacity when the user hovers over the window
    protected override void OnMouseEnter(EventArgs e)
    {
        _fadeTimer.Stop();
        Opacity = 1.0;
        base.OnMouseEnter(e);
    }
#endif

    // -----------------------------------------------------------------------
    // IME toggle
    // -----------------------------------------------------------------------

    private void ToggleIme()
    {
        IntPtr target = _lastForeignWindow;
        if (target == IntPtr.Zero)
        {
            target = NativeMethods.GetForegroundWindow();
            if (target == Handle || target == IntPtr.Zero) return;
        }

        IntPtr imeWnd = NativeMethods.ImmGetDefaultIMEWnd(target);
        if (imeWnd == IntPtr.Zero) return;

        bool current = NativeMethods.SendMessage(
            imeWnd,
            NativeMethods.WM_IME_CONTROL,
            (IntPtr)NativeMethods.IMC_GETOPENSTATUS,
            IntPtr.Zero) != IntPtr.Zero;

        NativeMethods.SendMessage(
            imeWnd,
            NativeMethods.WM_IME_CONTROL,
            (IntPtr)NativeMethods.IMC_SETOPENSTATUS,
            (IntPtr)(current ? 0 : 1));

        UpdateImeState();
    }

    // -----------------------------------------------------------------------
    // Settings
    // -----------------------------------------------------------------------

    private void OpenSettings()
    {
        if (_settingsForm != null)
        {
            _settingsForm.Activate();
            return;
        }

        using var form = new SettingsForm(_settings);
        _settingsForm = form;
        try
        {
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _settings.FollowFocus = form.FollowFocus;
                _settings.FadeOutSeconds = form.FadeOutSeconds;
                _settings.BackColorArgb = form.BackColorArgb;
                _settings.TransparentBackground = form.TransparentBackground;
                _settings.Save();
                ApplySettings();
            }
        }
        finally
        {
            _settingsForm = null;
        }
    }

    private void ApplySettings()
    {
        ApplyBackground();
    }

    // -----------------------------------------------------------------------
    // Window closing / exit
    // -----------------------------------------------------------------------

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.Style  |= WS_POPUP;         // Popup window: no Windows-imposed minimum width
            cp.ExStyle |= WS_EX_NOACTIVATE; // Never steal keyboard focus on click
            return cp;
        }
    }

    /// <summary>Minimise to tray instead of closing when the user presses the close button.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Visible = false;
        }
        else
        {
            CleanUp();
        }
        base.OnFormClosing(e);
    }

    private void ToggleVisibility()
    {
        if (Opacity <= 0.0)
        {
            // Treat as "currently hidden"
            Visible = false;
        }
        if (!Visible)
        {
            _fadeTimer.Stop();
            Opacity = 1.0;
        }
        Visible = !Visible;
        if (Visible) BringToFront();
    }

    private void ApplyDpiSize()
    {
        float scale = DeviceDpi / 96.0f;
        ClientSize = new Size((int)Math.Round(68 * scale), (int)Math.Round(76 * scale));
    }

    private void ApplyBackground()
    {
        TransparencyKey = _settings.TransparentBackground ? TransparencyKeyColor : Color.Empty;
        Invalidate();
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        ApplyDpiSize();
        Invalidate();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ApplyDpiSize();
        ApplyBackground();
        if (_settings.WindowX is int wx && _settings.WindowY is int wy &&
            Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(wx, wy, Width, Height))))
        {
            Location = new Point(wx, wy);
        }
        else
        {
            var workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
            Location = new Point(workArea.Right - Width - 12, workArea.Top + 12);
        }
        NativeMethods.RegisterHotKey(Handle, ToggleVisibilityHotkeyId,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT, (uint)Keys.L);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == ToggleVisibilityHotkeyId)
            ToggleVisibility();
        else
            base.WndProc(ref m);
    }

    private void ExitApplication()
    {
        CleanUp();
        Application.Exit();
    }

    private void CleanUp()
    {
        _settings.WindowX = Location.X;
        _settings.WindowY = Location.Y;
        _settings.Save();
        NativeMethods.UnregisterHotKey(Handle, ToggleVisibilityHotkeyId);
        _pollTimer.Stop();
        _fadeTimer.Stop();
        _notifyIcon.Visible = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pollTimer?.Dispose();
            _fadeTimer?.Dispose();
            _notifyIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
