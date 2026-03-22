# ime-pilot-lamp-indicator

A lightweight Windows desktop application that shows the current **IME (Input Method Editor) state** — **ON** or **OFF** — as a glowing pilot-lamp indicator.

## Features

- **Always-on-top** borderless mini-window, positioned in the top-right corner by default; position is saved between sessions
- **Green glowing lamp** when IME is ON (e.g. Japanese/Chinese/Korean input active)
- **Dim gray lamp** when IME is OFF (Latin / direct input)
- **System tray icon** that also reflects the current state
- **IME toggle**: left-click the tray icon to toggle IME ON/OFF in the currently active application
- **Global hotkey**: `Ctrl+Alt+L` toggles the indicator window visibility
- **Draggable**: left-click and drag the window anywhere on screen
- **Follow-focus** (optional): automatically moves the indicator near the mouse cursor whenever the active window changes via a mouse click
- **Fade-out** (optional): after a follow-focus move, the window gradually fades out over a configurable duration (0–3 s)
- **Customizable background**: choose any background color, or enable transparent mode to show only the lamp and label with no background
- **DPI-aware**: scales correctly on high-DPI / multi-monitor setups
- **Non-intrusive**: never steals keyboard focus (`WS_EX_NOACTIVATE`)
- **Polls every 100 ms** using the Windows IME API (`ImmGetDefaultIMEWnd` + `WM_IME_CONTROL / IMC_GETOPENSTATUS`)

## Screenshots

| IME OFF | IME ON |
|---------|--------|
| ![IME OFF – dim gray lamp](docs/ime-off.png) | ![IME ON – green glowing lamp](docs/ime-on.png) |

## Requirements

- Windows 10 / 11 (64-bit)
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (or newer)

## Build

```bash
cd ImePilotLamp
dotnet build -c Release
```

The compiled executable is placed in `ImePilotLamp/bin/Release/net8.0-windows/`.

## Publish (single-file, self-contained)

```bash
cd ImePilotLamp
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Usage

Run `ImePilotLamp.exe`. The pilot lamp appears in the top-right corner of your screen.

| Action | Result |
|--------|--------|
| Left-click and drag (lamp window) | Move the indicator window |
| Left-click tray icon | Toggle IME ON/OFF for the active application |
| Right-click tray icon | Open context menu |
| `Ctrl+Alt+L` | Toggle indicator window visibility |

### Context menu (right-click tray icon)

| Item | Action |
|------|--------|
| Show / Hide (Ctrl+Alt+L) | Toggle window visibility |
| 設定… | Open settings dialog |
| Exit | Quit the application |

### Settings dialog

| Setting | Description |
|---------|-------------|
| Follow-focus | Move the indicator near the cursor when the active window changes via a mouse click |
| Fade-out | Duration (0–3 s) before the indicator fades out after a follow-focus move; 0 = disabled |
| Background color | Choose a custom background color with the color picker |
| Transparent background | Remove the background entirely; only the lamp and label are visible |

Settings are saved automatically to `%AppData%\ImePilotLamp\settings.json`.

## How it works

1. A `System.Windows.Forms.Timer` fires every 100 ms.
2. The current foreground window handle is obtained via `GetForegroundWindow()`.
3. The IME window for that handle is resolved with `ImmGetDefaultIMEWnd()`.
4. A `WM_IME_CONTROL / IMC_GETOPENSTATUS` message is sent to the IME window.
5. A non-zero return value means IME is open (ON); zero means closed (OFF).
6. The indicator repaints only when the state actually changes.

When **follow-focus** is enabled, a low-level mouse hook (`WH_MOUSE_LL`) records each left-button click. If the foreground window changes within 2 seconds of a click (and the new window is not the desktop or taskbar), the indicator moves above the cursor. A configurable fade-out timer then gradually reduces the window's opacity.

## Project structure

```
ImePilotLamp/
  ImePilotLamp.csproj      – WinForms project (net8.0-windows)
  Program.cs               – Application entry point
  MainForm.cs              – Pilot lamp UI, IME polling, follow-focus, and hotkey logic
  MainForm.Designer.cs     – Designer-generated form setup
  SettingsForm.cs          – Settings dialog code-behind
  SettingsForm.Designer.cs – Settings dialog layout
  AppSettings.cs           – JSON settings persistence (%AppData%\ImePilotLamp\settings.json)
  NativeMethods.cs         – P/Invoke declarations (imm32.dll, user32.dll)
```
