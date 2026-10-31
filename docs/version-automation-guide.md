# Version Automation Guide

This document outlines the automation solutions implemented to keep the SDO developer tools documentation synchronized with version information from YAML configuration files in the `dev-setup/` directory.

The SDO project maintains tool version information in two places:
1. **YAML Configuration Files** (`dev-setup/*.yaml`) - Used for automated installation
2. **Documentation Table** (`docs/sdo.md`) - User-facing version reference

Previously, these had to be updated manually, leading to inconsistencies and outdated documentation.

## Solutions Implemented

---

## 1. PowerShell Module Integration (v3.0.0+)

- **Module**: `scripts/module-package/sdo-scripts.psm1`
- **Function**: `Get-VersionFromYaml`
- **Purpose**: Consolidated version management within the SDO scripts module
- **Integration**: Available in all build processes and CI/CD pipelines

### Using the Module Approach
```powershell
# Import the module
Import-Module "./scripts/module-package/sdo-scripts.psm1" -Force

# Update documentation with latest versions
# (Handled by MSBuild target)

# Get version from the SDO YAML manifest
$version = Get-VersionFromYaml -YamlPath "./dev-setup/sdo.yaml"
```

---

Tool versions in documentation can be updated through `sdo tool manifest update --latest --file-path .\docs\sdo.md`. This updates the manifest from the latest stable releases, then extracts all tool/version pairs from `dev-setup/apps.yaml` and updates the documentation table. See the documentation in `sdo.md` for details.

### Service Integration

- **File**: `Sdo/Services/UpdateVersionsInDocs.cs`
- **Purpose**: Service for updating Markdown tool version tables
- **Execution**: Invoked by `sdo tool manifest update`

### Features
- Native C# service implementation
- YAML and JSON manifest support
- Regex-based markdown table updating
- Comprehensive tool name mapping logic
- Explicit command error handling

### Usage
```powershell
sdo tool manifest update --latest --file-path .\docs\sdo.md
```
