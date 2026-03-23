# ime-pilot-lamp-indicator

A lightweight Windows desktop application that shows the current **IME (Input Method Editor) state** — **ON** or **OFF** — as a glowing pilot-lamp indicator.

This application aims to reduce the frustration of starting to type, only to realise that the IME is either on or off.

Ideally, you would be able to see whether the IME is on or off right at the text input cursor or nearby.
Unfortunately, however, this is not possible in Windows unless the individual applications implement this functionality themselves.

This app doesn't resolve the issue either. However, it has a more intuitive pilot lamp than the IME toolbar,
as well as an optional feature that moves the lamp to near the mouse pointer  (unfortunately not the text input cursor) when the active window changes.

---

現在の **IME（Input Method Editor）の状態**（ON / OFF）を光るパイロットランプとして表示する、軽量な Windows デスクトップアプリです。

IME がオンなのかオフなのか気づかずに入力を始めてしまう、あの煩わしさを少しでも減らすことを目的としています。

テキスト入力カーソルかその周辺で IME の状態を確認できれば理想的です。
しかし残念ながら、Windows では個々のアプリ自体がその機能を実装していない限り、それは叶いません。

このアプリは、その問題を根本的に解決するものではありませんが、IME ツールバーよりも直感的なパイロットランプと、
アクティブウィンドウが切り替わったときにランプをマウスポインター（残念ながらテキスト入力カーソルではなく）の近くへ移動するオプション機能を提供します。

## Features

- **Always-on-top** borderless mini-window, positioned in the top-right corner by default; position is saved between sessions
- **Green glowing lamp** when IME is ON (e.g. Japanese/Chinese/Korean input active)
- **Dim gray lamp** when IME is OFF (Latin / direct input)
- **System tray icon** that also reflects the current state
- **IME toggle**: left-click the tray icon to toggle IME ON/OFF in the currently active application
- **Global hotkey**: `Ctrl+Alt+L` toggles the indicator window visibility
- **Draggable**: left-click and drag the window anywhere on screen
- **Follow-focus** (optional): automatically moves the indicator near the mouse cursor whenever the active window changes (mouse click, Alt+Tab, taskbar, etc.)
- **Fade-out** (optional): after a follow-focus move, the window gradually fades out over a configurable duration (0–3 s)
- **Customizable background**: choose any background color, or enable transparent mode to show only the lamp and label with no background
- **DPI-aware**: scales correctly on high-DPI / multi-monitor setups
- **Non-intrusive**: never steals keyboard focus (`WS_EX_NOACTIVATE`)
- **Polls every 100 ms** using the Windows IME API (`ImmGetDefaultIMEWnd` + `WM_IME_CONTROL / IMC_GETOPENSTATUS`)

## 機能

- **常に最前面**に表示されるボーダーレスのミニウィンドウ。デフォルトでは画面右上に配置され、位置はセッション間で保存されます
- IME が **ON**（日本語・中国語・韓国語入力など）のとき：**緑色の光るランプ**
- IME が **OFF**（英数字入力）のとき：**暗いグレーのランプ**
- 現在の状態を反映する**システムトレイアイコン**
- **IME 切り替え**：トレイアイコンを左クリックすると、現在アクティブなアプリの IME をオン/オフ切り替え
- **グローバルホットキー**：`Ctrl+Alt+L` でインジケーターウィンドウの表示・非表示を切り替え
- **ドラッグ移動**：ウィンドウを左クリックしてドラッグし、画面上の任意の位置へ移動
- **フォローフォーカス**（オプション）：マウスクリック・Alt+Tab・タスクバーなどでアクティブウィンドウが切り替わるたびに、インジケーターをマウスカーソルの近くへ自動移動
- **フェードアウト**（オプション）：フォローフォーカスで移動した後、設定した時間（0〜3 秒）をかけてウィンドウが徐々に透明になります（0 = 無効）
- **背景色のカスタマイズ**：カラーピッカーで任意の背景色を設定するか、透明モードにしてランプとラベルだけを表示
- **DPI 対応**：高 DPI・マルチモニター環境でも正しくスケーリング
- **不干渉**：キーボードフォーカスを奪わない（`WS_EX_NOACTIVATE`）
- Windows IME API（`ImmGetDefaultIMEWnd` + `WM_IME_CONTROL / IMC_GETOPENSTATUS`）を使用し、**100 ms ごとにポーリング**

## Screenshots

| IME OFF | IME ON |
|---------|--------|
| ![IME OFF – dim gray lamp](docs/ime-off.png) | ![IME ON – green glowing lamp](docs/ime-on.png) |

## Requirements

- Windows 10 / 11 (64-bit)
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (or newer)

## 動作環境

