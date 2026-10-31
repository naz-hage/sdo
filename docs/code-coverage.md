# Code coverage

SDO collects .NET test coverage and generates HTML, Cobertura, and text
reports through its build targets.

## Run coverage

Run tests and collect coverage:

```powershell
sdo TEST
```

Generate reports from the test results:

```powershell
sdo COVERAGE
```

Display the text summary:

```powershell
sdo COVERAGE_SUMMARY
```

`COVERAGE` requires test results. Run `sdo TEST` first unless a stage build
has already run the tests. `sdo STAGE` includes testing, report generation,
and the coverage summary.

## Configure coverage

Coverage is enabled by default. Set these MSBuild properties in the project
file to change the behavior:

```xml
<PropertyGroup>
  <EnableCodeCoverage>true</EnableCodeCoverage>
  <CoverageAssemblyFilters>+MyProject*;-*Tests*;-*Mock*</CoverageAssemblyFilters>
  <CoverageClassFilters>+*;-*.Tests.*;-*.Mocks.*</CoverageClassFilters>
</PropertyGroup>
```

Filters use `+` to include, `-` to exclude, `*` as a wildcard, and semicolons
to separate patterns.

Disable coverage for a faster test run:

```powershell
sdo TEST -p:EnableCodeCoverage=false
```

## Output

The report location is the configured `CoverageFolder`, normally:

```text
<artifacts>\CoverageReport\<version>\
```

Open `index.html` for the detailed report. `Summary.txt` contains the text
summary, and `coverage.cobertura.xml` is copied to the repository root for CI
workflows.

Report generation installs the
`dotnet-reportgenerator-globaltool` tool when it is not already available.
