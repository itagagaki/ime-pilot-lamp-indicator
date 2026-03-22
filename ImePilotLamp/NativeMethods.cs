using System.Runtime.InteropServices;

namespace ImePilotLamp;

/// <summary>
/// P/Invoke declarations for Windows IME and user32 APIs.
/// </summary>
internal static class NativeMethods
{
    /// <summary>Message sent to an IME window to control it.</summary>
    internal const int WM_IME_CONTROL = 0x0283;

    /// <summary>Subcommand: get the open/close status of the IME.</summary>
    internal const int IMC_GETOPENSTATUS = 0x0005;

    /// <summary>Returns the handle to the foreground window (the window with which the user is currently working).</summary>
    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    /// <summary>Returns the default window handle to the IME associated with the specified window.</summary>
    [DllImport("imm32.dll")]
    internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

    /// <summary>Sends the specified message to a window or windows.</summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    /// <summary>Destroys an icon and frees any memory the icon occupied.</summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon(IntPtr hIcon);
}
