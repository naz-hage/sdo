# NbuildTasks

`NbuildTasks` is the MSBuild task library used by SDO build targets. It is
included in the consolidated `sdo.dll` assembly.

## Available tasks

| Task | Use |
| --- | --- |
| `RedError` | Write an error message in red. |
| `ColorMessage` | Write a colored message. |
| `FileVersion` | Read a file version. |
| `WebDownload` | Download a file. |
| `UnZip` | Extract a ZIP archive. |
| `Zip` | Create a ZIP archive. |
| `Pwsh` | Run a PowerShell script. |

The tasks are registered by
[`common.targets`](../Sdo/Resources/common.targets).

Git operations are provided by the `sdo git` CLI; the legacy MSBuild `Git`
task has been removed. Built-in Git targets capture quiet CLI output into
MSBuild properties.

## Examples

Display a colored message:

```xml
<Target Name="BUILD_MESSAGE">
  <ColorMessage Message="Build complete" Color="Green" />
</Target>
```

Read a Git tag through the SDO CLI:

```xml
<Target Name="TAG">
  <Exec
    Command='"$(BuildTools)\sdo.exe" git tag get -q'
    ConsoleToMSBuild="true"
    IgnoreExitCode="true">
    <Output TaskParameter="ConsoleOutput" PropertyName="Tag" />
    <Output TaskParameter="ExitCode" PropertyName="TagExitCode" />
  </Exec>
  <Message Text="Tag: $(Tag)" Importance="high" />
</Target>
```

Download a file:

```xml
<Target Name="DOWNLOAD">
  <WebDownload
    WebUri="https://example.com/archive.zip"
    FileName="$(IntermediateOutputPath)archive.zip" />
</Target>
```

Run PowerShell:

```xml
<Target Name="SCRIPT">
  <Pwsh
    Script="$(SolutionDir)scripts\build\custom-script.ps1"
    Arguments="--example true"
    NoProfile="true"
    ExecutionPolicy="Bypass" />
</Target>
```

For publishing and installation workflows, use the
[sdo-scripts module](./sdo-scripts-module.md). For the targets that consume
these tasks, see the [target reference](./sdo-targets.md).
