using Launcher.Services;
using System.CommandLine;
using Sdo.Services;
using NbuildCommand = Nbuild.Command;

namespace Sdo.Commands
{
    /// <summary>
    /// Command handler for environment tool management.
    /// </summary>
    public class ToolCommand : System.CommandLine.Command
    {
        /// <summary>
        /// Initializes a new instance of the ToolCommand class.
        /// </summary>
        /// <param name="verboseOption">Option for verbose output.</param>
        public ToolCommand(Option<bool> verboseOption, Option<bool>? dryRunOption = null) : base("tool", "Environment tool installation, auditing, and manifest management")
        {
            AddInstallCommand(verboseOption, dryRunOption);
            AddListCommand(verboseOption, dryRunOption);
            AddManifestCommand(verboseOption, dryRunOption);
            AddUninstallCommand(verboseOption, dryRunOption);
            AddDownloadCommand(verboseOption, dryRunOption);
        }

        private void AddInstallCommand(Option<bool> verboseOption, Option<bool>? dryRunOption)
        {
            var installCommand = new System.CommandLine.Command("install", "Install tools from a manifest or by application name.");
            var manifestOption = new Option<string?>("--manifest", ["-m"])
            {
                Description = "Path to a JSON or YAML tools manifest; if omitted, searches apps.yaml in the current directory, the executable directory, and the default installation directory"
            };
            var nameOption = new Option<string?>("--name", ["-n"]) { Description = "Application name to find in apps.json" };
            var versionOption = new Option<string?>("--appversion", ["-av"]) { Description = "Optional application version override" };
            var latestOption = new Option<bool>("--latest") { Description = "Discover the latest stable release instead of using the manifest version" };

            installCommand.Add(manifestOption);
            installCommand.Add(nameOption);
            installCommand.Add(versionOption);
            installCommand.Add(latestOption);
            installCommand.Add(verboseOption);
            AddDryRunOption(installCommand, dryRunOption);
            installCommand.SetAction(parseResult =>
            {
                var manifest = parseResult.GetValue(manifestOption);
                var name = parseResult.GetValue(nameOption);
                var version = parseResult.GetValue(versionOption);
                var latest = parseResult.GetValue(latestOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult, dryRunOption);

                if (string.IsNullOrEmpty(manifest) && string.IsNullOrEmpty(name))
                {
                    Console.Error.WriteLine("Error: Either --manifest (-m) or --name (-n) must be specified.");
                    return 1;
                }

                if (latest && string.IsNullOrWhiteSpace(name))
                {
                    Console.Error.WriteLine("Error: --name (-n) is required with --latest.");
                    return 1;
                }

                if (latest && !string.IsNullOrWhiteSpace(version))
                {
                    Console.Error.WriteLine("Error: --appversion (-av) cannot be used with --latest.");
                    return 1;
                }

                WriteDryRunNotice(dryRun);
                try
                {
                    return latest
                        ? NbuildCommand.InstallLatest(manifest, name, version, verbose, dryRun, latest: true).Code
                        : NbuildCommand.Install(manifest, name, version, verbose, dryRun).Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return -1;
                }
            });

            Subcommands.Add(installCommand);
        }

        private void AddManifestCommand(Option<bool> verboseOption, Option<bool>? dryRunOption)
        {
            var manifestCommand = new System.CommandLine.Command("manifest", "Maintain tool manifests.");
            var updateCommand = new System.CommandLine.Command("update", "Update manifest versions from the latest stable releases.");
            var manifestOption = new Option<string?>("--manifest", ["-m"])
            {
                Description = "Path to the YAML tools manifest; if omitted, searches apps.yaml in the current directory, the executable directory, and the default installation directory"
            };
            var nameOption = new Option<string?>("--name", ["-n"])
            {
                Description = "Optional application name; updates all applications when omitted"
            };
            var filePathOption = new Option<string?>("--file-path")
            {
                Description = "Optional Markdown file path to update with manifest versions"
            };
            var latestOption = new Option<bool>("--latest")
            {
                Description = "Discover and write the latest stable release versions"
            };

            updateCommand.Add(manifestOption);
            updateCommand.Add(nameOption);
            updateCommand.Add(filePathOption);
            updateCommand.Add(latestOption);
            updateCommand.Add(verboseOption);
            AddDryRunOption(updateCommand, dryRunOption);
            updateCommand.SetAction(parseResult =>
            {
                var manifest = parseResult.GetValue(manifestOption);
                var name = parseResult.GetValue(nameOption);
                var filePath = parseResult.GetValue(filePathOption);
                var latest = parseResult.GetValue(latestOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult, dryRunOption);

                if (!latest)
                {
                    Console.Error.WriteLine("Error: --latest is required.");
                    return 1;
                }

                WriteDryRunNotice(dryRun);
                try
                {
                    var result = NbuildCommand.UpdateManifest(manifest, name, verbose, dryRun);
                    if (!result.IsSuccess())
                    {
                        Console.Error.WriteLine($"Error: {result.GetFirstOutput()}");
                        return result.Code;
                    }

                    if (!dryRun && !string.IsNullOrWhiteSpace(filePath))
                    {
                        var resolvedManifest = NbuildCommand.ResolveManifestPath(manifest);
                        var documentationPath = Path.GetFullPath(filePath);
                        new UpdateVersionsInDocs().Update(
                            Path.GetDirectoryName(Path.GetFullPath(resolvedManifest))
                                ?? throw new InvalidOperationException("The manifest directory could not be determined."),
                            documentationPath);
                        ConsoleHelper.WriteSuccess($"Updated documentation: {documentationPath}");
                    }

                    return result.Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return -1;
                }
            });

            manifestCommand.Subcommands.Add(updateCommand);
            Subcommands.Add(manifestCommand);
        }

