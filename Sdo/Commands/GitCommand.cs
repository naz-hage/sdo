// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.

using Launcher.Services;
using Sdo.Git;
using System.CommandLine;

namespace Sdo.Commands
{
    /// <summary>
    /// Canonical command group for local Git branch, clone, and tag operations.
    /// </summary>
    public sealed class GitCommand : Command
    {
        private readonly Option<bool>? _dryRunOption;

        public GitCommand(Option<bool> verboseOption, Option<bool>? dryRunOption = null, Option<bool>? quietOption = null)
            : base("git", "Local Git operations")
        {
            _dryRunOption = dryRunOption;

            AddBranchCommand(verboseOption, quietOption);
            AddCloneCommand(verboseOption);
            AddTagCommand(verboseOption, quietOption);
        }

        private void AddBranchCommand(Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var branchCommand = new Command("branch", "Manage Git branches");
            var listOption = new Option<bool>("--list")
            {
                Description = "List local and remote branches"
            };
            var localOption = new Option<bool>("--local")
            {
                Description = "Limit listing to local branches"
            };
            var remoteOption = new Option<bool>("--remote")
            {
                Description = "Limit listing to remote branches"
            };
            var deleteOption = new Option<bool>("--delete")
            {
                Description = "Delete matching local and remote branches"
            };
            var containsOption = new Option<string?>("--contains")
            {
                Description = "Only select branches whose names contain this text"
            };
            var nameOption = new Option<string?>("--name")
            {
                Description = "Select one branch by name"
            };
            branchCommand.Add(listOption);
            branchCommand.Add(localOption);
            branchCommand.Add(remoteOption);
            branchCommand.Add(deleteOption);
            branchCommand.Add(containsOption);
            branchCommand.Add(nameOption);
            branchCommand.Add(verboseOption);
            AddDryRunOption(branchCommand);
            AddGetBranchCommand(branchCommand, quietOption);
            branchCommand.SetAction(parseResult =>
            {
                var list = parseResult.GetValue(listOption);
                var localOnly = parseResult.GetValue(localOption);
                var remoteOnly = parseResult.GetValue(remoteOption);
                var delete = parseResult.GetValue(deleteOption);
                var contains = parseResult.GetValue(containsOption);
                var name = parseResult.GetValue(nameOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult);

                if (localOnly && remoteOnly)
                {
                    ConsoleHelper.WriteError("Use either --local or --remote, not both.");
                    return 1;
                }

                if (list && delete)
                {
                    ConsoleHelper.WriteError("Use either --list or --delete, not both.");
                    return 1;
                }

                var git = new GitWrapper(verbose: verbose);
                var local = (!remoteOnly || delete)
                    ? git.ListBranches().Select(branch => branch.Trim())
                        .Where(branch => !string.IsNullOrWhiteSpace(branch)).ToList()
                    : [];
                var remote = (!localOnly || delete)
                    ? git.ListRemoteBranches()
                    : [];

                if (list)
                {
                    if (!remoteOnly)
                    {
                        ConsoleHelper.WriteLine("Local branches:");
                        foreach (var branch in local)
                        {
                            ConsoleHelper.WriteLine($"  {branch}");
                        }
                    }

                    if (!localOnly)
                    {
                        ConsoleHelper.WriteLine("Remote branches:");
                        foreach (var branch in remote)
                        {
                            ConsoleHelper.WriteLine($"  {branch}");
                        }
                    }

                    return 0;
                }

                if (!delete)
                {
                    ConsoleHelper.WriteError("Specify --list to list branches or --delete with --name or --contains to delete branches.");
                    return 1;
                }

                if (localOnly || remoteOnly)
                {
                    ConsoleHelper.WriteError("--local and --remote are only valid with --list.");
                    return 1;
                }

                if (string.IsNullOrWhiteSpace(contains) && string.IsNullOrWhiteSpace(name))
                {
                    ConsoleHelper.WriteError("Specify --name or --contains to select branches for deletion.");
                    return 1;
                }

                var selected = local.Concat(remote)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Where(branch => string.IsNullOrWhiteSpace(name) ||
                                     branch.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .Where(branch => string.IsNullOrWhiteSpace(contains) ||
                                     branch.Contains(contains, StringComparison.OrdinalIgnoreCase))
                    .Where(branch => !branch.Equals(git.Branch, StringComparison.OrdinalIgnoreCase) &&
                                     !branch.Equals("main", StringComparison.OrdinalIgnoreCase) &&
                                     !branch.Equals("master", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (selected.Count == 0)
                {
                    ConsoleHelper.WriteError("No matching branches were found.");
                    return 1;
                }

                ConsoleHelper.WriteLine("Branches selected for deletion:");
                foreach (var branch in selected)
                {
                    var isLocal = local.Contains(branch, StringComparer.OrdinalIgnoreCase);
                    var isRemote = remote.Contains(branch, StringComparer.OrdinalIgnoreCase);
                    var location = isLocal && isRemote
                        ? "local & remote"
                        : isLocal
                            ? "local"
                            : "remote";
                    ConsoleHelper.WriteLine($"  {branch} ({location})");
                }

                if (!string.IsNullOrWhiteSpace(contains))
                {
                    ConsoleHelper.WriteWarning($"Delete {selected.Count} matching branch(es)? Type 'yes' to continue:");
                    if (!string.Equals(Console.ReadLine(), "yes", StringComparison.OrdinalIgnoreCase))
                    {
                        ConsoleHelper.WriteInfo("Deletion cancelled.");
                        return 0;
                    }
                }

                if (dryRun)
                {
                    ConsoleHelper.WriteInfo($"Dry run: would delete {selected.Count} branch(es).");
                    return 0;
                }

                var failures = 0;
                foreach (var branch in selected)
                {
                    if (local.Contains(branch, StringComparer.OrdinalIgnoreCase) && !git.DeleteBranch(branch))
                    {
                        failures++;
                    }

                    if (remote.Contains(branch, StringComparer.OrdinalIgnoreCase) && !git.DeleteRemoteBranch(branch))
                    {
                        failures++;
                    }
                }

                return failures == 0 ? 0 : 1;
            });
            Subcommands.Add(branchCommand);
        }

        private void AddGetBranchCommand(Command branchCommand, Option<bool>? quietOption)
        {
            var getCommand = new Command(
                "get",
                "Display the current local Git branch on a single line.");
            getCommand.SetAction(parseResult =>
                Nbuild.Command.GetBranch(quietOption != null && parseResult.GetValue(quietOption)).Code);
            branchCommand.Subcommands.Add(getCommand);
        }

        private void AddCloneCommand(Option<bool> verboseOption)
        {
            var cloneCommand = new Command(
                "clone",
                "Clone a Git repository to a specified path");
            var urlOption = new Option<string>("--url")
            {
                Description = "Git repository URL",
                Required = true
            };
            var pathOption = new Option<string?>("--path")
            {
                Description = "Destination path; defaults to the current directory"
            };

            cloneCommand.Add(urlOption);
            cloneCommand.Add(pathOption);
            cloneCommand.Add(verboseOption);
            if (_dryRunOption != null)
            {
                cloneCommand.Add(_dryRunOption);
            }

            cloneCommand.SetAction(parseResult =>
            {
                var url = parseResult.GetValue(urlOption);
                var path = parseResult.GetValue(pathOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = _dryRunOption != null && parseResult.GetValue(_dryRunOption);
                return Nbuild.Command.Clone(url, path, verbose, dryRun).Code;
            });

            Subcommands.Add(cloneCommand);
        }

        private void AddTagCommand(Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var tagCommand = new Command(
                "tag",
                "Manage Git tags. Commands change local tags by default; list shows the latest 10 by version and compares with origin (--all lists and compares every tag), --push publishes tags, and delete --remote also removes them from origin.");

            AddAutoTagCommand(tagCommand, verboseOption, quietOption);
            AddDeleteTagCommand(tagCommand, verboseOption, quietOption);
            AddGetTagCommand(tagCommand, quietOption);
            AddListTagCommand(tagCommand, verboseOption, quietOption);
            AddPushTagCommand(tagCommand, verboseOption, quietOption);
            AddSetTagCommand(tagCommand, verboseOption, quietOption);

            Subcommands.Add(tagCommand);
        }

        private void AddGetTagCommand(Command tagCommand, Option<bool>? quietOption)
        {
            var getCommand = new Command(
                "get",
                "Display the current local Git tag on a single line.");
            getCommand.SetAction(parseResult =>
                Nbuild.Command.GetTag(quietOption != null && parseResult.GetValue(quietOption)).Code);
            tagCommand.Subcommands.Add(getCommand);
        }

        private void AddListTagCommand(Command tagCommand, Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var listCommand = new Command(
                "list",
                "List the latest 10 local tags by version and show local/origin discrepancies; use --all to list and compare every tag.");
            var allOption = new Option<bool>("--all")
            {
                Description = "List all local tags instead of only the latest 10"
            };

            listCommand.Add(allOption);
            listCommand.Add(verboseOption);
            listCommand.SetAction(parseResult =>
            {
                var all = parseResult.GetValue(allOption);
                var verbose = parseResult.GetValue(verboseOption);
                var quiet = quietOption != null && parseResult.GetValue(quietOption);
                return Nbuild.Command.ListTags(all, verbose, quiet).Code;
            });

            tagCommand.Subcommands.Add(listCommand);
        }

        private void AddPushTagCommand(Command tagCommand, Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var pushCommand = new Command(
                "push",
                "Push an existing local tag to origin without changing it.");
            var tagOption = new Option<string>("--tag", ["-t"])
            {
                Description = "Tag to push to origin",
                Required = true
            };

            pushCommand.Add(tagOption);
            pushCommand.Add(verboseOption);
            pushCommand.SetAction(parseResult =>
            {
                var tag = parseResult.GetValue(tagOption);
                var verbose = parseResult.GetValue(verboseOption);
                var quiet = quietOption != null && parseResult.GetValue(quietOption);
                return Nbuild.Command.PushTag(tag, verbose, quiet).Code;
            });

            tagCommand.Subcommands.Add(pushCommand);
        }

        private void AddAutoTagCommand(Command tagCommand, Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var autoCommand = new Command(
                "auto",
                "Calculate and create the next STAGE or PROD tag locally; use --push to publish it to origin.");
            var buildTypeOption = new Option<string>("--buildtype", ["-b"])
            {
                Description = "Build type used to calculate the next tag: STAGE or PROD",
                Required = true
            };
            var pushOption = new Option<bool>("--push")
            {
                Description = "Push the created tag to origin"
            };

            autoCommand.Add(buildTypeOption);
            autoCommand.Add(pushOption);
            autoCommand.Add(verboseOption);
            AddDryRunOption(autoCommand);
            autoCommand.SetAction(parseResult =>
            {
                var buildType = parseResult.GetValue(buildTypeOption);
                var push = parseResult.GetValue(pushOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult);
                var quiet = quietOption != null && parseResult.GetValue(quietOption);
                return Nbuild.Command.SetAutoTag(buildType, push, verbose, dryRun, quiet).Code;
            });

            tagCommand.Subcommands.Add(autoCommand);
        }

        private void AddDeleteTagCommand(Command tagCommand, Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var deleteCommand = new Command(
                "delete",
                "Delete a local tag; use --remote to also delete it from origin.");
            var tagOption = new Option<string>("--tag", ["-t"])
            {
                Description = "Tag to delete from the local repository",
                Required = true
            };
            var remoteOption = new Option<bool>("--remote")
            {
                Description = "Also delete the tag from origin"
            };

            deleteCommand.Add(tagOption);
            deleteCommand.Add(remoteOption);
            deleteCommand.Add(verboseOption);
            AddDryRunOption(deleteCommand);
            deleteCommand.SetAction(parseResult =>
            {
                var tag = parseResult.GetValue(tagOption);
                var deleteRemote = parseResult.GetValue(remoteOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult);
                var quiet = quietOption != null && parseResult.GetValue(quietOption);
                return Nbuild.Command.DeleteTag(tag, verbose, dryRun, deleteRemote, quiet).Code;
            });

            tagCommand.Subcommands.Add(deleteCommand);
        }

        private void AddSetTagCommand(Command tagCommand, Option<bool> verboseOption, Option<bool>? quietOption)
        {
            var setCommand = new Command(
                "set",
                "Create or replace a local annotated tag at HEAD; use --push to publish it to origin.");
            var tagOption = new Option<string>("--tag", ["-t"])
            {
                Description = "Tag name to create or replace locally",
                Required = true
            };
            var pushOption = new Option<bool>("--push")
            {
                Description = "Push the created tag to origin"
            };

            setCommand.Add(tagOption);
            setCommand.Add(pushOption);
            setCommand.Add(verboseOption);
            AddDryRunOption(setCommand);
            setCommand.TreatUnmatchedTokensAsErrors = true;
            setCommand.SetAction(parseResult =>
            {
                var quiet = quietOption != null && parseResult.GetValue(quietOption);
                if (parseResult.UnmatchedTokens.Count > 0)
                {
                    if (!quiet)
                    {
                        Console.Error.WriteLine($"Unknown option or argument: {string.Join(' ', parseResult.UnmatchedTokens)}");
                    }
                    return 1;
                }

                var tag = parseResult.GetValue(tagOption);
                var push = parseResult.GetValue(pushOption);
                var verbose = parseResult.GetValue(verboseOption);
                var dryRun = GetDryRunValue(parseResult);
                return Nbuild.Command.SetTag(tag, verbose, dryRun, push, quiet).Code;
            });

            tagCommand.Subcommands.Add(setCommand);
        }

        private void AddDryRunOption(Command command)
        {
            if (_dryRunOption != null)
            {
                command.Add(_dryRunOption);
            }
        }

        private bool GetDryRunValue(ParseResult parseResult)
        {
            return _dryRunOption != null && parseResult.GetValue(_dryRunOption);
        }
    }
}
