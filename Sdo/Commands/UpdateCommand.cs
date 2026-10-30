using System.CommandLine;
using Launcher.Services;
using NbuildCommand = Nbuild.Command;

namespace Sdo.Commands
{
    /// <summary>
    /// Updates an installed application from an apps.yaml manifest.
    /// </summary>
    public sealed class UpdateCommand : System.CommandLine.Command
    {
        public UpdateCommand(Option<bool> verboseOption, Option<bool>? dryRunOption = null)
            : base("update", "Update an installed application to the latest manifest version")
        {
            var nameOption = new Option<string?>("--name", ["-n"])
            {
                Description = "Application name to update (required unless --list is specified)"
            };
            var yamlOption = new Option<string?>("--yaml", ["-y"])
            {
                Description = "Path to an apps.yaml manifest (optional; searches the current directory and the default installation directory)"
            };
            var versionOption = new Option<string?>("--version")
            {
                Description = "Optional application version override"
            };
            var latestOption = new Option<bool>("--latest")
            {
                Description = "Discover the latest stable GitHub release instead of using the YAML version"
            };
            var listOption = new Option<bool>("--list")
            {
                Description = "List latest versions for every application without installing"
            };

            Add(nameOption);
            Add(yamlOption);
            Add(versionOption);
            Add(latestOption);
            Add(listOption);
            Add(verboseOption);
            if (dryRunOption != null)
            {
                Add(dryRunOption);
            }

            SetAction(parseResult =>
            {
                var name = parseResult.GetValue(nameOption);
                var yaml = parseResult.GetValue(yamlOption);
                var version = parseResult.GetValue(versionOption);
                var latest = parseResult.GetValue(latestOption);
                var list = parseResult.GetValue(listOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = dryRunOption != null && parseResult.GetValue(dryRunOption);

                try
                {
                    if (list)
                    {
                        if (!latest)
                        {
                            Console.Error.WriteLine("Error: --list requires --latest.");
                            return 1;
                        }

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            Console.Error.WriteLine("Error: --name cannot be used with --list.");
                            return 1;
                        }

                        var results = NbuildCommand.GetLatestVersions(yaml);
                        var box = new BoxBuilder("Application", "Manifest", "Latest");
                        foreach (var result in results)
                        {
                            var latestVersion = string.IsNullOrEmpty(result.Error)
                                ? result.LatestVersion
                                : $"ERROR: {result.Error}";
                            box.AddRow(result.Name, result.ManifestVersion, latestVersion);
                        }

                        box.Write();
                        return 0;
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.Error.WriteLine("Error: --name is required unless --list is specified.");
                        return 1;
                    }

                    return NbuildCommand.Update(yaml, name, version, verbose, dryRun, latest).Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return 1;
                }
            });
        }
    }
}
