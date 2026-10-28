using System.CommandLine;
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
            var nameOption = new Option<string>("--name", ["-n"])
            {
                Description = "Application name to update",
                Required = true
            };
            var yamlOption = new Option<string?>("--yaml", ["-y"])
            {
                Description = "Path to an apps.yaml manifest (optional; searches the current directory and the default installation directory)"
            };
            var versionOption = new Option<string?>("--version")
            {
                Description = "Optional application version override"
            };

            Add(nameOption);
            Add(yamlOption);
            Add(versionOption);
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
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = dryRunOption != null && parseResult.GetValue(dryRunOption);

                try
                {
                    return NbuildCommand.Update(yaml, name, version, verbose, dryRun).Code;
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
