using Sdo.Commands;
using System.CommandLine;

namespace SdoTests;

[TestClass]

public class RepositoryCommandTests
{
    private readonly Option<bool> _verboseOption;

    public RepositoryCommandTests()
    {
        _verboseOption = new Option<bool>("--verbose");
    }

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectName()
    {
        var command = new RepositoryCommand(_verboseOption);
        Assert.AreEqual("repo", command.Name);
    }

    [TestMethod]
    public void Constructor_AddsCreateSubcommand()
    {
        var command = new RepositoryCommand(_verboseOption);
        var createCmd = command.Subcommands.Single(s => s.Name == "create");
        Assert.IsNotNull(createCmd);
    }

    [TestMethod]
    public void Constructor_AddsBranchSubcommand()
    {
        var command = new RepositoryCommand(_verboseOption);
        var branchCmd = command.Subcommands.Single(s => s.Name == "branch");

        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--list"));
        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--local"));
        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--remote"));
        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--delete"));
        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--contains"));
        Assert.IsNotNull(branchCmd.Options.FirstOrDefault(o => o.Name == "--name"));
    }

    [TestMethod]
    public void Constructor_AddsCloneSubcommandWithUrlAndPathOptions()
    {
        var command = new RepositoryCommand(_verboseOption, dryRunOption: new Option<bool>("--dry-run"));
        var cloneCmd = command.Subcommands.Single(s => s.Name == "clone");

        Assert.IsNotNull(cloneCmd.Options.FirstOrDefault(o => o.Name == "--url"));
        Assert.IsNotNull(cloneCmd.Options.FirstOrDefault(o => o.Name == "--path"));
    }

    [TestMethod]
    public void Constructor_AddsTagSubcommandWithNestedCommands()
    {
        var command = new RepositoryCommand(_verboseOption, dryRunOption: new Option<bool>("--dry-run"));
        var tagCmd = command.Subcommands.Single(s => s.Name == "tag");
        var nestedNames = tagCmd.Subcommands.Select(s => s.Name).ToList();

        Assert.IsTrue(new[] { "auto", "delete", "push-auto", "set" }.SequenceEqual(nestedNames));
    }

    [TestMethod]
    public void AutoTagSubcommand_HasBuildTypeShortAlias()
    {
        var command = new RepositoryCommand(_verboseOption);
        var tagCmd = command.Subcommands.First(s => s.Name == "tag");
        var autoCmd = tagCmd.Subcommands.First(s => s.Name == "auto");
        var buildTypeOption = autoCmd.Options.First(o => o.Name == "--buildtype");

        Assert.IsTrue(buildTypeOption.Aliases.Contains("-b"));
    }

    [TestMethod]
    public void Constructor_AddsDeleteSubcommand()
    {
        var command = new RepositoryCommand(_verboseOption);
        var deleteCmd = command.Subcommands.Single(s => s.Name == "delete");
        Assert.IsNotNull(deleteCmd);
    }

    [TestMethod]
    public void Constructor_AddsListSubcommand()
    {
        var command = new RepositoryCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");
        Assert.IsNotNull(listCmd);
    }

    [TestMethod]
    public void Constructor_AddsInfoSubcommand()
    {
        var command = new RepositoryCommand(_verboseOption);
        var infoCmd = command.Subcommands.Single(s => s.Name == "info");
        Assert.IsNotNull(infoCmd);
    }

    [TestMethod]
    public void Constructor_RegistersSubcommandsInAlphabeticalOrder()
    {
        var command = new RepositoryCommand(_verboseOption);
        var subcommandNames = command.Subcommands.Select(s => s.Name).ToList();
        Assert.IsTrue(new[] { "branch", "clone", "create", "delete", "info", "list", "tag" }.SequenceEqual(subcommandNames));
    }

    [TestMethod]
    public void CreateSubcommand_HasNameArgument()
    {
        var command = new RepositoryCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var nameArg = createCmd.Arguments.FirstOrDefault(a => a.Name == "name");
        Assert.IsNotNull(nameArg);
    }

    [TestMethod]
    public void CreateSubcommand_HasOptionalDescriptionOption()
    {
        var command = new RepositoryCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var descOption = createCmd.Options.FirstOrDefault(o =>
            o.Name.Contains("description"));
        Assert.IsNotNull(descOption);
    }

    [TestMethod]
    public void CreateSubcommand_HasPrivateOption()
    {
        var command = new RepositoryCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var privateOption = createCmd.Options.FirstOrDefault(o =>
            o.Name.Contains("private"));
        Assert.IsNotNull(privateOption);
    }

    [TestMethod]
    public void DeleteSubcommand_HasForceOption()
    {
        var command = new RepositoryCommand(_verboseOption);
        var deleteCmd = command.Subcommands.First(s => s.Name == "delete");
        var forceOption = deleteCmd.Options.FirstOrDefault(o =>
            o.Name.Contains("force"));
        Assert.IsNotNull(forceOption);
    }

    [TestMethod]
    public void ListSubcommand_HasTopOption()
    {
        var command = new RepositoryCommand(_verboseOption);
        var listCmd = command.Subcommands.First(s => s.Name == "list");
        var topOption = listCmd.Options.FirstOrDefault(o =>
            o.Name.Contains("top"));
        Assert.IsNotNull(topOption);
    }
}
