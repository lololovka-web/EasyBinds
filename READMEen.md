# EasyBinds

Global hotkey manager for Windows 10/11.

## Description

EasyBinds lets you assign global hotkeys that work anywhere in the system. The app minimizes to the system tray and stays out of your way.

## Features

- Assign hotkeys to various actions
- Launch programs with a hotkey
- Close the active window
- Run command-line commands
- Open websites
- Show message boxes
- Toggle system mute/unmute
- Run at Windows startup
- Switch between dark and light themes
- Switch between Russian and English languages
- Minimize to system tray

## Installation

### From source

```
dotnet build
```

Binary: `bin\Debug\net10.0-windows\EasyBinds.exe`

### Via InnoSetup

1. Install [Inno Setup](https://jrsoftware.org/isinfo.php)
2. Open the `installer.iss` file
3. Click Build > Compile
4. Run the created installer

## Usage

1. Run `EasyBinds.exe`
2. Click "Add Binding"
3. Press the desired key combination
4. Select an action from the list
5. Set a parameter (file path, URL, command, etc.)
6. Click OK

## Hotkeys

The app captures global key combinations via Win32 API. Combinations work even when the window is minimized.

## Requirements

- Windows 10/11
- .NET 10 Runtime (for running from source)

## Technologies

- C# / WPF (.NET 10)
- Win32 API (RegisterHotKey, PostMessage)
- System.Text.Json for settings persistence
