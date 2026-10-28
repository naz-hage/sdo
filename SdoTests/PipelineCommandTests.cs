// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// PipelineCommandTests.cs
//
// Unit tests for PipelineCommand - pipeline/workflow management operations.

using System.CommandLine;
using Sdo.Commands;

namespace SdoTests;

/// <summary>
/// Unit tests for PipelineCommand covering all subcommands and options.
/// </summary>
[TestClass]
public class PipelineCommandTests
{
    private readonly Option<bool> _verboseOption;

    public PipelineCommandTests()
    {
        _verboseOption = new Option<bool>("--verbose");
    }

    #region Command Structure Tests

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectName()
    {
        var command = new PipelineCommand(_verboseOption);
        Assert.AreEqual("pipeline", command.Name);
    }

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectDescription()
    {
        var command = new PipelineCommand(_verboseOption);
        Assert.IsNotNull(command.Description);
        StringAssert.Contains(command.Description, "Pipeline");
    }

    [TestMethod]
    public void Constructor_Has9Subcommands()
    {
        var command = new PipelineCommand(_verboseOption);
        Assert.AreEqual(9, command.Subcommands.Count);
    }

    [TestMethod]
    public void Constructor_RegistersSubcommandsInAlphabeticalOrder()
    {
        var command = new PipelineCommand(_verboseOption);
        var subcommandNames = command.Subcommands.Select(s => s.Name).ToList();
        var expected = new[] { "create", "delete", "lastbuild", "list", "logs", "run", "show", "status", "update" };
        Assert.IsTrue(expected.SequenceEqual(subcommandNames));
    }

    #endregion

    #region Individual Subcommand Tests

