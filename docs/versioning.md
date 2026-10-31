# Versioning and automation

SDO uses `X.Y.Z` versions for binaries, products, and repository tags:

| Part | Changes when |
| --- | --- |
| `X` | A breaking change is released. |
| `Y` | A feature or fix is released, or a production build is deployed. Incrementing `Y` resets `Z` to `0`. |
| `Z` | A staging build is deployed. |

Tags identify repository versions, associate release assets, and generate
release notes from commits since the previous tag. See
[buildtypes.md](./buildtypes.md) for staging and production builds and
[github-release.md](./github-release.md) for release behavior.

## Update tool versions

Tool versions are maintained in `dev-setup/apps.yaml`. Update the manifest
and a Markdown version table with:

```powershell
sdo tool manifest update --latest --file-path .\docs\sdo.md
```

The command discovers the latest stable releases, updates the YAML manifest,
and synchronizes matching tool/version rows in the selected Markdown file.
Use `--dry-run` to preview changes.

## Read the SDO version

```powershell
Import-Module "./scripts/module-package/sdo-scripts.psm1" -Force
$version = Get-VersionFromYaml -YamlPath "./dev-setup/sdo.yaml"
```

The Markdown synchronization is implemented by
`Sdo/Services/UpdateVersionsInDocs.cs` and invoked by
`sdo tool manifest update`.