- Windows 10 / 11（64 ビット）
- [.NET 8 デスクトップランタイム](https://dotnet.microsoft.com/ja-jp/download/dotnet/8.0)（またはそれ以降）

## Build

```bash
cd ImePilotLamp
dotnet build -c Release
```

The compiled executable is placed in `ImePilotLamp/bin/Release/net8.0-windows/`.

## ビルド

```bash
cd ImePilotLamp
dotnet build -c Release
```

コンパイルされた実行ファイルは `ImePilotLamp/bin/Release/net8.0-windows/` に出力されます。

## Publish (single-file, self-contained)

```bash
cd ImePilotLamp
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## パブリッシュ（単一ファイル・自己完結型）

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
| Follow-focus | Move the indicator near the cursor whenever the active window changes |
| Fade-out | Duration (0–3 s) before the indicator fades out after a follow-focus move; 0 = disabled |
| Background color | Choose a custom background color with the color picker |
| Transparent background | Remove the background entirely; only the lamp and label are visible |

Settings are saved automatically to `%AppData%\ImePilotLamp\settings.json`.

## 使い方

`ImePilotLamp.exe` を実行します。パイロットランプが画面右上に表示されます。

| 操作 | 動作 |
|------|------|
| ランプウィンドウを左クリックしてドラッグ | インジケーターウィンドウを移動 |
| トレイアイコンを左クリック | アクティブなアプリの IME をオン/オフ切り替え |
| トレイアイコンを右クリック | コンテキストメニューを開く |
| `Ctrl+Alt+L` | インジケーターウィンドウの表示・非表示を切り替え |

### コンテキストメニュー（トレイアイコンを右クリック）

| 項目 | 動作 |
|------|------|
| 表示 / 非表示 (Ctrl+Alt+L) | ウィンドウの表示・非表示を切り替え |
| 設定… | 設定ダイアログを開く |
| 終了 | アプリを終了 |

### 設定ダイアログ

| 設定項目 | 説明 |
|----------|------|
| フォローフォーカス | アクティブウィンドウが切り替わるたびにインジケーターをカーソル近くへ移動 |
| フェードアウト | フォローフォーカスで移動後、インジケーターがフェードアウトするまでの時間（0〜3 秒）。0 = 無効 |
| 背景色 | カラーピッカーで任意の背景色を選択 |
| 透明背景 | 背景を完全に非表示にし、ランプとラベルのみ表示 |

設定は `%AppData%\ImePilotLamp\settings.json` に自動保存されます。

## How it works

1. A `System.Windows.Forms.Timer` fires every 100 ms.
2. The current foreground window handle is obtained via `GetForegroundWindow()`.
3. The IME window for that handle is resolved with `ImmGetDefaultIMEWnd()`.
4. A `WM_IME_CONTROL / IMC_GETOPENSTATUS` message is sent to the IME window.
5. A non-zero return value means IME is open (ON); zero means closed (OFF).
6. The indicator repaints only when the state actually changes.

When **follow-focus** is enabled, any foreground window change (mouse click, Alt+Tab, taskbar click, etc.) moves the indicator above the current cursor position, as long as the new window is not the desktop or taskbar. A configurable fade-out timer then gradually reduces the window's opacity.

## 動作の仕組み

1. `System.Windows.Forms.Timer` が 100 ms ごとに発火します。
2. `GetForegroundWindow()` で現在のフォアグラウンドウィンドウのハンドルを取得します。
3. そのハンドルに対応する IME ウィンドウを `ImmGetDefaultIMEWnd()` で解決します。
4. IME ウィンドウへ `WM_IME_CONTROL / IMC_GETOPENSTATUS` メッセージを送信します。
5. 戻り値が 0 以外なら IME はオープン（ON）、0 ならクローズ（OFF）です。
6. 状態が実際に変化したときのみインジケーターを再描画します。

**フォローフォーカス**が有効な場合、フォアグラウンドウィンドウの変化（マウスクリック・Alt+Tab・タスクバークリックなど）が発生すると、新しいウィンドウがデスクトップやタスクバーでない限り、インジケーターを現在のカーソル位置の上へ移動します。その後、設定可能なフェードアウトタイマーによってウィンドウの不透明度が徐々に下がります。

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

## プロジェクト構成

```
ImePilotLamp/
  ImePilotLamp.csproj      – WinForms プロジェクト (net8.0-windows)
  Program.cs               – アプリケーションエントリーポイント
  MainForm.cs              – パイロットランプ UI、IME ポーリング、フォローフォーカス、ホットキーのロジック
  MainForm.Designer.cs     – デザイナー生成のフォームセットアップ
  SettingsForm.cs          – 設定ダイアログのコードビハインド
  SettingsForm.Designer.cs – 設定ダイアログのレイアウト
  AppSettings.cs           – JSON 設定の永続化（%AppData%\ImePilotLamp\settings.json）
  NativeMethods.cs         – P/Invoke 宣言（imm32.dll、user32.dll）
```
