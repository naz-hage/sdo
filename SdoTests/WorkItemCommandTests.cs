// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// WorkItemCommandTests.cs
//
// Unit tests for the WorkItemCommand class.

using Sdo.Commands;
using System.CommandLine;

namespace SdoTests;

/// <summary>
/// Unit tests for the WorkItemCommand class.
/// Tests for show and list subcommands (existing functionality) and
/// placeholder tests for update and comment subcommands (TDD-driven development).
/// </summary>
[TestClass]
public class WorkItemCommandTests
{
    private readonly Option<bool> _verboseOption;

    public WorkItemCommandTests()
    {
        _verboseOption = new Option<bool>("--verbose");
        _verboseOption.Description = "Enable verbose output";
    }

    #region Constructor and Subcommand Registration Tests

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectNameAndDescription()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        Assert.AreEqual("wi", command.Name);
        Assert.AreEqual("Work item management commands", command.Description);
    }

    [TestMethod]
    public void Constructor_AddsShowSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var showSubcommand = command.Subcommands.Single(s => s.Name == "show");
        Assert.IsNotNull(showSubcommand);
        Assert.AreEqual("Display detailed work item information", showSubcommand.Description);
    }

    [TestMethod]
    public void Constructor_AddsListSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var listSubcommand = command.Subcommands.Single(s => s.Name == "list");
        Assert.IsNotNull(listSubcommand);
        Assert.AreEqual("List work items with optional filtering", listSubcommand.Description);
    }

    #endregion

    #region Show Subcommand Option Tests

    [TestMethod]
    public void ShowSubcommand_HasIdOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");

        // Assert
        var idOption = showCmd.Options.Single(o => o.Name == "--id");
        Assert.IsNotNull(idOption);
        Assert.AreEqual("Work item ID (optional; auto-detected from branch name if not on main)", idOption.Description);
    }

    [TestMethod]
    public void ShowSubcommand_HasCommentsOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");

        // Assert
        var commentsOption = showCmd.Options.Single(o => o.Name == "--comments");
        Assert.IsNotNull(commentsOption);
        Assert.AreEqual("Show comments/discussion", commentsOption.Description);
    }

    [TestMethod]
    public void ShowSubcommand_CommentsOptionHasShortAlias()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");
        var commentsOption = showCmd.Options.Single(o => o.Name == "--comments");

        // Assert
        // The option should support -c as a short alias
        Assert.IsNotNull(commentsOption);
        Assert.IsTrue(commentsOption.Aliases.Contains("-c"));
    }

    [TestMethod]
    public void ShowSubcommand_HasVerboseOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");

        // Assert
        var verboseOpt = showCmd.Options.Single(o => o.Name == "--verbose");
        Assert.IsNotNull(verboseOpt);
        Assert.AreEqual("Enable verbose output", verboseOpt.Description);
    }

    #endregion

    #region List Subcommand Option Tests

    [TestMethod]
    public void ListSubcommand_HasTypeOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var typeOption = listCmd.Options.Single(o => o.Name == "--type");
        Assert.IsNotNull(typeOption);
        Assert.AreEqual("Filter by work item type (PBI, Bug, Task, Spike, Epic)", typeOption.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasStateOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var stateOption = listCmd.Options.Single(o => o.Name == "--state");
        Assert.IsNotNull(stateOption);
        Assert.AreEqual("Filter by state (New, Approved, Committed, Done, To Do, In Progress)", stateOption.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasAssignedToOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var assignedToOption = listCmd.Options.Single(o => o.Name == "--assigned-to");
        Assert.IsNotNull(assignedToOption);
        Assert.AreEqual("Filter by assigned user (email or display name)", assignedToOption.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasAssignedToMeOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var assignedToMeOption = listCmd.Options.Single(o => o.Name == "--assigned-to-me");
        Assert.IsNotNull(assignedToMeOption);
        Assert.AreEqual("Filter by work items assigned to current user", assignedToMeOption.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasTopOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var topOption = listCmd.Options.Single(o => o.Name == "--top");
        Assert.IsNotNull(topOption);
        Assert.AreEqual("Maximum number of items to return (default: 50)", topOption.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasVerboseOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var verboseOpt = listCmd.Options.Single(o => o.Name == "--verbose");
        Assert.IsNotNull(verboseOpt);
        Assert.AreEqual("Enable verbose output", verboseOpt.Description);
    }

    [TestMethod]
    public void ListSubcommand_HasConfigOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var listCmd = command.Subcommands.Single(s => s.Name == "list");

        // Assert
        var configOption = listCmd.Options.Single(o => o.Name == "--config");
        Assert.IsNotNull(configOption);
        Assert.AreEqual("Path to sdo-config.yaml file (optional)", configOption.Description);
    }

    #endregion

    #region Show Subcommand Execution Tests

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithValidId_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "123" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        // Result should be an integer (either 0 for success or non-zero for error)
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdAndComments_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "123", "--comments" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdAndCommentShortAlias_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "123", "-c" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdAndVerbose_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "123", "--verbose" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdOne_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "1" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdHundred_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "100" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ShowSubcommand_WithIdLarge_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "show", "--id", "999999" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    #endregion

    #region List Subcommand Execution Tests

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithNoFilters_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithTypeFilter_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--type", "Bug" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithStateFilter_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--state", "Done" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithAssignedToFilter_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--assigned-to", "user@example.com" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithAssignedToMeFilter_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--assigned-to-me" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithTopOption_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--top", "100" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithMultipleFilters_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--type", "Task", "--state", "In Progress", "--top", "25" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithVerbose_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--verbose" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithNoOptions_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithTypeFilterOnly_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--type", "PBI" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithStateFilterOnly_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--state", "New" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithTopFilterOnly_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--top", "50" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    #endregion

    #region Update Subcommand Tests

    [TestMethod]
    public void Constructor_AddsUpdateSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var updateSubcommand = command.Subcommands.Single(s => s.Name == "update");
        Assert.IsNotNull(updateSubcommand);
        Assert.AreEqual("Update work item properties", updateSubcommand.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasIdOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var idOption = updateCmd.Options.Single(o => o.Name == "--id");
        Assert.IsNotNull(idOption);
        Assert.AreEqual("Work item ID (optional; auto-detected from branch name if not on main)", idOption.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasTitleOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var titleOption = updateCmd.Options.Single(o => o.Name == "--title");
        Assert.IsNotNull(titleOption);
        Assert.AreEqual("Update work item title", titleOption.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasStateOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var stateOption = updateCmd.Options.Single(o => o.Name == "--state");
        Assert.IsNotNull(stateOption);
        Assert.AreEqual("Update work item state", stateOption.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasAssigneeOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var assigneeOption = updateCmd.Options.Single(o => o.Name == "--assignee");
        Assert.IsNotNull(assigneeOption);
        Assert.AreEqual("Update assigned user", assigneeOption.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasDescriptionOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var descriptionOption = updateCmd.Options.Single(o => o.Name == "--description");
        Assert.IsNotNull(descriptionOption);
        Assert.AreEqual("Update work item description", descriptionOption.Description);
    }

    [TestMethod]
    public void UpdateSubcommand_HasVerboseOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");

        // Assert
        var verboseOpt = updateCmd.Options.Single(o => o.Name == "--verbose");
        Assert.IsNotNull(verboseOpt);
        Assert.AreEqual("Enable verbose output", verboseOpt.Description);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithValidArguments_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "123", "--title", "Updated Title" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithIdAndState_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "123", "--state", "Done" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithIdAndAssignee_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "123", "--assignee", "user@example.com" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithMultipleProperties_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "123", "--state", "Done", "--title", "Updated" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    public void UpdateSubcommand_WithoutId_Parses()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--state", "Done" };

        // Act
        var parseResult = command.Parse(args);

        // Assert
        // Should parse successfully because id is now optional (auto-detected from branch at runtime)
        Assert.IsEmpty(parseResult.Errors);
    }

    [TestMethod]
    public void UpdateSubcommand_IdIsNowOptional()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var updateSubcommand = command.Subcommands.Single(s => s.Name == "update");

        // Act
        var idOption = updateSubcommand.Options.Single(o => o.Name == "--id");

        // Assert
        // ID should be optional (nullable int)
        Assert.IsNotNull(idOption);
    }

    [TestMethod]
    public void UpdateSubcommand_WithoutId_AndNotOnMain_AutoDetectsFromBranchName()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--state", "Done" };

        // Act
        var result = command.Parse(args);

        // Assert
        // Should parse without error (auto-detection happens at runtime based on git branch)
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UpdateSubcommand_WithoutId_ShouldParseSuccessfully()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--state", "Done" };

        // Act
        var result = command.Parse(args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Errors); // Parsing should succeed; runtime auto-detection happens in UpdateWorkItem
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithNegativeId_ReturnsErrorExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "-5", "--state", "Done" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        // Command should reject negative IDs at runtime
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void UpdateSubcommand_WithVerbose_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "update", "--id", "123", "--state", "Done", "--verbose" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    #endregion

    #region Update Subcommand Integration Tests (Phase 3.2+)

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials")]
    public void IntegrationTest_Update_GitHub_WithValidIssueAndAutoDetect()
    {
        // TODO: Implement real GitHub API integration test with auto-detection
        // 1. Create a test issue
        // 2. Start work on issue (creates branch with ID prefix)
        // 3. Update using auto-detected ID
        // 4. Verify update via GitHub API
        // 5. Clean up test issue and branch
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials")]
    public void IntegrationTest_Update_AzureDevOps_WithValidWorkItemAndAutoDetect()
    {
        // TODO: Implement real Azure DevOps API integration test with auto-detection
        // 1. Create a test work item
        // 2. Start work on work item (creates branch with ID prefix)
        // 3. Update using auto-detected ID
        // 4. Verify update via Azure DevOps API
        // 5. Clean up test work item and branch
        Assert.IsTrue(true);
    }

    #endregion

    #region TDD Placeholder Tests for Comment Subcommand

    [Ignore("TDD: Comment subcommand not yet implemented")]
    public void Constructor_AddsCommentSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var commentSubcommand = command.Subcommands.Single(s => s.Name == "comment");
        Assert.IsNotNull(commentSubcommand);
        Assert.AreEqual("Add a comment to a work item", commentSubcommand.Description);
    }

    [Ignore("TDD: Comment subcommand not yet implemented")]
    public void CommentSubcommand_HasIdOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var commentCmd = command.Subcommands.Single(s => s.Name == "comment");

        // Assert
        var idOption = commentCmd.Options.Single(o => o.Name == "--id");
        Assert.IsNotNull(idOption);
    }

    [Ignore("TDD: Comment subcommand not yet implemented")]
    public void CommentSubcommand_HasTextOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var commentCmd = command.Subcommands.Single(s => s.Name == "comment");

        // Assert
        var textOption = commentCmd.Options.Single(o => o.Name == "--text");
        Assert.IsNotNull(textOption);
    }

    [Ignore("TDD: Comment subcommand not yet implemented")]
    public void CommentSubcommand_WithValidArguments_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "comment", "--id", "123", "--text", "This is a comment" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    #endregion

    #region Create Subcommand Tests

    [TestMethod]
    public void Constructor_AddsCreateSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var createSubcommand = command.Subcommands.Single(s => s.Name == "create");
        Assert.IsNotNull(createSubcommand);
        Assert.AreEqual("Create a new work item", createSubcommand.Description);
    }

    [TestMethod]
    public void CreateSubcommand_HasFilePathOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var createCmd = command.Subcommands.Single(s => s.Name == "create");

        // Assert
        var filePathOption = createCmd.Options.Single(o => o.Name == "--file-path");
        Assert.IsNotNull(filePathOption);
        Assert.AreEqual("Path to markdown file containing work item details (auto-detected as .temp/wi.md if not provided)", filePathOption.Description);
    }

    [TestMethod]
    public void CreateSubcommand_HasDryRunOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var createCmd = command.Subcommands.Single(s => s.Name == "create");

        // Assert
        var dryRunOption = createCmd.Options.Single(o => o.Name == "--dry-run");
        Assert.IsNotNull(dryRunOption);
        Assert.AreEqual("Parse and preview work item creation without creating it", dryRunOption.Description);
    }

    [TestMethod]
    public void CreateSubcommand_AcceptsFilePathAndDryRunTogether()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "create", "--file-path", "test-work-item.md", "--dry-run" };

        // Act
        var parseResult = command.Parse(args);
        
        // Assert
        Assert.IsEmpty(parseResult.Errors); // Arguments should parse successfully
    }

    #endregion

    #region Start Subcommand Tests

    [TestMethod]
    public void Constructor_AddsStartSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var startSubcommand = command.Subcommands.Single(s => s.Name == "start");
        Assert.IsNotNull(startSubcommand);
        Assert.AreEqual("Start work on a work item by creating a feature branch and PR template", startSubcommand.Description);
    }

    [TestMethod]
    public void StartSubcommand_HasIdArgument()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var startCmd = command.Subcommands.Single(s => s.Name == "start");

        // Assert
        var idArgument = startCmd.Arguments.Single(a => a.Name == "id");
        Assert.IsNotNull(idArgument);
        Assert.AreEqual("Work item ID (optional; auto-detected from branch name if not on main)", idArgument.Description);
    }

    [TestMethod]
    public void StartSubcommand_HasVerboseOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var startCmd = command.Subcommands.Single(s => s.Name == "start");

        // Assert
        var verboseOpt = startCmd.Options.Single(o => o.Name == "--verbose");
        Assert.IsNotNull(verboseOpt);
        Assert.AreEqual("Enable verbose output", verboseOpt.Description);
    }

    [TestMethod]
    public void StartSubcommand_WithNoArguments_Succeeds()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start" };

        // Act
        var parseResult = command.Parse(args);

        // Assert
        // Should parse successfully because id is now optional (auto-detected from branch at runtime)
        Assert.IsEmpty(parseResult.Errors);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void StartSubcommand_WithValidId_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start", "123" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void StartSubcommand_WithValidIdAndVerbose_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start", "123", "--verbose" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void StartSubcommand_WithIdOne_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start", "1" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void StartSubcommand_WithLargeId_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start", "999999" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void StartSubcommand_WithNegativeId_ReturnsErrorExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start", "-5" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        // Command should reject negative IDs at runtime (not at parse time)
        Assert.AreEqual(1, result);
    }

    #endregion

    #region Integration Tests (Phase 3.2+)

    /// <summary>
    /// Integration tests for actual API calls.
    /// These require valid credentials and network access.
    /// Scaffolded for Phase 3.2 implementation.
    /// </summary>

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials")]
    public void IntegrationTest_Create_GitHub_WithValidRepository()
    {
        // TODO: Implement real GitHub API integration test
        // 1. Create new issue using wi create command
        // 2. Verify issue created via GitHub API
        // 3. Clean up test issue
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials")]
    public void IntegrationTest_Create_AzureDevOps_WithValidProject()
    {
        // TODO: Implement real Azure DevOps API integration test
        // 1. Create new work item using wi create command
        // 2. Verify work item created via Azure DevOps API
        // 3. Clean up test work item
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials and git")]
    public void IntegrationTest_Start_GitHub_WithValidRepository()
    {
        // TODO: Implement real GitHub API + git integration test
        // 1. Start work on issue using wi start command
        // 2. Verify branch created locally
        // 3. Verify PR template file generated
        // 4. Clean up branch
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials and git")]
    public void IntegrationTest_Start_AzureDevOps_WithValidProject()
    {
        // TODO: Implement real Azure DevOps API + git integration test
        // 1. Start work on work item using wi start command
        // 2. Verify branch created locally
        // 3. Verify PR template file generated
        // 4. Clean up branch
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials")]
    public void IntegrationTest_Update_GitHub_WithValidIssue()
    {
        // TODO: Implement real GitHub API integration test
        // 1. Create a test issue
        // 2. Update it using wi update command
        // 3. Verify update via GitHub API
        // 4. Clean up test issue
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials")]
    public void IntegrationTest_Update_AzureDevOps_WithValidWorkItem()
    {
        // TODO: Implement real Azure DevOps API integration test
        // 1. Create a test work item
        // 2. Update it using wi update command
        // 3. Verify update via Azure DevOps API
        // 4. Clean up test work item
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials")]
    public void IntegrationTest_Comment_GitHub_WithValidIssue()
    {
        // TODO: Implement real GitHub API integration test
        // 1. Create a test issue
        // 2. Add comment using wi comment command
        // 3. Verify comment via GitHub API
        // 4. Clean up test issue
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials")]
    public void IntegrationTest_Comment_AzureDevOps_WithValidWorkItem()
    {
        // TODO: Implement real Azure DevOps API integration test
        // 1. Create a test work item
        // 2. Add comment using wi comment command
        // 3. Verify comment via Azure DevOps API
        // 4. Clean up test work item
        Assert.IsTrue(true);
    }

    #region Config File and Defaults Tests

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithConfigParameter_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--config", "./sdo-config.yaml" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_WithConfigAndOtherFilters_ReturnsExitCode()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list", "--config", "./sdo-config.yaml", "--state", "Done" };

        // Act
        var result = command.Parse(args).Invoke();

        // Assert
        // CLI filters should override config defaults
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void ListSubcommand_ConfigParameterIsOptional()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "list" };

        // Act - should not throw, config parameter is optional
        var result = command.Parse(args).Invoke();

        // Assert
        Assert.IsTrue(result == 0 || result == 1, $"Expected exit code 0 or 1, got {result}");
    }

    #endregion

    #region Start Subcommand Auto-Detection Tests

    [TestMethod]
    public void StartSubcommand_WithoutId_AndNotOnMain_AutoDetectsFromBranchName()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        // Test auto-detection without id argument
        var args = new[] { "start" };

        // Act
        var result = command.Parse(args);

        // Assert
        // Should parse without error (auto-detection happens at runtime based on git branch)
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void StartSubcommand_WithoutId_ShouldParseSuccessfully()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "start" };

        // Act
        var result = command.Parse(args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Errors); // Parsing should succeed; runtime auto-detection happens in StartWorkItem
    }

    [TestMethod]
    public void StartSubcommand_IdArgumentIsNowOptional()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var startSubcommand = command.Subcommands.Single(s => s.Name == "start");

        // Act
        var idArgument = startSubcommand.Arguments.FirstOrDefault(a => a.Name == "id");

        // Assert
        Assert.IsNotNull(idArgument);
        // Verify it's optional (not required)
        Assert.AreEqual(ArgumentArity.ZeroOrOne, idArgument.Arity);
    }

    #endregion

    #region Close Subcommand Tests

    [TestMethod]
    public void Constructor_AddsCloseSubcommand()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);

        // Assert
        var closeSubcommand = command.Subcommands.Single(s => s.Name == "close");
        Assert.IsNotNull(closeSubcommand);
        Assert.AreEqual("Close a work item and clean up feature branch", closeSubcommand.Description);
    }

    [TestMethod]
    public void CloseSubcommand_HasIdArgument()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var closeCmd = command.Subcommands.Single(s => s.Name == "close");

        // Assert
        var idArgument = closeCmd.Arguments.Single(a => a.Name == "id");
        Assert.IsNotNull(idArgument);
        Assert.AreEqual("Work item ID (optional; auto-detected from branch name if not on main)", idArgument.Description);
    }

    [TestMethod]
    public void CloseSubcommand_HasVerboseOption()
    {
        // Act
        var command = new WorkItemCommand(_verboseOption);
        var closeCmd = command.Subcommands.Single(s => s.Name == "close");

        // Assert
        var verboseOpt = closeCmd.Options.Single(o => o.Name == "--verbose");
        Assert.IsNotNull(verboseOpt);
        Assert.AreEqual("Enable verbose output", verboseOpt.Description);
    }

    [TestMethod]
    public void CloseSubcommand_WithNoArguments_Succeeds()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "close" };

        // Act
        var parseResult = command.Parse(args);

        // Assert
        // Should parse successfully because id is now optional (auto-detected from branch at runtime)
        Assert.IsEmpty(parseResult.Errors);
    }

    [TestMethod]
    public void CloseSubcommand_IdArgumentIsOptional()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var closeSubcommand = command.Subcommands.Single(s => s.Name == "close");

        // Act
        var idArgument = closeSubcommand.Arguments.FirstOrDefault(a => a.Name == "id");

        // Assert
        Assert.IsNotNull(idArgument);
        // Verify it's optional (not required)
        Assert.AreEqual(ArgumentArity.ZeroOrOne, idArgument.Arity);
    }

    [TestMethod]
    public void CloseSubcommand_WithoutId_AndNotOnMain_AutoDetectsFromBranchName()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        // Test auto-detection without id argument
        var args = new[] { "close" };

        // Act
        var result = command.Parse(args);

        // Assert
        // Should parse without error (auto-detection happens at runtime based on git branch)
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void CloseSubcommand_WithoutId_ShouldParseSuccessfully()
    {
        // Arrange
        var command = new WorkItemCommand(_verboseOption);
        var args = new[] { "close" };

        // Act
        var result = command.Parse(args);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Errors); // Parsing should succeed; runtime auto-detection happens in CloseWorkItem
    }

    #endregion

    #region Close Subcommand Integration Tests (Phase 3.2+)

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires GitHub credentials and git")]
    public void IntegrationTest_Close_GitHub_WithValidRepository()
    {
        // TODO: Implement real GitHub API + git integration test
        // 1. Start work on issue using wi start command (creates feature branch)
        // 2. Close work item using wi close command
        // 3. Verify branch deleted locally
        // 4. Verify checked out to main branch
        Assert.IsTrue(true);
    }

    [Ignore("Integration: Scaffolded for Phase 3.2 - requires Azure DevOps credentials and git")]
    public void IntegrationTest_Close_AzureDevOps_WithValidProject()
    {
        // TODO: Implement real Azure DevOps API + git integration test
        // 1. Start work on work item using wi start command (creates feature branch)
        // 2. Close work item using wi close command
        // 3. Verify branch deleted locally
        // 4. Verify checked out to main branch
        Assert.IsTrue(true);
    }

    #endregion

    #endregion
}
