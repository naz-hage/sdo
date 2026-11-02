using Sdo.Commands;
using System.CommandLine;
using System.Reflection;

namespace SdoTests;

[TestClass]

public class PullRequestCommandTests
{
    private readonly Option<bool> _verboseOption;

    public PullRequestCommandTests()
    {
        _verboseOption = new Option<bool>("--verbose");
    }

    /// <summary>
    /// Captures console output for testing error messages
    /// </summary>
    private class ConsoleOutputCapture : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOut;

        public ConsoleOutputCapture()
        {
            _stringWriter = new StringWriter();
            _originalOut = Console.Out;
            Console.SetOut(_stringWriter);
        }

        public string GetOutput() => _stringWriter.ToString();

        public void Dispose()
        {
            Console.SetOut(_originalOut);
            _stringWriter?.Dispose();
        }
    }

    [TestMethod]
    public void Constructor_CreatesCommandWithCorrectName()
    {
        var command = new PullRequestCommand(_verboseOption);
        Assert.AreEqual("pr", command.Name);
    }

    [TestMethod]
    public void Constructor_AddsCreateSubcommand()
    {
        var command = new PullRequestCommand(_verboseOption);
        var createCmd = command.Subcommands.Single(s => s.Name == "create");
        Assert.IsNotNull(createCmd);
    }

    [TestMethod]
    public void Constructor_AddsLsSubcommand()
    {
        var command = new PullRequestCommand(_verboseOption);
        var lsCmd = command.Subcommands.Single(s => s.Name == "list");
        Assert.IsNotNull(lsCmd);
    }

    [TestMethod]
    public void Constructor_AddsShowSubcommand()
    {
        var command = new PullRequestCommand(_verboseOption);
        var showCmd = command.Subcommands.Single(s => s.Name == "show");
        Assert.IsNotNull(showCmd);
    }

    [TestMethod]
    public void Constructor_AddsStatusSubcommand()
    {
        var command = new PullRequestCommand(_verboseOption);
        var statusCmd = command.Subcommands.Single(s => s.Name == "status");
        Assert.IsNotNull(statusCmd);
    }

    [TestMethod]
    public void Constructor_AddsUpdateSubcommand()
    {
        var command = new PullRequestCommand(_verboseOption);
        var updateCmd = command.Subcommands.Single(s => s.Name == "update");
        Assert.IsNotNull(updateCmd);
    }

    [TestMethod]
    public void Constructor_RegistersSubcommandsInAlphabeticalOrder()
    {
        var command = new PullRequestCommand(_verboseOption);
        var subcommandNames = command.Subcommands.Select(s => s.Name).ToList();
        Assert.IsTrue(new[] { "create", "list", "show", "status", "update" }.SequenceEqual(subcommandNames));
    }

    [TestMethod]
    public void CreateSubcommand_HasFileOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var fileOption = createCmd.Options.FirstOrDefault(o => o.Name.Contains("file"));
        Assert.IsNotNull(fileOption);
    }

    [TestMethod]
    public void CreateSubcommand_HasWorkItemOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var workItemOption = createCmd.Options.FirstOrDefault(o => o.Name.Contains("work-item"));
        Assert.IsNotNull(workItemOption);
    }

    [TestMethod]
    public void CreateSubcommand_HasDraftOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var draftOption = createCmd.Options.FirstOrDefault(o => o.Name.Contains("draft"));
        Assert.IsNotNull(draftOption);
    }

    [TestMethod]
    public void CreateSubcommand_HasDryRunOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var createCmd = command.Subcommands.First(s => s.Name == "create");
        var dryRunOption = createCmd.Options.FirstOrDefault(o => o.Name.Contains("dry-run"));
        Assert.IsNotNull(dryRunOption);
    }

    [TestMethod]
    public void LsSubcommand_HasStatusOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var lsCmd = command.Subcommands.First(s => s.Name == "list");
        var statusOption = lsCmd.Options.FirstOrDefault(o => o.Name.Contains("status"));
        Assert.IsNotNull(statusOption);
    }

    [TestMethod]
    public void LsSubcommand_HasTopOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var lsCmd = command.Subcommands.First(s => s.Name == "list");
        var topOption = lsCmd.Options.FirstOrDefault(o => o.Name.Contains("top"));
        Assert.IsNotNull(topOption);
    }

    [TestMethod]
    public void ShowSubcommand_HasPrIdArgument()
    {
        var command = new PullRequestCommand(_verboseOption);
        var showCmd = command.Subcommands.First(s => s.Name == "show");
        var prIdArg = showCmd.Arguments.FirstOrDefault(a => a.Name == "pr-id");
        Assert.IsNotNull(prIdArg);
    }

    [TestMethod]
    public void StatusSubcommand_HasPrNumberArgument()
    {
        var command = new PullRequestCommand(_verboseOption);
        var statusCmd = command.Subcommands.First(s => s.Name == "status");
        var prArg = statusCmd.Arguments.FirstOrDefault(a => a.Name == "pr-id");
        Assert.IsNotNull(prArg);
    }

    [TestMethod]
    public void UpdateSubcommand_HasPrIdOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var updateCmd = command.Subcommands.First(s => s.Name == "update");
        var prIdOption = updateCmd.Options.FirstOrDefault(o => o.Name.Contains("pr-id"));
        Assert.IsNotNull(prIdOption);
    }

    [TestMethod]
    public void UpdateSubcommand_HasFileOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var updateCmd = command.Subcommands.First(s => s.Name == "update");
        var fileOption = updateCmd.Options.FirstOrDefault(o => o.Name.Contains("file"));
        Assert.IsNotNull(fileOption);
    }

    [TestMethod]
    public void UpdateSubcommand_HasTitleOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var updateCmd = command.Subcommands.First(s => s.Name == "update");
        var titleOption = updateCmd.Options.FirstOrDefault(o => o.Name.Contains("title"));
        Assert.IsNotNull(titleOption);
    }

    [TestMethod]
    public void UpdateSubcommand_HasStatusOption()
    {
        var command = new PullRequestCommand(_verboseOption);
        var updateCmd = command.Subcommands.First(s => s.Name == "update");
        var statusOption = updateCmd.Options.FirstOrDefault(o => o.Name.Contains("status"));
        Assert.IsNotNull(statusOption);
    }

    // Error Handling Tests for PR ID Validation

    [TestMethod]
    public async Task UpdatePullRequest_WithZeroPrId_ReturnsErrorCode()
    {
        using (new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("UpdatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method!.Invoke(command, new object[] { 0, null!, null!, null!, false })!;
            var result = await task;

            // Should return error code 1
            Assert.AreEqual(1, result);
        }
    }

    [TestMethod]
    public async Task UpdatePullRequest_WithNegativePrId_ReturnsErrorCode()
    {
        using (new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("UpdatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method!.Invoke(command, new object[] { -1, null!, null!, null!, false })!;
            var result = await task;

            // Should return error code 1
            Assert.AreEqual(1, result);
        }
    }

    [TestMethod]
    public async Task UpdatePullRequest_WithZeroPrId_DisplaysHelpfulErrorMessage()
    {
        using (var capture = new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("UpdatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method!.Invoke(command, new object[] { 0, null!, null!, null!, false })!;
            await task;

            var output = capture.GetOutput();
            
            // Verify error message contains key information
            StringAssert.Contains(output, "PR ID is required");
            StringAssert.Contains(output, "--pr-id");
            StringAssert.Contains(output, "Example:");
            StringAssert.Contains(output, "sdo pr update --pr-id 123");
        }
    }

    [TestMethod]
    public async Task ShowPullRequest_WithZeroPrId_ReturnsErrorCode()
    {
        using (new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("ShowPullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method.Invoke(command, new object[] { 0, false })!;
            var result = await task;

            // Should return error code 1
            Assert.AreEqual(1, result);
        }
    }

    [TestMethod]
    public async Task ShowPullRequest_WithNegativePrId_ReturnsErrorCode()
    {
        using (new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("ShowPullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method.Invoke(command, new object[] { -1, false })!;
            var result = await task;

            // Should return error code 1
            Assert.AreEqual(1, result);
        }
    }

    [TestMethod]
    public async Task ShowPullRequest_WithZeroPrId_DisplaysHelpfulErrorMessage()
    {
        using (var capture = new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("ShowPullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method.Invoke(command, new object[] { 0, false })!;
            await task;

            var output = capture.GetOutput();
            
            // Verify error message contains key information
            StringAssert.Contains(output, "PR ID is required");
            StringAssert.Contains(output, "positional argument");
            StringAssert.Contains(output, "--pr-id");
            StringAssert.Contains(output, "Example:");
            StringAssert.Contains(output, "sdo pr show 123");
        }
    }

    [TestMethod]
    public async Task UpdatePullRequest_WithZeroPrId_ShowsExamples()
    {
        using (var capture = new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("UpdatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method!.Invoke(command, new object[] { 0, null!, null!, null!, false })!;
            await task;

            var output = capture.GetOutput();
            
            // Verify both examples are shown
            StringAssert.Contains(output, "sdo pr update --pr-id 123 -f ./pr-message.md");
            StringAssert.Contains(output, "sdo pr update --pr-id 123 --title");
        }
    }

    [TestMethod]
    public async Task ShowPullRequest_WithZeroPrId_ShowsExamples()
    {
        using (var capture = new ConsoleOutputCapture())
        {
            var command = new PullRequestCommand(_verboseOption);
            var method = typeof(PullRequestCommand).GetMethod("ShowPullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);

            var task = (Task<int>)method.Invoke(command, new object[] { 0, false })!;
            await task;

            var output = capture.GetOutput();
            
            // Verify both examples are shown
            StringAssert.Contains(output, "sdo pr show 123");
            StringAssert.Contains(output, "sdo pr show --pr-id 123");
        }
    }

    /// <summary>
    /// Tests for PR auto-detection feature
    /// </summary>
    [TestClass]
    public class PullRequestAutoDetectionTests
    {
        private readonly Option<bool> _verboseOption;
        private readonly PullRequestCommand _command;

        public PullRequestAutoDetectionTests()
        {
            _verboseOption = new Option<bool>("--verbose");
            _command = new PullRequestCommand(_verboseOption);
        }

        [TestMethod]
        [DataRow("244-issue", 244)]
        [DataRow("123-feature-name", 123)]
        [DataRow("1-bugfix", 1)]
        [DataRow("001-test", 1)]
        [DataRow("999-very-long-branch-name-with-many-words", 999)]
        public void ExtractWorkItemIdFromBranch_WithValidBranch_ReturnsCorrectId(string branchName, int expectedId)
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ExtractWorkItemIdFromBranch",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (int?)method.Invoke(_command, new object[] { branchName });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedId, result.Value);
        }

        [TestMethod]
        [DataRow("main")]
        [DataRow("feature")]
        [DataRow("develop")]
        [DataRow("")]
        [DataRow("no-number-here")]
        [DataRow("-123-starts-with-dash")]
        public void ExtractWorkItemIdFromBranch_WithInvalidBranch_ReturnsNull(string branchName)
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ExtractWorkItemIdFromBranch",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (int?)method.Invoke(_command, new object[] { branchName });

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExtractWorkItemIdFromBranch_WithLeadingZeros_ReturnsCorrectId()
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ExtractWorkItemIdFromBranch",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (int?)method.Invoke(_command, new object[] { "0001-feature" });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public void ExtractWorkItemIdFromBranch_WithNullBranch_ReturnsNull()
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ExtractWorkItemIdFromBranch",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (int?)method.Invoke(_command, new object[] { null! });

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow(244)]
        [DataRow(123)]
        [DataRow(1)]
        public void ConstructDefaultFilePath_WithValidId_ReturnsCorrectPath(int workItemId)
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ConstructDefaultFilePath",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (string?)method.Invoke(_command, new object[] { workItemId });

            // Assert
            Assert.IsNotNull(result);
            // Normalize path separators for cross-platform compatibility
            var normalizedResult = result.Replace("/", "\\");
            StringAssert.EndsWith(normalizedResult, $"{workItemId}-pr-message.md");
            StringAssert.Contains(normalizedResult, ".temp");
        }

        [TestMethod]
        public void ConstructDefaultFilePath_ContainsWorkItemId()
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ConstructDefaultFilePath",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);
            int workItemId = 456;

            // Act
            var result = (string?)method.Invoke(_command, new object[] { workItemId });

            // Assert
            StringAssert.Contains(result, "456");
            StringAssert.Contains(result, "-pr-message.md");
        }

        [TestMethod]
        public void ConstructDefaultFilePath_ContainsTempDirectory()
        {
            // Arrange
            var method = typeof(PullRequestCommand).GetMethod("ConstructDefaultFilePath",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            Assert.IsNotNull(method);

            // Act
            var result = (string?)method.Invoke(_command, new object[] { 100 });

            // Assert
            StringAssert.Contains(result, ".temp");
        }
    }
}
