- **C:\source\ntools\sdo.targets Targets**

| **Target Name** | **Description** |
| --- | --- |
| SETUP_ENVIRONMENT   | Setup development environment by importing sdo-scripts and calling Set-DevelopmentEnvironment |
| TEST_NTOOLS_SCRIPTS | Test that sdo-scripts module is installed and can report a version |
| MKDOCS              | Build docs locally for testing |
| RUN_NBTESTS_COVERAGE | Run nbTests and generate code coverage report |
| MKDOCS_DEPLOY       | mkdocs deploy locally with live reload fixes |
| INSTALL_PYTHON_TEST_DEPS | Install python test dependencies for atools and run pytest (verbose) |
| RUN_PYTESTS_VERBOSE | Run Python pytest for atools package (verbose) |
| RUN_SDO_TESTS       | Run SDO tests specifically |
| GET_PRODUCT_CODES   | Example to get the installation Product code used for uninstallation of product |
| CORE                | Display core properties |
| BUILD               | Build the solution alias solution target |
| CHECK_GITHUB_KEY    | Check for API_GITHUB_KEY environment variable and print its length |
| UPDATE_NTOOLS       | Update ntools locally for testing |
| NUGET_UPDATE        | Update the sdo-launcher nuget package in the local feed for testing - not needed for normal builds |
| YELLOW_MESSAGE      | Example of a target that displays a yellow color message |
| RED_MESSAGE         | Example of a target that displays a red color message |
| INSTALL_DOTNET_OUTDATED_TOOL | Install dotnet-outdated-tool globally |
| UPDATE_NUGET_PACKAGES | Update all NuGet packages to the latest version |
| LIST_NUGET_SOURCES  | List all NuGet sources |
| GENERATE_COMMIT_MESSAGE | Intelligent commit message generation |
| GIT_COMMIT_INFRASTRUCTURE | Automated Git commit with intelligent message generation |
| UPDATE_AND_COMMIT   | Combined target: Update versions and commit with smart message |
| INFRASTRUCTURE_COMMIT | Full infrastructure update and commit with intelligent analysis |
| PREVIEW_COMMIT_MESSAGE | Preview commit message without committing |
| INSTALL_REPORTGENERATOR | Install ReportGenerator tool globally |
| GITHUB_RELEASE      | Creates a stage or prod release Add for testing and remove after success |
| GITHUB_PRE_RELEASE  | Creates a stage or prod pre-release Add for testing and remove after success |
| RUN_RELEASESERVICEFACTORYTESTS | Run the focused ReleaseServiceFactoryTests via dotnet test |
| NB_PRECHECK         | Pre-check: ensure no machine-level Debug/Release folders are present before running tests |
| NB_TEST             | Run the full solution test suite (used by 'nb test') |
| NB_TEST_DIAGNOSTICS | Diagnostic target to run automatically after NB_TEST to list TestFramework DLLs |
| NB_DIAG_NbuildTasksTests | Targets auto-generated: one per Tests.csproj in the solution |
| NB_DIAG_NbuildTests |  |
| NB_DIAG_lfTests     |  |
| NB_DIAG_nbTests     |  |
| NB_DIAG_nBackupTests |  |
| NB_DIAG_GitHubReleaseTests |  |
| NB_TEST_DIAG_ALL    | Aggregate target: run all per-project diagnostics sequentially |
| STAGE               | Create a stage package for testing |
| PROD                | Create a PROD package for release |
| SMOKE_TEST          | Run the published `sdo.exe --version` check |
| TEST                | This prevents double test execution that causes conflicts |

`PROPERTIES` initializes the artifact, test-results, and coverage output folders.
`PUBLISH`, tests, coverage, and packaging use these paths directly; the deprecated
`ARTIFACTS` and `COPY_ARTIFACTS` targets are no longer required.


- **C:\Program Files\Nbuild\common.targets Targets**

| **Target Name** | **Description** |
| --- | --- |
| PROPERTIES          | Common properties that will be used by all targets |
| CLEAN               | Clean up the project and artifacts folder |
| TELEMETRY_OPT_OUT   | Opt out of the DOTNET_CLI_TELEMETRY_OPTOUT - move to common |
| DEV                 | Create a development package for testing without incrementing the version |
| STAGE               | Create a stage package for testing |
| PROD                | Create a PROD package for release |
| GITHUB_RELEASE      | Creates a prod release |
| GITHUB_PRE_RELEASE  | Creates a pre-release |
| SOLUTION            | Build the solution Release configuration  using dotnet build |
| PACKAGE             | Create a package for the solution default is a zip file of all artifacts |
| DEPLOY              | Deploy the package. default is to extract artifacts into DeploymentProperty folder |
| TEST                | Run all tests using dotnet test in Release mode |
| PUBLISH             | Publish all non-test projects to one versioned artifacts folder without PDB files |
| COVERAGE            | Generate code coverage reports |
| COVERAGE_SUMMARY    | Display high-level code coverage summary |
| IS_ADMIN            | Check if current process is running in admin mode AdminCheckExitCode property is set |
| SingleProject       | Example how to build a single project |
| HandleError         | Error handling placeholder |


## PUBLISH output

`PUBLISH` uses `dotnet publish` for each non-test project. All projects use the
Release configuration, `$(ProductVersion)`, and the same output folder:
`$(ArtifactsFolder)`.

Test projects are excluded from the project list. Debug symbols are disabled,
and any `.pdb` files remaining in the output folder are removed before the
target completes.

After stage or production auto-tagging, the default artifacts, test-results, and
coverage properties are updated to use the generated `ProductVersion`.
Test-results and coverage paths remain under the solution directory in GitHub
Actions and under the artifacts solution folder otherwise.

- **C:\Program Files\nbuild\ngit.targets Targets**

| **Target Name** | **Description** |
| --- | --- |
| AUTOTAG_STAGE       | Increment version for a stage build |
| SET_TAG             | Set version for a stage build |
| AUTOTAG_PROD        | Increment version for a production build |
| TAG                 | Get the tag from git |
| PUSH_TAG            | Push the tag to the remote repo |
| GIT_BRANCH          | Get the current git branch |

The built-in Git targets invoke `sdo git` commands with `-q` and capture their
single-line output into MSBuild properties. Stage and production auto-tagging
uses `sdo git tag auto --dry-run -q` to calculate `ProductVersion`, then
`SET_TAG` runs `sdo git tag set` to create the tag locally. `PUSH_TAG` and the
GitHub release targets in `common.targets` and `sdo.targets` run
`sdo git tag push` to publish an existing tag without replacing it. The legacy
MSBuild `Git` task has been removed.

- **unit-tests.targets Targets**

| **Target Name** | **Description** |
| --- | --- |
| UNIT_TEST_ALL | Run all SDO unit tests |
