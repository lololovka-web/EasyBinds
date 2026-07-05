[Setup]
AppName=EasyBinds
AppVersion=1.0
AppPublisher=EasyBinds
DefaultDirName={autopf}\EasyBinds
DefaultGroupName=EasyBinds
OutputDir=Output
OutputBaseFilename=EasyBinds-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "bin\Release\net10.0-windows\EasyBinds.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\net10.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\EasyBinds"; Filename: "{app}\EasyBinds.exe"
Name: "{group}\{cm:UninstallProgram,EasyBinds}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\EasyBinds"; Filename: "{app}\EasyBinds.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\EasyBinds.exe"; Description: "{cm:LaunchProgram,EasyBinds}"; Flags: nowait postinstall skipifsilent
