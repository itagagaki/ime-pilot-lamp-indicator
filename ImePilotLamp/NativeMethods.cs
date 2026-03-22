using System.Runtime.InteropServices;
using System.Text;

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

    /// <summary>Low-level mouse hook identifier.</summary>
    internal const int WH_MOUSE_LL = 14;

    /// <summary>Posted when the left mouse button is pressed.</summary>
    internal const int WM_LBUTTONDOWN = 0x0201;

    /// <summary>Callback delegate used with SetWindowsHookEx.</summary>
    internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>Screen coordinates used in low-level hook structs.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT { public int x; public int y; }

    /// <summary>Data received by a low-level mouse hook procedure.</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

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

    /// <summary>Installs a system-wide hook procedure.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>Removes a hook procedure installed by SetWindowsHookEx.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>Passes the hook information to the next hook in the chain.</summary>
    [DllImport("user32.dll")]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>Retrieves the name of the class to which the specified window belongs.</summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
}
