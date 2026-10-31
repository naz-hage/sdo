# GitHub releases

SDO creates GitHub releases from the packages produced by the `stage` and
`prod` build targets. See [buildtypes.md](./buildtypes.md) for package
creation and [sdo-net.md](./sdo-net.md) for the complete CLI reference.

## Release targets

Run these targets from a GitHub Actions workflow:

```powershell
sdo GITHUB_PRE_RELEASE
sdo GITHUB_RELEASE
```

- `GITHUB_PRE_RELEASE` publishes the stage package as a prerelease.
- `GITHUB_RELEASE` publishes the production package as a release.

Release packages use this name:

```text
$(SolutionName)-$(ProductVersion).zip
```

Production releases remove older stage releases. Release notes are generated
from commits since the previous matching tag.

## Authentication

Set `API_GITHUB_KEY` in GitHub Actions or in the local PowerShell session:

```powershell
$env:API_GITHUB_KEY = "ghp_..."
```

GitHub CLI authentication is also supported locally:

```powershell
gh auth login
gh auth status
```

For GitHub Actions:

```yaml
- name: Create release
  shell: pwsh
  run: sdo GITHUB_RELEASE
  env:
    API_GITHUB_KEY: ${{ secrets.API_GITHUB_KEY }}
    OWNER: ${{ github.repository_owner }}
```

Public repositories need authentication for write operations. Private
repositories need authentication for all operations. The token must be able to
read repository metadata and create releases and assets. For a classic token,
use the `repo` scope.

## Private release assets

Tool manifests are YAML files. Reference a GitHub release asset with its
download URL:

```yaml
WebDownloadFile: "https://github.com/OWNER/REPOSITORY/releases/download/TAG/asset.zip"
```

If a private asset URL returns `404`, SDO retries through the GitHub API using
the configured token. Verify the repository, tag, asset name, and credentials.