    [TestMethod]
    public void Constructor_AddsCreateSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var createCmd = command.Subcommands.Single(s => s.Name == "create");
        Assert.IsNotNull(createCmd);
    }

    [TestMethod]
    public void Constructor_AddsDeleteSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var deleteCmd = command.Subcommands.Single(s => s.Name == "delete");
        Assert.IsNotNull(deleteCmd);
    }

    [TestMethod]
    public void Constructor_AddsLastbuildSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var lastbuildCmd = command.Subcommands.Single(s => s.Name == "lastbuild");
        Assert.IsNotNull(lastbuildCmd);
    }

    [TestMethod]
    public void Constructor_AddsListSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");
        Assert.IsNotNull(listCmd);
    }

    [TestMethod]
    public void Constructor_AddsLogsSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var logsCmd = command.Subcommands.Single(s => s.Name == "logs");
        Assert.IsNotNull(logsCmd);
    }

    [TestMethod]
    public void Constructor_AddsRunSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var runCmd = command.Subcommands.Single(s => s.Name == "run");
        Assert.IsNotNull(runCmd);
    }

    [TestMethod]
    public void Constructor_AddsShowSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");
        Assert.IsNotNull(showCmd);
    }

    [TestMethod]
    public void Constructor_AddsStatusSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var statusCmd = command.Subcommands.Single(s => s.Name == "status");
        Assert.IsNotNull(statusCmd);
    }

    [TestMethod]
    public void Constructor_AddsUpdateSubcommand()
    {
        var command = new PipelineCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");
        Assert.IsNotNull(updateCmd);
    }

    #endregion

    #region Subcommand Options Tests

    [TestMethod]
    public void CreateSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        Assert.IsNotEmpty(createCmd.Options);
    }

    [TestMethod]
    public void CreateSubcommand_HasFileArgument()
    {
        var command = new PipelineCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        Assert.IsNotEmpty(createCmd.Arguments);
    }

    [TestMethod]
    public void CreateCommand_CanBeCalledWithFilePath()
    {
        var command = new PipelineCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var fileArg = createCmd.Arguments.FirstOrDefault(a => a.Name.Contains("file"));
        Assert.IsNotNull(fileArg);
    }

    [TestMethod]
    public void DeleteSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var deleteCmd = command.Subcommands.First(s => s.Name == "delete");
        // Should have force option and verbose option
        Assert.IsTrue(deleteCmd.Options.Count >= 2);
    }

    [TestMethod]
    public void DeleteSubcommand_HasPipelineIdArgument()
    {
        var command = new PipelineCommand(_verboseOption);
        var deleteCmd = command.Subcommands.First(s => s.Name == "delete");
        Assert.IsNotEmpty(deleteCmd.Arguments);
    }

    [TestMethod]
    public void DeleteCommand_HasForceOption()
    {
        var command = new PipelineCommand(_verboseOption);
        var deleteCmd = command.Subcommands.First(s => s.Name == "delete");
        // Verify force option exists (can be checked by name containing "force")
        var hasForceOption = deleteCmd.Options.Any(o => o.Name.Contains("force"));
        Assert.IsTrue(hasForceOption);
    }

    [TestMethod]
    public void LastbuildSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var lastbuildCmd = command.Subcommands.First(s => s.Name == "lastbuild");
        Assert.IsNotEmpty(lastbuildCmd.Options);
    }

    [TestMethod]
    public void LastbuildSubcommand_HasPipelineNameArgument()
    {
        var command = new PipelineCommand(_verboseOption);
        var lastbuildCmd = command.Subcommands.First(s => s.Name == "lastbuild");
        Assert.IsNotEmpty(lastbuildCmd.Arguments);
    }

    [TestMethod]
    public void ListSubcommand_HasMultipleOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var listCmd = command.Subcommands.First(s => s.Name == "list");
        // List should have: verbose, repo, and all options
        Assert.IsTrue(listCmd.Options.Count >= 3);
    }

    [TestMethod]
    public void ListCommand_NoRequiredArguments()
    {
        var command = new PipelineCommand(_verboseOption);
        var listCmd = command.Subcommands.First(s => s.Name == "list");
        // List command should have no required arguments
        var requiredArgs = listCmd.Arguments.Where(a => a.Arity.MinimumNumberOfValues > 0).ToList();
        Assert.IsEmpty(requiredArgs);
    }

    [TestMethod]
    public void ListCommand_CanBeCalledWithoutOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var listCmd = command.Subcommands.First(s => s.Name == "list");
        Assert.IsNotNull(listCmd);
        Assert.IsNotEmpty(listCmd.Options);
    }

    [TestMethod]
    public void LogsSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var logsCmd = command.Subcommands.First(s => s.Name == "logs");
        Assert.IsNotEmpty(logsCmd.Options);
    }

    [TestMethod]
    public void RunSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var runCmd = command.Subcommands.First(s => s.Name == "run");
        // Should have branch option and verbose option
        Assert.IsTrue(runCmd.Options.Count >= 2);
    }

    [TestMethod]
    public void ShowSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var showCmd = command.Subcommands.First(s => s.Name == "show");
        Assert.IsNotEmpty(showCmd.Options);
    }

    [TestMethod]
    public void ShowCommand_NoArguments()
    {
        var command = new PipelineCommand(_verboseOption);
        var showCmd = command.Subcommands.First(s => s.Name == "show");
        // Show command should have no required arguments
        var requiredArgs = showCmd.Arguments.Where(a => a.Arity.MinimumNumberOfValues > 0).ToList();
        Assert.IsEmpty(requiredArgs);
    }

    [TestMethod]
    public void StatusSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var statusCmd = command.Subcommands.First(s => s.Name == "status");
        Assert.IsNotEmpty(statusCmd.Options);
    }

    [TestMethod]
    public void UpdateSubcommand_HasOptions()
    {
        var command = new PipelineCommand(_verboseOption);
        var updateCmd = command.Subcommands.First(s => s.Name == "update");
        Assert.IsNotEmpty(updateCmd.Options);
    }

    #endregion
}
