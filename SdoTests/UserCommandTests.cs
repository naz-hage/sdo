// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.

using Sdo.Commands;
using System.CommandLine;

namespace SdoTests;

[TestClass]

public class UserCommandTests
{
    private readonly Option<bool> _verboseOption;

    public UserCommandTests()
    {
        _verboseOption = new Option<bool>("--verbose");
        _verboseOption.Description = "Enable verbose output";
    }

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectNameAndDescription()
    {
        var cmd = new UserCommand(_verboseOption);
        Assert.AreEqual("user", cmd.Name);
        Assert.AreEqual("User management commands for GitHub and Azure DevOps", cmd.Description);
    }

    [TestMethod]
    public void Constructor_AddsExpectedSubcommands()
    {
        var cmd = new UserCommand(_verboseOption);
        Assert.IsNotNull(cmd.Subcommands);
        Assert.IsTrue(cmd.Subcommands.Any(s => s.Name == "show"));
        Assert.IsTrue(cmd.Subcommands.Any(s => s.Name == "list"));
        Assert.IsTrue(cmd.Subcommands.Any(s => s.Name == "search"));
        Assert.IsTrue(cmd.Subcommands.Any(s => s.Name == "permissions"));
    }

    [TestMethod]
    public void ShowSubcommand_HasLoginOption()
    {
        var cmd = new UserCommand(_verboseOption);
        var show = cmd.Subcommands.Single(s => s.Name == "show");
        var opt = show.Options.Single(o => o.Name == "--login");
        Assert.AreEqual("User login or id", opt.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasTopOption()
    {
        var cmd = new UserCommand(_verboseOption);
        var list = cmd.Subcommands.Single(s => s.Name == "list");
        var opt = list.Options.Single(o => o.Name == "--top");
        Assert.AreEqual("Limit results", opt.Description);
    }

    [TestMethod]
    public void SearchSubcommand_RequiresQuery()
    {
        var cmd = new UserCommand(_verboseOption);
        var args = new[] { "search" };
        var result = cmd.Parse(args).Invoke();
        Assert.AreNotEqual(0, result);
    }

    [TestMethod]
    public void PermissionsSubcommand_HasUserOption()
    {
        var cmd = new UserCommand(_verboseOption);
        var perms = cmd.Subcommands.Single(s => s.Name == "permissions");
        var opt = perms.Options.Single(o => o.Name == "--user");
        Assert.AreEqual("User login or id", opt.Description);
    }

    [TestMethod]
    public void Subcommands_HaveGlobalVerboseOption()
    {
        var cmd = new UserCommand(_verboseOption);
        foreach (var sub in cmd.Subcommands)
        {
            Assert.IsTrue(sub.Options.Any(o => o.Name == "--verbose"));
        }
    }
}
