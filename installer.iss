#define MyAppName "Tovia"
#define MyAppVersion "2.0.0"
#define MyAppExeName "Tovia.exe"
#define MyAppPublisher "Nick Solomonov"
#define MyAppURL "https://https://github.com/NickSlm/Tovia"

[Setup]
AppId={{A1D4A9D2-7E4F-4D6B-BB91-3C9D6E0F9E2C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}

OutputBaseFilename=Tovia-Setup-{#MyAppVersion}
OutputDir=dist

Compression=lzma2
SolidCompression=yes
WizardStyle=modern

DisableProgramGroupPage=yes
PrivilegesRequired=lowest

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "src\Tovia\bin\Release\net8.0-windows\win-x64\publish\*"; \
    DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Tovia"; Flags: nowait postinstall skipifsilent