        private void AddUninstallCommand(Option<bool> verboseOption, Option<bool>? dryRunOption)
        {
            var uninstallCommand = new System.CommandLine.Command("uninstall", "Uninstall tools from a manifest.");
            var manifestOption = new Option<string?>("--manifest", ["-m"])
            {
                Description = "Path to a JSON or YAML tools manifest; if omitted, searches apps.yaml in the current directory, the executable directory, and the default installation directory"
            };
            var nameOption = new Option<string?>("--name", ["-n"]) { Description = "Application name to uninstall" };
            var versionOption = new Option<string?>("--appversion", ["-av"]) { Description = "Optional application version to uninstall" };
            uninstallCommand.Add(manifestOption);
            uninstallCommand.Add(nameOption);
            uninstallCommand.Add(versionOption);
            uninstallCommand.Add(verboseOption);
            AddDryRunOption(uninstallCommand, dryRunOption);
            uninstallCommand.SetAction(parseResult =>
            {
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult, dryRunOption);
                var manifest = parseResult.GetValue(manifestOption);
                var name = parseResult.GetValue(nameOption);
                var version = parseResult.GetValue(versionOption);
                if (string.IsNullOrEmpty(manifest) && string.IsNullOrEmpty(name))
                {
                    Console.Error.WriteLine("Error: Either --manifest (-m) or --name (-n) must be specified.");
                    return 1;
                }

                WriteDryRunNotice(dryRun);
                try
                {
                    return NbuildCommand.Uninstall(manifest, verbose, dryRun, name, version).Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return -1;
                }
            });

            Subcommands.Add(uninstallCommand);
        }

        private void AddDownloadCommand(Option<bool> verboseOption, Option<bool>? dryRunOption)
        {
            var downloadCommand = new System.CommandLine.Command("download", "Download tools from a manifest.");
            var manifestOption = new Option<string?>("--manifest", ["-m"])
            {
                Description = "Path to a JSON or YAML tools manifest; if omitted, searches apps.yaml in the current directory, the executable directory, and the default installation directory"
            };
            var nameOption = new Option<string?>("--name", ["-n"]) { Description = "Application name to download" };
            var versionOption = new Option<string?>("--appversion", ["-av"]) { Description = "Optional application version override" };
            downloadCommand.Add(manifestOption);
            downloadCommand.Add(nameOption);
            downloadCommand.Add(versionOption);
            downloadCommand.Add(verboseOption);
            AddDryRunOption(downloadCommand, dryRunOption);
            downloadCommand.SetAction(parseResult =>
            {
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult, dryRunOption);
                var manifest = parseResult.GetValue(manifestOption);
                var name = parseResult.GetValue(nameOption);
                var version = parseResult.GetValue(versionOption);
                WriteDryRunNotice(dryRun);
                try
                {
                    return NbuildCommand.Download(manifest, verbose, dryRun, name, version).Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return -1;
                }
            });

            Subcommands.Add(downloadCommand);
        }

        private static void AddDryRunOption(System.CommandLine.Command command, Option<bool>? dryRunOption)
        {
            if (dryRunOption != null)
            {
                command.Add(dryRunOption);
            }
        }

        private static bool GetDryRunValue(ParseResult parseResult, Option<bool>? dryRunOption)
        {
            return dryRunOption != null && parseResult.GetValue(dryRunOption);
        }

        private static void WriteDryRunNotice(bool dryRun)
        {
            if (dryRun)
            {
                ConsoleHelper.WriteWarning("DRY-RUN: running in dry-run mode; no destructive actions will be performed.");
            }
        }

        private void AddListCommand(Option<bool> verboseOption, Option<bool>? dryRunOption)
        {
            var listCommand = new System.CommandLine.Command(
                "list",
                "Display a formatted table of all tools and their versions.");
            listCommand.TreatUnmatchedTokensAsErrors = true;

            var manifestOption = new Option<string?>("--manifest", ["-m"])
            {
                Description = "Path to a JSON or YAML tools manifest; if omitted, searches apps.yaml in the current directory, the executable directory, and the default installation directory"
            };
            var nameOption = new Option<string?>("--name", ["-n"])
            {
                Description = "Application name to list"
            };
            var versionOption = new Option<string?>("--appversion", ["-av"])
            {
                Description = "Optional application version to list"
            };
            listCommand.Options.Add(manifestOption);
            listCommand.Options.Add(nameOption);
            listCommand.Options.Add(versionOption);
            listCommand.Add(verboseOption);
            AddDryRunOption(listCommand, dryRunOption);
            listCommand.SetAction(parseResult =>
            {
                var manifest = parseResult.GetValue(manifestOption);
                var name = parseResult.GetValue(nameOption);
                var version = parseResult.GetValue(versionOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult, dryRunOption);

                WriteDryRunNotice(dryRun);
                try
                {
                    return NbuildCommand.List(manifest, verbose, name, version).Code;
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine($"Error: {exception.Message}");
                    return -1;
                }
            });

            Subcommands.Add(listCommand);
        }

    }
}