# Build types

SDO provides two standard build targets:

- `stage` creates a package for testing.
- `prod` creates a package for release.

## Stage

```powershell
sdo stage
```

The stage target cleans the solution, updates the stage version tag, builds
and tests the solution, copies the artifacts, and creates the package.

## Production

```powershell
sdo prod
```

The production target cleans the solution, pulls the latest changes, updates
the production version tag, builds and tests the solution, copies the
artifacts, and creates the package.

Version increments are documented in [versioning.md](./versioning.md).
GitHub release behavior is documented in
[github-release.md](./github-release.md).

Projects can add custom targets in `sdo.targets`. See the
[target reference](./sdo-targets.md) for the available build targets.
