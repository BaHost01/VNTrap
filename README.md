# VNTrap

**VNTrap** is a Windows launcher for Roblox built around fast startup, client configuration, diagnostics, and power-user controls.

The project is based on an existing Bloxstrap/Fishstrap codebase and is being progressively refactored into its own launcher while retaining useful launcher functionality.

> [!NOTE]
> VNTrap currently targets **Windows 10 and newer** and uses **.NET 6 / WPF**.

## Highlights

- Roblox Player and Roblox Studio launching
- FastFlags and Global Basic Settings management
- Roblox channel switching
- Cache and log cleanup tools
- Server information through the existing RoValra integration
- Custom integrations
- Local crash diagnostics
- Optional Windows process performance tuning
- Best-effort multi-client launching
- WPF-UI based launcher interface
- Bootstrapper and update workflow

## Launching

VNTrap can be launched normally through the application UI or with command-line arguments.

### Player

```powershell
VNTrap.exe -player
```

### Performance mode

```powershell
VNTrap.exe -player -performance
```

Performance mode applies Windows process scheduling settings to running Roblox Player processes. It does **not** modify Roblox binaries, inject code, or guarantee an FPS increase.

### Multiple clients

```powershell
VNTrap.exe -player -multi 2
VNTrap.exe -player -multi 4
VNTrap.exe -player -multi 8 -performance
```

`-multi` accepts **2–8 requested clients**. VNTrap starts the normal client and then attempts to start the additional processes.

> [!IMPORTANT]
> Multi-client support is intentionally best-effort. The installed Roblox client can enforce its own single-instance behavior, so VNTrap cannot guarantee that every requested client will remain active.

## Diagnostics

VNTrap includes application-level exception handling and local crash reporting.

Crash reports are stored in the application's local log directory and are intended to provide useful information when troubleshooting crashes. Reports are **not automatically uploaded** by VNTrap.

## Project structure

The current source tree still contains the historical `Bloxstrap` project directory and namespace for compatibility with the existing codebase. The generated application assembly/product identity is **VNTrap**.

```text
VNTrap/
├── Bloxstrap/              # Main WPF application source
│   ├── Roblox/             # Roblox launch/performance helpers
│   ├── UI/                 # WPF-UI views and view models
│   ├── AppData/            # Launcher/application data
│   └── ...
├── .github/workflows/      # CI and release automation
├── wpfui/                  # WPF-UI dependency
├── Bloxstrap.sln
└── README.md
```

## Development

### Requirements

- Windows 10 or newer
- .NET 6 SDK
- Git
- Visual Studio 2022 or another compatible .NET/WPF development environment

### Build

```powershell
dotnet restore
dotnet build .\Bloxstrap\Bloxstrap.csproj -c Release
```

The resulting application assembly is named **`VNTrap.exe`**.

## CI

The repository includes GitHub Actions workflows for debug and release builds.

CI is responsible for restoring dependencies, building/publishing the WPF application, and producing VNTrap artifacts for the corresponding workflow.

## Project status

VNTrap is under active refactoring. Current work is focused on:

- separating VNTrap identity from the original upstream branding
- improving launcher reliability and diagnostics
- keeping the UI responsive and lightweight
- improving Roblox launch workflows
- gradually modernizing the underlying project structure

Some behavior remains dependent on the installed Roblox client and Windows environment.

## Attribution

VNTrap contains code and structure derived from the project's upstream launcher codebase. Existing licenses, attribution notices, and third-party licenses remain applicable.

See the repository license files for the complete licensing information.
