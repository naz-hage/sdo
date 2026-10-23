using Sdo.Commands;
using Sdo.Mapping;
using System.CommandLine;
using System.Reflection;

namespace SdoTests
{
    class TestMappingPresenter : IMappingPresenter
    {
        public string? Last { get; private set; }
        public void Present(string mapping)
        {
            Last = mapping;
        }
    }

    [TestClass]

    public class PullRequestCommandMappingTests
    {
        private readonly Option<bool> _verboseOption = new Option<bool>("--verbose");

        [TestMethod]
        public async Task CreateCommand_DryRun_PresentsMapping()
        {
            var presenter = new TestMappingPresenter();
            var generator = new Sdo.Mapping.MappingGenerator();
            var cmd = new PullRequestCommand(_verboseOption, generator, presenter);

            // Prepare temp markdown file
            var tmp = Path.GetTempFileName();
            File.WriteAllText(tmp, "# Test PR\n\nBody");

            // Force platform detector internals to GitHub with owner/repo
            var platField = typeof(PullRequestCommand).GetField("_platformDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(platField);
            var platInstance = platField.GetValue(cmd);
            Assert.IsNotNull(platInstance);
            // Set private fields on PlatformService
            var detField = platInstance.GetType().GetField("_detectedPlatform", BindingFlags.NonPublic | BindingFlags.Instance);
            var orgField = platInstance.GetType().GetField("_organization", BindingFlags.NonPublic | BindingFlags.Instance);
            var projField = platInstance.GetType().GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
            detField!.SetValue(platInstance, Sdo.Interfaces.Platform.GitHub);
            orgField!.SetValue(platInstance, "ownerX");
            projField!.SetValue(platInstance, "repoY");

            // Invoke private CreatePullRequest with dryRun=true and verbose=true
            var method = typeof(PullRequestCommand).GetMethod("CreatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(method);
            var task = (Task<int>)method.Invoke(cmd, new object[] { tmp, 1, false, true, true })!;
            var result = await task;
            Assert.AreEqual(0, result);

            // For dry-run with verbose, the presenter should have received the mapping
            // Verify that a mapping was presented (non-null and contains gh pr create command)
            Assert.IsNotNull(presenter.Last);
            StringAssert.Contains(presenter.Last, "gh pr create");
            StringAssert.Contains(presenter.Last, "ownerX/repoY");

            File.Delete(tmp);
        }

        [TestMethod]
        public async Task ListCommand_Verbose_PresentsMapping()
        {
            // Ensure env token exists so GetGitHubTokenAsync returns a non-null value
            Environment.SetEnvironmentVariable("GITHUB_TOKEN", "fake-token-for-tests");

            var presenter = new TestMappingPresenter();
            var generator = new Sdo.Mapping.MappingGenerator();
            var cmd = new PullRequestCommand(_verboseOption, generator, presenter);

            // Configure platform detector internals
            var platField = typeof(PullRequestCommand).GetField("_platformDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var platInstance = platField!.GetValue(cmd);
            var detField = platInstance!.GetType().GetField("_detectedPlatform", BindingFlags.NonPublic | BindingFlags.Instance);
            var orgField = platInstance.GetType().GetField("_organization", BindingFlags.NonPublic | BindingFlags.Instance);
            var projField = platInstance.GetType().GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
            detField!.SetValue(platInstance, Sdo.Interfaces.Platform.GitHub);
            orgField!.SetValue(platInstance, "ownerA");
            projField!.SetValue(platInstance, "repoB");

            var method = typeof(PullRequestCommand).GetMethod("ListPullRequests", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task<int>)method!.Invoke(cmd, new object[] { "open", 5, true })!;
            var result = await task;

            // mapping should have been presented
            var expected = generator.PrListGitHub("ownerA", "repoB", "open", 5);
            Assert.AreEqual(expected, presenter.Last);
        }

        [TestMethod]
        public async Task ShowCommand_Verbose_PresentsMapping()
        {
            Environment.SetEnvironmentVariable("GITHUB_TOKEN", "fake-token-for-tests");
            var presenter = new TestMappingPresenter();
            var generator = new Sdo.Mapping.MappingGenerator();
            var cmd = new PullRequestCommand(_verboseOption, generator, presenter);

            var platField = typeof(PullRequestCommand).GetField("_platformDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var platInstance = platField!.GetValue(cmd);
            var detField = platInstance!.GetType().GetField("_detectedPlatform", BindingFlags.NonPublic | BindingFlags.Instance);
            var orgField = platInstance.GetType().GetField("_organization", BindingFlags.NonPublic | BindingFlags.Instance);
            var projField = platInstance.GetType().GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
            detField!.SetValue(platInstance, Sdo.Interfaces.Platform.GitHub);
            orgField!.SetValue(platInstance, "ownerA");
            projField!.SetValue(platInstance, "repoB");

            var method = typeof(PullRequestCommand).GetMethod("ShowPullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task<int>)method!.Invoke(cmd, new object[] { 123, true })!;
            var result = await task;

            var expected = $"gh pr view -R ownerA/repoB 123";
            Assert.AreEqual(expected, presenter.Last);
        }

        [TestMethod]
        public async Task UpdateCommand_Verbose_PresentsMapping()
        {
            Environment.SetEnvironmentVariable("GITHUB_TOKEN", "fake-token-for-tests");
            var presenter = new TestMappingPresenter();
            var generator = new Sdo.Mapping.MappingGenerator();
            var cmd = new PullRequestCommand(_verboseOption, generator, presenter);

            var platField = typeof(PullRequestCommand).GetField("_platformDetector", BindingFlags.NonPublic | BindingFlags.Instance);
            var platInstance = platField!.GetValue(cmd);
            var detField = platInstance!.GetType().GetField("_detectedPlatform", BindingFlags.NonPublic | BindingFlags.Instance);
            var orgField = platInstance.GetType().GetField("_organization", BindingFlags.NonPublic | BindingFlags.Instance);
            var projField = platInstance.GetType().GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
            detField!.SetValue(platInstance, Sdo.Interfaces.Platform.GitHub);
            orgField!.SetValue(platInstance, "ownerA");
            projField!.SetValue(platInstance, "repoB");

            var method = typeof(PullRequestCommand).GetMethod("UpdatePullRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            var task = (Task<int>)method!.Invoke(cmd, new object[] { 321, null!, "New Title", "closed", true })!;
            var result = await task;

            var expectedStart = $"gh pr edit -R ownerA/repoB 321";
            Assert.IsNotNull(presenter.Last);
            StringAssert.StartsWith(presenter.Last!, expectedStart);
            StringAssert.Contains(presenter.Last, "--title \"New Title\"");
            StringAssert.Contains(presenter.Last, "--state closed");
        }
    }
}
