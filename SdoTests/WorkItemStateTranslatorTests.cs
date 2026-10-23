// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// WorkItemStateTranslatorTests.cs
//
// Unit tests for the WorkItemStateTranslator class.
// Tests state parsing, GitHub translation, and Azure DevOps translation.

using Sdo.Models;

namespace SdoTests;

/// <summary>
/// Unit tests for the WorkItemStateTranslator class.
/// Tests the state parsing, platform translation, and validation logic.
/// </summary>
[TestClass]
public class WorkItemStateTranslatorTests
{
    #region ParseState Tests

    [TestMethod]
    public void ParseState_WithValidNewState_ReturnsNewEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("new");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.New, result);
    }

    [TestMethod]
    public void ParseState_WithValidNewStateUppercase_ReturnsNewEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("NEW");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.New, result);
    }

    [TestMethod]
    public void ParseState_WithValidNewStateMixedCase_ReturnsNewEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("New");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.New, result);
    }

    [TestMethod]
    public void ParseState_WithValidApprovedState_ReturnsApprovedEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("approved");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Approved, result);
    }

    [TestMethod]
    public void ParseState_WithValidCommittedState_ReturnsCommittedEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("committed");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Committed, result);
    }

    [TestMethod]
    public void ParseState_WithValidDoneState_ReturnsDoneEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("done");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    [TestMethod]
    public void ParseState_WithValidClosedState_ReturnsDoneEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("closed");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    [TestMethod]
    public void ParseState_WithValidToDoState_ReturnsToDoEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("to do");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.ToDo, result);
    }

    [TestMethod]
    public void ParseState_WithValidToDoStateNoSpace_ReturnsToDoEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("todo");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.ToDo, result);
    }

    [TestMethod]
    public void ParseState_WithValidInProgressState_ReturnsInProgressEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("in progress");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.InProgress, result);
    }

    [TestMethod]
    public void ParseState_WithValidInProgressStateNoSpace_ReturnsInProgressEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("inprogress");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.InProgress, result);
    }

    [TestMethod]
    public void ParseState_WithNull_ReturnsNull()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseState_WithEmptyString_ReturnsNull()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseState_WithWhitespace_ReturnsNull()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("   ");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseState_WithInvalidState_ReturnsNull()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("invalid");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseState_WithStateContainingLeadingWhitespace_ReturnsValidEnum()
    {
        // Act
        var result = WorkItemStateTranslator.ParseState("  done  ");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    #endregion

    #region ToGitHubState Tests

    [TestMethod]
    public void ToGitHubState_WithNewState_ReturnsOpen()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.New);

        // Assert
        Assert.AreEqual("open", result);
    }

    [TestMethod]
    public void ToGitHubState_WithApprovedState_ReturnsOpen()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.Approved);

        // Assert
        Assert.AreEqual("open", result);
    }

    [TestMethod]
    public void ToGitHubState_WithCommittedState_ReturnsOpen()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.Committed);

        // Assert
        Assert.AreEqual("open", result);
    }

    [TestMethod]
    public void ToGitHubState_WithToDoState_ReturnsOpen()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.ToDo);

        // Assert
        Assert.AreEqual("open", result);
    }

    [TestMethod]
    public void ToGitHubState_WithInProgressState_ReturnsOpen()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.InProgress);

        // Assert
        Assert.AreEqual("open", result);
    }

    [TestMethod]
    public void ToGitHubState_WithDoneState_ReturnsClosed()
    {
        // Act
        var result = WorkItemStateTranslator.ToGitHubState(WorkItemState.Done);

        // Assert
        Assert.AreEqual("closed", result);
    }

    #endregion

    #region ToAzureDevOpsState Tests

    [TestMethod]
    public void ToAzureDevOpsState_WithNewState_ReturnsNew()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.New);

        // Assert
        Assert.AreEqual("New", result);
    }

    [TestMethod]
    public void ToAzureDevOpsState_WithApprovedState_ReturnsApproved()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.Approved);

        // Assert
        Assert.AreEqual("Approved", result);
    }

    [TestMethod]
    public void ToAzureDevOpsState_WithCommittedState_ReturnsCommitted()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.Committed);

        // Assert
        Assert.AreEqual("Committed", result);
    }

    [TestMethod]
    public void ToAzureDevOpsState_WithDoneState_ReturnsDone()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.Done);

        // Assert
        Assert.AreEqual("Done", result);
    }

    [TestMethod]
    public void ToAzureDevOpsState_WithToDoState_ReturnsToDoWithSpace()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.ToDo);

        // Assert
        Assert.AreEqual("To Do", result);
    }

    [TestMethod]
    public void ToAzureDevOpsState_WithInProgressState_ReturnsInProgressWithSpace()
    {
        // Act
        var result = WorkItemStateTranslator.ToAzureDevOpsState(WorkItemState.InProgress);

        // Assert
        Assert.AreEqual("In Progress", result);
    }

    #endregion

    #region Round-trip Conversion Tests

    [DataTestMethod]
    [DataRow("new", WorkItemState.New)]
    [DataRow("approved", WorkItemState.Approved)]
    [DataRow("committed", WorkItemState.Committed)]
    [DataRow("done", WorkItemState.Done)]
    [DataRow("to do", WorkItemState.ToDo)]
    [DataRow("in progress", WorkItemState.InProgress)]
    public void RoundTrip_ParseStateThenToAzureDevOps_PreservesState(string userInput, WorkItemState expected)
    {
        // Act
        var parsed = WorkItemStateTranslator.ParseState(userInput);
        Assert.IsNotNull(parsed);
        var adoState = WorkItemStateTranslator.ToAzureDevOpsState(parsed.Value);

        // Assert
        Assert.AreEqual(expected, parsed.Value);
        var expectedAdo = WorkItemStateTranslator.ToAzureDevOpsState(expected);
        Assert.AreEqual(expectedAdo, adoState);
    }

    [DataTestMethod]
    [DataRow("new", "open")]
    [DataRow("approved", "open")]
    [DataRow("committed", "open")]
    [DataRow("done", "closed")]
    [DataRow("to do", "open")]
    [DataRow("in progress", "open")]
    public void RoundTrip_ParseStateThenToGitHub_ProducesCorrectGitHubState(string userInput, string expectedGitHub)
    {
        // Act
        var parsed = WorkItemStateTranslator.ParseState(userInput);
        Assert.IsNotNull(parsed);
        var gitHubState = WorkItemStateTranslator.ToGitHubState(parsed.Value);

        // Assert
        Assert.AreEqual(expectedGitHub, gitHubState);
    }

    #endregion

    #region GetValidStatesForHelp Tests

    [TestMethod]
    public void GetValidStatesForHelp_ReturnsFormattedStateList()
    {
        // Act
        var result = WorkItemStateTranslator.GetValidStatesForHelp();

        // Assert
        Assert.IsNotNull(result);
        StringAssert.Contains(result, "New");
        StringAssert.Contains(result, "Approved");
        StringAssert.Contains(result, "Committed");
        StringAssert.Contains(result, "Done");
        StringAssert.Contains(result, "To Do");
        StringAssert.Contains(result, "In Progress");
    }

    [TestMethod]
    public void GetValidStatesForHelp_ContainsAllSixStates()
    {
        // Act
        var result = WorkItemStateTranslator.GetValidStatesForHelp();
        var states = result.Split(", ");

        // Assert
        Assert.AreEqual(6, states.Length);
    }

    #endregion

    #region Case Insensitivity Tests

    [DataTestMethod]
    [DataRow("DONE")]
    [DataRow("Done")]
    [DataRow("dONe")]
    public void ParseState_WithDifferentCasings_AllReturnDoneState(string input)
    {
        // Act
        var result = WorkItemStateTranslator.ParseState(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    [DataTestMethod]
    [DataRow("TO DO")]
    [DataRow("To Do")]
    [DataRow("TO do")]
    [DataRow("TODO")]
    [DataRow("Todo")]
    public void ParseState_WithDifferentCasingsForToDo_AllReturnToDoState(string input)
    {
        // Act
        var result = WorkItemStateTranslator.ParseState(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.ToDo, result);
    }

    [DataTestMethod]
    [DataRow("IN PROGRESS")]
    [DataRow("In Progress")]
    [DataRow("IN progress")]
    [DataRow("INPROGRESS")]
    [DataRow("InProgress")]
    public void ParseState_WithDifferentCasingsForInProgress_AllReturnInProgressState(string input)
    {
        // Act
        var result = WorkItemStateTranslator.ParseState(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.InProgress, result);
    }

    #endregion

    #region Closed as Done Alias Tests

    [TestMethod]
    public void ParseState_WithClosedAlias_ReturnsDoneState()
    {
        // Act & Assert
        var result = WorkItemStateTranslator.ParseState("closed");
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    [TestMethod]
    public void ParseState_WithClosedAliasUppercase_ReturnsDoneState()
    {
        // Act & Assert
        var result = WorkItemStateTranslator.ParseState("CLOSED");
        Assert.IsNotNull(result);
        Assert.AreEqual(WorkItemState.Done, result);
    }

    #endregion
}
