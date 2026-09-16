# VNTrap

VNTrap is a Windows Roblox launcher focused on a fast startup experience, configurable client behavior, diagnostics, and useful power-user tooling.

> [!NOTE]
> VNTrap currently targets Windows 10 and newer.

## Features

- Roblox Player and Roblox Studio support
- FastFlags and Global Basic Settings editors
- Channel switching
- Cache and log cleanup
- Detailed server information through the existing RoValra integration
- Custom integrations
- Crash diagnostics and local crash reports
- Performance launch mode
- Multi-client launch mode
- Mica-based WPF interface with WPF-UI
- Automatic update and bootstrapper workflow

## Multi-client mode

VNTrap supports a best-effort multi-client launcher through command-line flags:

```text
VNTrap.exe -player -multi 2
VNTrap.exe -player -multi 4 -performance
```

The supported range is 2 to 8 requested clients. VNTrap starts the primary client normally and then requests the additional Roblox processes. Whether multiple clients can actually remain active is ultimately determined by the installed Roblox client and its current single-instance behavior.

## Performance mode

```text
VNTrap.exe -player -performance
```

Performance mode applies a conservative Windows scheduling adjustment to active `RobloxPlayerBeta.exe` processes. It does not modify Roblox binaries, inject code, or guarantee a higher FPS.

## Diagnostics

Unhandled application exceptions are recorded locally under the application's log directory. Crash reports contain diagnostic information useful for troubleshooting and are not uploaded automatically.

## Development

The application is a WPF project using .NET 6, CommunityToolkit.Mvvm, WPF-UI and several existing integrations. The project is gradually being separated from its original upstream structure while preserving the launcher functionality that is still useful to VNTrap.

## Building

```powershell
dotnet restore
dotnet build .\Bloxstrap\Bloxstrap.csproj -c Release
```

The release assembly is `VNTrap.exe`.

## License

See the repository's license and upstream attribution files for licensing information.
