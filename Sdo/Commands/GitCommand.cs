// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.

using System.CommandLine;

namespace Sdo.Commands
{
    /// <summary>
    /// Canonical command group for local Git branch, clone, and tag operations.
    /// </summary>
    public sealed class GitCommand : RepositoryCommand
    {
        public GitCommand(Option<bool> verboseOption, Option<bool>? dryRunOption = null, Option<bool>? quietOption = null)
            : base(
                verboseOption,
                mappingGenerator: null,
                mappingPresenter: null,
                dryRunOption: dryRunOption,
                quietOption: quietOption,
                commandName: "git",
                description: "Local Git operations",
                includeRepositoryManagementCommands: false,
                includeGitCommands: true)
        {
        }
    }
}
