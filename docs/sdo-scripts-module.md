# sdo-scripts PowerShell module

The `sdo-scripts` module provides the PowerShell functions used by SDO build
and CI workflows.

Source:

```text
scripts/module-package/sdo-scripts.psm1
```

## Quick start

Import the module from a repository checkout:

```powershell
Import-Module .\scripts\module-package\sdo-scripts.psm1 -Force
Get-SdoScriptsVersion
Get-Command -Module sdo-scripts | Sort-Object Name
```

Install the latest stable SDO release:

```powershell
Install-Sdo
```

When no version is supplied, `Install-Sdo` queries the GitHub releases API.
The module uses `API_GITHUB_KEY`, then `GITHUB_TOKEN`, and finally a token
from `gh auth token` when GitHub CLI is authenticated. In GitHub Actions, the
workflow passes its repository token to `API_GITHUB_KEY` automatically.

Install a specific release:

```powershell
Install-Sdo -Version "1.29.7" -DownloadsDirectory "C:\MyDownloads"
```

The module is also installed by the SDO build and GitHub Actions workflows.

## Exported functions

The public API is defined by `FunctionsToExport` in
`scripts/module-package/sdo-scripts.psd1`.

| Function | Purpose |
| --- | --- |
| `Get-SdoScriptsVersion` | Return the module version. |
| `Set-DevelopmentEnvironment` | Set local development environment variables. |
| `Get-VersionFromYaml` | Read version fields from an `sdo.yaml` file. |
| `Write-TestResult` | Write a standardized test result. |
| `Test-TargetExists` | Check whether an MSBuild target exists. |
| `Test-TargetDependencies` | Validate MSBuild target dependencies. |
| `Test-TargetDelegation` | Verify target delegation used by SDO. |
| `Get-FileHash256` | Compute a SHA-256 file hash. |
| `Get-FileVersionInfo` | Read file and product version metadata. |
| `Invoke-FastForward` | Fast-forward a Git ref. |
| `Write-OutputMessage` | Write consistently formatted output. |
| `Get-SdoFileVersion` | Read the SDO product version from a binary. |
| `Add-DeploymentPathToEnvironment` | Add a deployment path to `PATH`. |
| `Invoke-SdoDownload` | Download an SDO release artifact. |
| `Get-LatestSdoReleaseVersion` | Find the latest stable SDO release. |
| `Install-Sdo` | Install an SDO release. |

View the installed commands at any time:

```powershell
Get-Command -Module sdo-scripts
```

## Build and CI usage

For direct project publishing, use the .NET CLI:

```powershell
dotnet publish .\Sdo\Sdo.csproj `
  --configuration Release `
  --output C:\Artifacts `
  --property:Version=1.0.0 `
  --property:DebugSymbols=false `
  --property:DebugType=None
```
The `PUBLISH` target invokes `dotnet publish` once per non-test project and
writes every project to the same versioned output folder. The `SMOKE_TEST`
target uses `Test-TargetDelegation`.

Example GitHub Actions setup:

```yaml
- name: Install SDO
  shell: pwsh
  run: |
    Import-Module ./scripts/module-package/sdo-scripts.psm1 -Force
    Install-Sdo
```

## Development

1. Add or update a function in `sdo-scripts.psm1`.
2. Update `FunctionsToExport` in `sdo-scripts.psd1`.
3. Update this document.
4. Run the module tests:

```powershell
.\scripts\module-package\test-module.ps1
```

## Troubleshooting

Check that the source module exists:

```powershell
Test-Path .\scripts\module-package\sdo-scripts.psm1
```

Check the installed module and version:

```powershell
Get-Module sdo-scripts -ListAvailable
Import-Module .\scripts\module-package\sdo-scripts.psm1 -Force
Get-SdoScriptsVersion
```

For MSBuild task usage, see [NbuildTasks](./nbuildtasks.md). For build target
details, see [sdo-targets](./sdo-targets.md).
