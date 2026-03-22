using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
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
    private readonly NotifyIcon _notifyIcon;
    private const int ToggleVisibilityHotkeyId = 1;

    private bool _imeOn;
    private Point _dragOffset;
    private bool _dragging;

    // Remember the last foreground window that wasn't our own indicator
    private IntPtr _lastForeignWindow = IntPtr.Zero;

    // Settings
    private readonly AppSettings _settings = AppSettings.Load();

    // Follow-focus mouse hook state
    private NativeMethods.HookProc? _mouseHookProc; // Held to prevent GC collection
    private IntPtr _mouseHook = IntPtr.Zero;
    private bool _pendingMouseClick;
    private Point _mouseClickPoint;
    private DateTime _mouseClickTime;

    public MainForm()
    {
        InitializeComponent();

        // Restore the last saved window position if it is still on screen
        if (_settings.WindowX is int wx && _settings.WindowY is int wy &&
            Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(wx, wy, Width, Height))))
        {
            Location = new Point(wx, wy);
        }

        // Poll IME state every 100 ms
        _pollTimer = new System.Windows.Forms.Timer { Interval = 100 };
        _pollTimer.Tick += PollTimer_Tick;
        _pollTimer.Start();

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

        if (_settings.FollowFocus)
            InstallMouseHook();
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
        Invalidate();
    }

    private bool GetImeOpenStatus()
    {
        IntPtr foreground = NativeMethods.GetForegroundWindow();

        // Ignore our own window to avoid disturbing the last-known state
        if (foreground == Handle || foreground == IntPtr.Zero)
            foreground = _lastForeignWindow;
        else
        {
            if (foreground != _lastForeignWindow)
            {
                if (_settings.FollowFocus && _pendingMouseClick &&
                    (DateTime.UtcNow - _mouseClickTime).TotalSeconds < 2.0 &&
                    !IsDesktopOrTaskbar(foreground))
                {
                    MoveNearCursor(_mouseClickPoint);
                }
                _pendingMouseClick = false;
                _lastForeignWindow = foreground;
            }
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

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // --- Background ---
        g.Clear(Color.FromArgb(28, 28, 28));

        // --- Lamp circle (centred horizontally, near the top) ---
        int lampDiameter = 44;
        int lampX = (ClientSize.Width - lampDiameter) / 2;
        int lampY = 10;
        var lampRect = new RectangleF(lampX, lampY, lampDiameter, lampDiameter);

        if (_imeOn)
        {
            // Outer glow
            int glowPad = 6;
            var glowRect = new RectangleF(
                lampX - glowPad, lampY - glowPad,
                lampDiameter + glowPad * 2, lampDiameter + glowPad * 2);
            using var glowPath = new System.Drawing.Drawing2D.GraphicsPath();
            glowPath.AddEllipse(glowRect);
            using var glowBrush = new PathGradientBrush(glowPath)
            {
                CenterColor = Color.FromArgb(160, Color.LimeGreen),
                SurroundColors = new[] { Color.Transparent }
            };
            g.FillEllipse(glowBrush, glowRect);

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
            var hlRect = new RectangleF(lampX + 7, lampY + 6, lampDiameter * 0.35f, lampDiameter * 0.25f);
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
        float textY = lampY + lampDiameter + 5;
        g.DrawString(statusText, font, textBrush, textX, textY);

        // --- Thin border around the whole window ---
        using var borderPen = new Pen(Color.FromArgb(60, 60, 60), 1f);
        g.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
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
    // Follow-focus – low-level mouse hook
    // -----------------------------------------------------------------------

    private void InstallMouseHook()
    {
        if (_mouseHook != IntPtr.Zero) return;
        _mouseHookProc = MouseHookCallback;
        _mouseHook = NativeMethods.SetWindowsHookEx(
            NativeMethods.WH_MOUSE_LL, _mouseHookProc, IntPtr.Zero, 0);
    }

    private void UninstallMouseHook()
    {
        if (_mouseHook == IntPtr.Zero) return;
        NativeMethods.UnhookWindowsHookEx(_mouseHook);
        _mouseHook = IntPtr.Zero;
        _mouseHookProc = null;
    }

    private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (int)wParam == NativeMethods.WM_LBUTTONDOWN)
        {
            var hs = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
            _pendingMouseClick = true;
            _mouseClickPoint = new Point(hs.pt.x, hs.pt.y);
            _mouseClickTime = DateTime.UtcNow;
        }
        return NativeMethods.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
    }

    private static bool IsDesktopOrTaskbar(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero) return false;
        var sb = new StringBuilder(64);
        NativeMethods.GetClassName(hwnd, sb, sb.Capacity);
        return sb.ToString() is "Progman" or "WorkerW" or "Shell_TrayWnd" or "Shell_SecondaryTrayWnd";
    }

    private void MoveNearCursor(Point cursorPos)
    {
        const int offsetY = 20;

        var screen = Screen.FromPoint(cursorPos);
        var workArea = screen.WorkingArea;

        int x = cursorPos.X - Width / 2;
        int y = cursorPos.Y + offsetY;

        // Clamp horizontally within the working area
        x = Math.Max(workArea.Left, Math.Min(x, workArea.Right - Width));

        // Prefer below the cursor; fall back to above if it would go off-screen
        if (y + Height > workArea.Bottom)
            y = cursorPos.Y - Height - offsetY;

        // Final vertical clamp
        y = Math.Max(workArea.Top, Math.Min(y, workArea.Bottom - Height));

        Location = new Point(x, y);
        if (!Visible) Visible = true;
        BringToFront();
    }

    // -----------------------------------------------------------------------
    // IME toggle
    // -----------------------------------------------------------------------

    private void ToggleIme()
    {
        _pendingMouseClick = false;
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
        using var form = new SettingsForm(_settings);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _settings.FollowFocus = form.FollowFocus;
            _settings.Save();
            ApplySettings();
        }
    }

    private void ApplySettings()
    {
        if (_settings.FollowFocus)
            InstallMouseHook();
        else
            UninstallMouseHook();
    }

    // -----------------------------------------------------------------------
    // Window closing / exit
    // -----------------------------------------------------------------------

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
        Visible = !Visible;
        if (Visible) BringToFront();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
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
        UninstallMouseHook();
        _notifyIcon.Visible = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pollTimer?.Dispose();
            _notifyIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
