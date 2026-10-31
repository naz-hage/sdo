# Install and set up SDO

## Prerequisites

- 64-bit [Git for Windows](https://git-scm.com/download/win)
- [.NET SDK](https://dotnet.microsoft.com/download)
- PowerShell

## Install SDO

Clone the repository and install the latest stable SDO release:

```powershell
git clone https://github.com/naz-hage/sdo.git
Set-Location .\sdo
Import-Module .\scripts\module-package\sdo-scripts.psm1 -Force
Install-Sdo
```

Install a specific release with `-Version`:

```powershell
Install-Sdo -Version "1.74.0"
```

Verify the installation:

```powershell
sdo --help
sdo --version
```

For module functions and CI usage, see
[sdo-scripts-module.md](./sdo-scripts-module.md).

## Overview of dev-setup Folder

The dev-setup folder typically includes the following files:

- **`sdo.yaml`**  
    - Contains installation information for SDO. This file is required to install SDO before other development tools.
- **`apps.yaml`**  
    - Lists the development tools required for your project, including their installation and uninstallation details.
- **Legacy scripts** (deprecated in favor of sdo-scripts module)
- Individual PowerShell scripts for specific tasks

---

## File Details

### 1. sdo.yaml
This file provides the installation details for SDO, which is required to manage other tools in the project.

**Example:**
```yaml
Version: "1.2.0"
NbuildAppList:
  - Name: "sdo"
    Version: "1.7.0"
    AppFileName: "$(InstallPath)\\sdo.exe"
    WebDownloadFile: "https://github.com/naz-hage/sdo/releases/download/$(Version)/$(Name)-$(Version).zip"
    DownloadedFile: "$(Name)-$(Version).zip"
    InstallCommand: "powershell.exe"
    InstallArgs: "-Command Expand-Archive -Path $(DownloadedFile) -DestinationPath '$(InstallPath)' -Force"
    InstallPath: "$(ProgramFiles)\\sdo"
    UninstallCommand: "powershell.exe"
    UninstallArgs: "-Command Remove-Item -Path '$(InstallPath)' -Recurse -Force"
    StoredHash: "XXX"
    AddToPath: true
```

---

### 2. apps.yaml
This file lists all the development tools required for the project. Each tool is defined with its name, version, installation details, and uninstallation details.

**Example:**
```yaml
Version: "1.2.0"
NbuildAppList:
  - Name: "7-zip"
    Version: "23.01"
    AppFileName: "$(InstallPath)\\7z.exe"
    WebDownloadFile: "https://www.7-zip.org/a/7z2301-x64.exe"
    DownloadedFile: "7zip.exe"
    InstallCommand: "$(DownloadedFile)"
    InstallArgs: "/S /D=\"$(ProgramFiles)\\7-Zip\""
    InstallPath: "$(ProgramFiles)\\7-Zip"
    UninstallCommand: "$(InstallPath)\\Uninstall.exe"
    UninstallArgs: "/S"
```

**Key Elements in apps.yaml:**

| Element Name       | Description                                                                 |
|--------------------|-----------------------------------------------------------------------------|
| `Name`            | The name of the tool.                                                      |
| `Version`         | The version of the tool.                                                   |
| `AppFileName`     | The file name of the tool, used to check if it is already installed.        |
| `WebDownloadFile` | The URL to download the tool.                                               |
| `LatestVersionUrl` | (Optional) URL used by `tool list` to discover the latest stable release when the download URL is not a GitHub release URL. |
| `LatestVersionStrategy` | (Optional) Provider-specific discovery strategy, such as `github-tags`, `mongodb-download-page`, or `docker-appcast`. |
| `DownloadedFile`  | The name of the downloaded file, used for installation.                    |
| `InstallCommand`  | The command to install the tool.                                            |
| `InstallArgs`     | The arguments for the installation command.                                 |
| `InstallPath`     | The location where the tool will be installed.                              |
| `UninstallCommand`| The command to uninstall the tool.                                          |
| `UninstallArgs`   | The arguments for the uninstallation command.                               |
| `UpdateInPlace`   | (Optional) Updates the tool without uninstalling the existing installation first. Use this for installers that support in-place upgrades, such as Docker Desktop. |
| `StoredHash`      | (Optional) SHA256 hash of the file for verification and version reporting when the file has no version metadata. |
| `AddToPath`       | (Optional) Whether to add the tool's path to the system PATH environment variable. |

`LatestVersionUrl` is not required for normal installation or for tools that
use a pinned manifest version. The `tool list` command can discover a release
automatically when `WebDownloadFile` is a GitHub release download URL.
Add `LatestVersionUrl` only when the download host is different from the
release host, such as Helm:

```yaml
- Name: "Helm"
  Version: "3.21.3"
  WebDownloadFile: "https://get.helm.sh/helm-v$(Version)-windows-amd64.zip"
  LatestVersionUrl: "https://github.com/helm/helm"
```

In this example, Helm releases are discovered from GitHub while the actual
archive continues to download from `get.helm.sh`. The value should identify
the GitHub repository (`https://github.com/OWNER/REPOSITORY`); it is not
required to include a version or release tag.

---

### 3. `dev-setup.ps1`
This PowerShell script automates the installation of tools and sets up the development environment.

**Key Responsibilities:**

- Installs the latest stable SDO release ending in `.0`.
- Installs other tools listed in apps.yaml  .
- Verifies administrative privileges before proceeding.

**Example:**
```powershell
# Import the install module
$url = "https://raw.githubusercontent.com/naz-hage/sdo/main/dev-setup/install.psm1"
$output = "./install.psm1"
Invoke-WebRequest -Uri $url -OutFile $output
Import-Module ./install.psm1 -Force

# Install SDO
Install-Sdo
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage "Error: Installation of SDO failed. Exiting script."
    exit 1
}

# Install other tools
& $global:NbExePath install --manifest .\apps.yaml
if ($LASTEXITCODE -ne 0) {
    Write-OutputMessage "Error: Installation of other tools failed. Exiting script."
    exit 1
}

Write-OutputMessage "Completed installation script."
```

---

## Folder Structure

Your project folder should look like this:
```plaintext
%MainDirectory%\
├── MyProject\
│   ├── dev-setup\
│   │   ├── sdo.yaml
│   │   ├── apps.yaml
│   │   ├── dev-setup.ps1
│   ├── ... other project and test files
│   └── sdo.targets  (this file is required in the solution folder)
```

---

## Adding a New Development Tool

To add a new tool to your project:
1. Identify the tool's:
   - Download URL and file name.
   - Installation and uninstallation commands and arguments.
   - Installation path.
   - File name for version checks.
   - Version and name.
2. Add the tool's details to apps.yaml.

**Example for a new tool:**
```yaml 
Name: "Docker"
Version: "4.38.0.0"
AppFileName: "$(InstallPath)\\Docker Desktop.exe"
WebDownloadFile: "https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe"
DownloadedFile: "Docker Desktop Installer.exe"
InstallCommand: "$(DownloadedFile)"
InstallArgs: "install --quiet"
InstallPath: "$(ProgramFiles)\\Docker\\Docker"
UninstallCommand: "powershell.exe"
UninstallArgs: "-Command \"Remove-Item -Path '$(InstallPath)' -Recurse -Force\""
AddToPath: true
```
