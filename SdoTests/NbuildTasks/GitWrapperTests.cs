using Launcher.Services;
using Ntools;
using Sdo;
using Sdo.Git;
using System.Diagnostics;
using static NbuildTasks.Enums;

namespace NbuildTasks.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class GitWrapperTests
    {
        private const string InvalidUrl = "invalid-url";
        private static readonly string ProjectName = "getting-started-" + Guid.NewGuid().ToString("N");
        private static string FixtureRoot = string.Empty;
        private static string BareRepository = string.Empty;
        private static string RepositoryUrl = string.Empty;

        private GitWrapper GitWrapper => new(ProjectName, verbose: true, testMode: true);

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            FixtureRoot = Path.Combine(Path.GetTempPath(), "NbuildTasksTests", "git-fixture-" + Guid.NewGuid().ToString("N"));
            var sourceRepository = Path.Combine(FixtureRoot, "source");
            BareRepository = Path.Combine(FixtureRoot, ProjectName + ".git");
            RepositoryUrl = new Uri(BareRepository).AbsoluteUri;

            Directory.CreateDirectory(FixtureRoot);
            RunGit(FixtureRoot, "init --bare " + Quote(BareRepository));
            RunGit(FixtureRoot, "init -b main " + Quote(sourceRepository));
            RunGit(sourceRepository, "config user.name nbuild-tests");
            RunGit(sourceRepository, "config user.email nbuild-tests@example.invalid");
            File.WriteAllText(Path.Combine(sourceRepository, "README.md"), "local GitWrapper fixture");
            RunGit(sourceRepository, "add README.md");
            RunGit(sourceRepository, "commit -m initial");
            RunGit(sourceRepository, "tag 1.0.0");
            RunGit(sourceRepository, "remote add origin " + Quote(BareRepository));
            RunGit(sourceRepository, "push --set-upstream origin main");
            RunGit(sourceRepository, "push origin 1.0.0");
            RunGit(BareRepository, "-c safe.bareRepository=all symbolic-ref HEAD refs/heads/main");

            var workingRepository = Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName);
            DeleteDirectory(workingRepository);
            RunGit(Path.GetDirectoryName(workingRepository)!, "clone --branch main " + Quote(BareRepository) + " " + Quote(workingRepository));
            RunGit(workingRepository, "config user.name nbuild-tests");
            RunGit(workingRepository, "config user.email nbuild-tests@example.invalid");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _ = new GitWrapper(project: null, testMode: true);
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            DeleteDirectory(FixtureRoot);
            DeleteDirectory(Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Directory.SetCurrentDirectory(Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        [TestMethod]
        public void CloneProject_ShouldSucceed_WithValidUrl()
        {
            var sourceDirectory = CreateTestDirectory();
            var result = new GitWrapper(verbose: true).CloneProject(RepositoryUrl, sourceDirectory);

            Assert.IsTrue(result.IsSuccess());
            Assert.IsTrue(Directory.Exists(Path.Combine(sourceDirectory, ProjectName)));
        }

        [TestMethod]
        public void CloneProject_ShouldFail_WithInvalidUrl()
        {
            var result = new GitWrapper(verbose: true).CloneProject(InvalidUrl, CreateTestDirectory());

            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual(ResultHelper.InvalidParameter, result.Code);
        }

        [TestMethod]
        public void CloneProject_ShouldFail_WhenProjectAlreadyExists()
        {
            var sourceDirectory = CreateTestDirectory();
            Directory.CreateDirectory(Path.Combine(sourceDirectory, ProjectName));

            var result = new GitWrapper(verbose: true).CloneProject(RepositoryUrl, sourceDirectory);

            Assert.IsFalse(result.IsSuccess());
            Assert.AreEqual((int)RetCode.CloneProjectFailed, result.Code);
        }

        [TestMethod]
        public void CloneProject_ShouldCreateSourceDir_IfNotExists()
        {
            var sourceDirectory = Path.Combine(CreateTestDirectory(), "nested");
            var result = new GitWrapper(verbose: true).CloneProject(RepositoryUrl, sourceDirectory);

            Assert.IsTrue(result.IsSuccess());
            Assert.IsTrue(Directory.Exists(sourceDirectory));
        }

        [TestMethod]
        public void GetCurrentBranchTest() => Assert.AreEqual("main", GitWrapper.Branch);

        [TestMethod]
        public void GetBranchCommand_PrintsOnlyTheCurrentBranch()
        {
            var expectedBranch = GitWrapper.Branch;
            var originalOutput = Console.Out;
            using var output = new StringWriter();

            try
            {
                Console.SetOut(output);
                var result = Nbuild.Command.GetBranch();

                Assert.IsTrue(result.IsSuccess());
                Assert.AreEqual(expectedBranch + Environment.NewLine, output.ToString());
            }
            finally
            {
                Console.SetOut(originalOutput);
            }
        }

        [TestMethod]
        public void GetCurrentTagTest() => Assert.AreEqual("1.0.0", GitWrapper.Tag);

        [TestMethod]
        public void GetTagCommand_PrintsOnlyTheCurrentTag()
        {
            var expectedTag = GitWrapper.Tag;
            var originalOutput = Console.Out;
            using var output = new StringWriter();

            try
            {
                Console.SetOut(output);
                var result = Nbuild.Command.GetTag();

                Assert.IsTrue(result.IsSuccess());
                Assert.AreEqual(expectedTag + Environment.NewLine, output.ToString());
            }
            finally
            {
                Console.SetOut(originalOutput);
            }
        }

        [TestMethod]
        public void ProjectNameFromUrlTest()
        {
            Assert.AreEqual(ProjectName, GitWrapper.ProjectNameFromUrl(RepositoryUrl));
            AssertArgumentException(() => GitWrapper.ProjectNameFromUrl(null));
        }

        [TestMethod]
        public void WorkingDirectoryTest()
        {
            var wrapper = GitWrapper;
            var originalDirectory = wrapper.WorkingDirectory;
            var testDirectory = CreateTestDirectory();

            wrapper.WorkingDirectory = testDirectory;

            Assert.AreEqual(testDirectory, wrapper.WorkingDirectory);
            wrapper.WorkingDirectory = originalDirectory;
        }

        [TestMethod]
        public void SetAutoTagTest()
        {
            var tag = GitWrapper.SetAutoTag(BuildType.STAGE.ToString());

            Assert.AreEqual(tag, GitWrapper.Tag);
            Assert.IsTrue(GitWrapper.IsValidTag(tag));
            Assert.IsTrue(GitWrapper.LocalTagExists(tag));
            Assert.IsFalse(GitWrapper.RemoteTagExists(tag));
        }

        [TestMethod]
        public void SetTag_CreatesOnlyLocalTag()
        {
            var tag = "7.7." + (DateTime.UtcNow.Ticks % 1000000);

            Assert.IsTrue(GitWrapper.SetTag(tag));
            Assert.IsTrue(GitWrapper.LocalTagExists(tag));
            Assert.IsFalse(GitWrapper.RemoteTagExists(tag));
        }

        [TestMethod]
        public void ProgramMain_WithQuietTagSet_PrintsOnlyTheSetTag()
        {
            var tag = $"8.8.{Random.Shared.Next(100000, int.MaxValue)}";
            var git = new GitWrapper(ProjectName, testMode: true, quiet: true);
            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();
            using var error = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetError(error);

                Assert.AreEqual(0, Program.Main("git", "tag", "set", "--tag", tag, "-q"));
                Assert.AreEqual($"{tag}{Environment.NewLine}", output.ToString());
                Assert.AreEqual(string.Empty, error.ToString());
                Assert.IsTrue(git.LocalTagExists(tag));
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
                if (git.LocalTagExists(tag)) git.DeleteTag(tag);
            }
        }

        [TestMethod]
        [DataRow("stage")]
        [DataRow("prod")]
        public void ProgramMain_WithQuietAutoTag_PrintsOnlyTheCreatedTag(string buildType)
        {
            var git = new GitWrapper(ProjectName, testMode: true, quiet: true);
            var seedTag = $"8.{Random.Shared.Next(100000, 900000)}.{Random.Shared.Next(100000, int.MaxValue)}";
            Assert.IsTrue(git.SetTag(seedTag));
            var expectedTag = git.AutoTag(buildType);
            Assert.IsFalse(string.IsNullOrWhiteSpace(expectedTag));

            var tagFile = Path.Combine(git.WorkingDirectory, ".temp", ".tag");
            var tagFileExisted = File.Exists(tagFile);
            var previousTagFile = tagFileExisted ? File.ReadAllText(tagFile) : null;
            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();
            using var error = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetError(error);

                Assert.AreEqual(0, Program.Main("git", "tag", "auto", "--buildtype", buildType, "-q"));
                Assert.AreEqual($"{expectedTag}{Environment.NewLine}", output.ToString());
                Assert.AreEqual(string.Empty, error.ToString());
                Assert.IsTrue(git.LocalTagExists(expectedTag));
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
                if (git.LocalTagExists(expectedTag)) git.DeleteTag(expectedTag);
                if (git.LocalTagExists(seedTag)) git.DeleteTag(seedTag);

                if (tagFileExisted)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tagFile)!);
                    File.WriteAllText(tagFile, previousTagFile!);
                }
                else if (File.Exists(tagFile))
                {
                    File.Delete(tagFile);
                }
            }
        }

        [TestMethod]
        public void ProgramMain_WithQuietAutoTagDryRun_PrintsCandidateWithoutCreatingIt()
        {
            var git = new GitWrapper(ProjectName, testMode: true, quiet: true);
            var expectedTag = git.AutoTag("stage");
            var originalOutput = Console.Out;
            using var output = new StringWriter();

            try
            {
                Console.SetOut(output);

                Assert.AreEqual(0, Program.Main("git", "tag", "auto", "--buildtype", "stage", "--dry-run", "-q"));
                Assert.AreEqual($"{expectedTag}{Environment.NewLine}", output.ToString());
                Assert.IsFalse(git.LocalTagExists(expectedTag));
            }
            finally
            {
                Console.SetOut(originalOutput);
            }
        }

        [TestMethod]
        public void ProgramMain_WithQuietTagDelete_IsSilentAndPreservesExitStatus()
        {
            var git = new GitWrapper(ProjectName, testMode: true, quiet: true);
            var tag = $"8.8.{Random.Shared.Next(100000, int.MaxValue)}";
            var missingTag = $"8.8.{Random.Shared.Next(100000, int.MaxValue)}";
            Assert.IsTrue(git.SetTag(tag));

            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();
            using var error = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetError(error);

                Assert.AreEqual(0, Program.Main("git", "tag", "delete", "--tag", tag, "-q"));
                Assert.AreEqual(string.Empty, output.ToString());
                Assert.AreEqual(string.Empty, error.ToString());
                Assert.IsFalse(git.LocalTagExists(tag));

                output.GetStringBuilder().Clear();
                error.GetStringBuilder().Clear();
                Assert.AreNotEqual(0, Program.Main("git", "tag", "delete", "--tag", missingTag, "-q"));
                Assert.AreEqual(string.Empty, output.ToString());
                Assert.AreEqual(string.Empty, error.ToString());
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
                if (git.LocalTagExists(tag)) git.DeleteTag(tag);
            }
        }

        [TestMethod]
        public void ProgramMain_WithQuietPushTag_PrintsOnlyPushedTag()
        {
            var git = new GitWrapper(ProjectName, testMode: true, quiet: true);
            var tag = $"8.8.{Random.Shared.Next(100000, int.MaxValue)}";
            Assert.IsTrue(git.SetTag(tag));

            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();
            using var error = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetError(error);

                Assert.AreEqual(0, Program.Main("git", "tag", "push", "--tag", tag, "-q"));
                Assert.AreEqual($"{tag}{Environment.NewLine}", output.ToString());
                Assert.AreEqual(string.Empty, error.ToString());
                Assert.IsTrue(git.RemoteTagExists(tag));
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
                if (git.RemoteTagExists(tag)) git.DeleteTag(tag, deleteRemote: true);
                else if (git.LocalTagExists(tag)) git.DeleteTag(tag);
            }
        }

        [TestMethod]
        public void ProgramMain_WithQuietTagList_KeepsListOutput()
        {
            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();
            using var error = new StringWriter();

            try
            {
                Console.SetOut(output);
                Console.SetError(error);

                Assert.AreEqual(0, Program.Main("git", "tag", "list", "-q"));
                StringAssert.Contains(output.ToString(), "1.0.0");
                Assert.IsFalse(output.ToString().Contains(Nversion.Get(), StringComparison.Ordinal));
                Assert.IsFalse(output.ToString().Contains("git ", StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(string.Empty, error.ToString());
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
            }
        }

        [TestMethod]
        public void IsValidTagTest()
        {
            foreach (var tag in new[] { "1.1.1", "9999.999.000", "11.1.1" })
            {
                Assert.IsTrue(GitWrapper.IsValidTag(tag));
            }

            foreach (var tag in new[] { "A.1.1.1", "9.b.1.1", "1.1.mk", "", null })
            {
                Assert.IsFalse(GitWrapper.IsValidTag(tag));
            }
        }

        [TestMethod]
        public void IsValid4TagTest()
        {
            Assert.IsTrue(GitWrapper.IsValid4Tag("1.2.3.4"));
            Assert.IsFalse(GitWrapper.IsValid4Tag("1.2.3"));
            Assert.IsFalse(GitWrapper.IsValid4Tag("1.2.3.x"));
            Assert.IsFalse(GitWrapper.IsValid4Tag(null));
        }

        [TestMethod]
        public void AutoTagTest()
        {
            Assert.AreEqual(string.Empty, GitWrapper.AutoTag(null));
            AssertArgumentException(() => GitWrapper.AutoTag("unknown"));
        }

        [TestMethod]
        public void StageTagTest()
        {
            var currentTag = GitWrapper.Tag;
            var expectedTag = $"{currentTag.Split('.')[0]}.{currentTag.Split('.')[1]}.{int.Parse(currentTag.Split('.')[2]) + 1}";
            var nextTag = GitWrapper.StageTag();

            Assert.AreEqual(expectedTag, nextTag);
            Assert.IsTrue(GitWrapper.IsValidTag(nextTag));
        }

        [TestMethod]
        public void ProdTagTest()
        {
            var currentTag = GitWrapper.Tag;
            var expectedTag = $"{currentTag.Split('.')[0]}.{int.Parse(currentTag.Split('.')[1]) + 1}.0";
            var nextTag = GitWrapper.ProdTag();

            Assert.AreEqual(expectedTag, nextTag);
            Assert.IsTrue(GitWrapper.IsValidTag(nextTag));
        }

        [TestMethod]
        public void PullWithRebaseTest() => Assert.IsTrue(GitWrapper.PullWithRebase());

        [TestMethod]
        public void DeleteTagTest()
        {
            var tag = "9.9." + (DateTime.UtcNow.Ticks % 1000000);
            Assert.IsTrue(GitWrapper.SetTag(tag));

            Assert.IsTrue(GitWrapper.DeleteTag(tag));
            Assert.IsFalse(GitWrapper.DeleteTag(tag));
        }

        [TestMethod]
        public void TagLookupTests()
        {
            var tag = "8.8." + (DateTime.UtcNow.Ticks % 1000000);
            RunGit(Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName), "tag " + tag);
            RunGit(Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName), "push origin " + tag);

            Assert.IsTrue(GitWrapper.LocalTagExists(tag));
            Assert.IsTrue(GitWrapper.RemoteTagExists(tag));
            Assert.IsTrue(GitWrapper.ListLocalTags().Contains(tag));
            Assert.IsTrue(GitWrapper.ListRemoteTags().Contains(tag));

            Assert.IsTrue(GitWrapper.DeleteTag(tag));
            Assert.IsFalse(GitWrapper.LocalTagExists(tag));
            Assert.IsTrue(GitWrapper.RemoteTagExists(tag));

            Assert.IsTrue(GitWrapper.DeleteTag(tag, deleteRemote: true));
            Assert.IsFalse(GitWrapper.RemoteTagExists(tag));
        }

        [TestMethod]
        public void ListBranchesTest() => Assert.IsTrue(GitWrapper.ListBranches().Contains("main"));

        [TestMethod]
        public void TryListLocalTags_ReturnsAllTagsInDescendingVersionOrder()
        {
            var expectedAddedTags = Enumerable.Range(0, 11)
                .Reverse()
                .Select(version => $"9999.0.{version}")
                .ToList();
            var tagsToAdd = Enumerable.Range(0, 11)
                .Select(version => $"9999.0.{version}")
                .ToList();

            try
            {
                foreach (var tag in tagsToAdd)
                {
                    Assert.IsTrue(GitWrapper.SetTag(tag), $"Could not create fixture tag {tag}.");
                }

                Assert.IsTrue(GitWrapper.TryListLocalTags(out var allTags));
                CollectionAssert.AreEqual(expectedAddedTags, allTags.Take(expectedAddedTags.Count).ToList());
                Assert.IsTrue(allTags.Contains("1.0.0"));
            }
            finally
            {
                foreach (var tag in tagsToAdd)
                {
                    GitWrapper.DeleteTag(tag);
                }
            }
        }

        [TestMethod]
        public void ListTags_ListsLatestTagsAndShowsLocalRemoteDiscrepancies()
        {
            var minorVersion = 1000 + (int)(DateTime.UtcNow.Ticks % 8000);
            var localOnlyTags = Enumerable.Range(0, 11)
                .Select(version => $"9999.{minorVersion}.{version}")
                .ToList();
            var expectedLatestTags = Enumerable.Range(1, 10)
                .Reverse()
                .Select(version => $"9999.{minorVersion}.{version}")
                .ToList();
            var remoteOnlyTag = $"9998.{minorVersion}.0";
            var originalOutput = Console.Out;
            using var output = new StringWriter();

            try
            {
                foreach (var tag in localOnlyTags)
                {
                    Assert.IsTrue(GitWrapper.SetTag(tag), $"Could not create fixture tag {tag}.");
                }

                Assert.IsTrue(GitWrapper.SetTag(remoteOnlyTag));
                Assert.IsTrue(GitWrapper.PushTag(remoteOnlyTag));
                Assert.IsTrue(GitWrapper.DeleteTag(remoteOnlyTag));

                Console.SetOut(output);
                var result = Nbuild.Command.ListTags();

                Assert.IsTrue(result.IsSuccess());
                var outputLines = output.ToString()
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var listedTags = outputLines.Where(GitWrapper.IsValidTag).Take(10).ToList();
                CollectionAssert.AreEqual(
                    expectedLatestTags,
                    listedTags,
                    $"Unexpected tag listing output: {output}");
                var localDiscrepancyLine = Array.FindIndex(outputLines, line => line.StartsWith("local: ", StringComparison.Ordinal));
                var remoteDiscrepancyLine = Array.FindIndex(outputLines, line => line.StartsWith("remote: ", StringComparison.Ordinal));
                var lastListedTagLine = Array.LastIndexOf(outputLines, expectedLatestTags[^1]);
                Assert.IsTrue(localDiscrepancyLine > lastListedTagLine);
                Assert.IsTrue(remoteDiscrepancyLine > localDiscrepancyLine);
                Assert.AreEqual($"local: {string.Join(", ", expectedLatestTags)}", outputLines[localDiscrepancyLine]);
                StringAssert.Contains(outputLines[remoteDiscrepancyLine], remoteOnlyTag);

                foreach (var tag in localOnlyTags)
                {
                    Assert.IsTrue(GitWrapper.PushTag(tag), $"Could not publish fixture tag {tag}.");
                }

                output.GetStringBuilder().Clear();
                Assert.IsTrue(Nbuild.Command.ListTags().IsSuccess());
                var matchingOutputLines = output.ToString()
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                Assert.IsFalse(matchingOutputLines.Any(line => line.StartsWith("local: ", StringComparison.Ordinal)));
                Assert.IsFalse(matchingOutputLines.Any(line => line.StartsWith("remote: ", StringComparison.Ordinal)));
                Assert.IsFalse(matchingOutputLines.Any(line => line.Contains("none", StringComparison.OrdinalIgnoreCase)));
            }
            finally
            {
                Console.SetOut(originalOutput);
                foreach (var tag in localOnlyTags.Append(remoteOnlyTag))
                {
                    GitWrapper.DeleteTag(tag, deleteRemote: true);
                }
            }
        }

        [TestMethod]
        public void CallGitCommand_ShouldExecuteArguments()
        {
            Assert.AreEqual("main", GitWrapper.Branch);
        }

        [TestMethod]
        public void DeleteBranch_ShouldProtectCurrentAndDefaultBranches()
        {
            Assert.IsFalse(GitWrapper.DeleteBranch(GitWrapper.Branch));
            Assert.IsFalse(GitWrapper.DeleteBranch("main"));
            Assert.IsFalse(GitWrapper.DeleteBranch("master"));
            Assert.IsFalse(GitWrapper.DeleteRemoteBranch(GitWrapper.Branch));
            Assert.IsFalse(GitWrapper.DeleteRemoteBranch("main"));
            Assert.IsFalse(GitWrapper.DeleteRemoteBranch("master"));
        }

        [TestMethod]
        public void SwitchBranchTest2() => Assert.IsTrue(GitWrapper.SwitchBranch("main"));

        [TestMethod]
        public void BranchOperationsTest()
        {
            var branch = "fixture-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            Assert.IsFalse(GitWrapper.BranchExists(null));
            Assert.IsTrue(GitWrapper.SwitchBranch(branch, create: true));
            Assert.IsTrue(GitWrapper.BranchExists(branch));
            Assert.IsTrue(GitWrapper.ListBranches().Contains(branch));
            Assert.IsTrue(GitWrapper.Push(branch));
            Assert.IsTrue(GitWrapper.BranchExistsOnRemote(branch));
            Assert.IsTrue(GitWrapper.ListRemoteBranches().Contains(branch));
            Assert.IsTrue(GitWrapper.SwitchBranch("main"));
            Assert.IsTrue(GitWrapper.DeleteRemoteBranch(branch));
            Assert.IsFalse(GitWrapper.BranchExistsOnRemote(branch));
            Assert.IsTrue(GitWrapper.DeleteBranch(branch));
        }

        [TestMethod]
        public void DeleteAllLocalBranchesTest()
        {
            var branch = "fixture-cleanup-" + Guid.NewGuid().ToString("N").Substring(0, 8);

            Assert.IsTrue(GitWrapper.SwitchBranch(branch, create: true));
            Assert.IsTrue(GitWrapper.SwitchBranch("main"));
            Assert.IsTrue(GitWrapper.DeleteAllLocalBranches().Contains(branch));
            Assert.IsFalse(GitWrapper.BranchExists(branch));
        }

        [TestMethod]
        public void GitRepositoryAndRemoteTests()
        {
            Assert.IsTrue(GitWrapper.IsGitRepository());
            Assert.IsTrue(GitWrapper.GetRemoteUrl()!.Contains(ProjectName));
        }

        [TestMethod]
        public void GitChangeOperationsTest()
        {
            var file = Path.Combine(Path.GetTempPath(), "NbuildTasksTests", ProjectName, "fixture-change.txt");
            File.WriteAllText(file, "staged with StageFile");
            Assert.IsTrue(GitWrapper.StageFile("fixture-change.txt").Code == 0);
            Assert.IsTrue(GitWrapper.GetStatusPorcelain().Contains("fixture-change.txt"));
            Assert.IsTrue(GitWrapper.Commit("Add fixture change"));

            File.WriteAllText(file, "staged with StageAllChanges");
            Assert.IsTrue(GitWrapper.StageAllChanges());
            Assert.IsTrue(GitWrapper.Commit("Update fixture change"));
            Assert.IsTrue(GitWrapper.Push());
            Assert.AreEqual(string.Empty, GitWrapper.GetStatusPorcelain());
        }

        [TestMethod]
        public void FetchAllAndPruneTest() => Assert.IsTrue(GitWrapper.FetchAllAndPrune());

        [TestMethod]
        public void PushTagTest()
        {
            var tag = GitWrapper.SetAutoTag(BuildType.STAGE.ToString());

            Assert.IsFalse(GitWrapper.RemoteTagExists(tag));
            Assert.IsTrue(GitWrapper.PushTag(tag));
            Assert.IsTrue(GitWrapper.ListRemoteTags().Contains(tag));
        }

        [TestMethod]
        public void PushTag_ReturnsFalseWhenOriginIsUnavailable()
        {
            var wrapper = GitWrapper;
            var tag = "6.6." + (DateTime.UtcNow.Ticks % 1000000);
            Assert.IsTrue(wrapper.SetTag(tag));

            var repository = wrapper.WorkingDirectory;
            RunGit(repository, "remote remove origin");
            try
            {
                Assert.IsFalse(wrapper.PushTag(tag));
            }
            finally
            {
                RunGit(repository, "remote add origin " + Quote(BareRepository));
            }
        }

        [TestMethod]
        public void DeleteTag_ReturnsFalseWhenOriginCannotBeChecked()
        {
            var wrapper = GitWrapper;
            var tag = "5.5." + (DateTime.UtcNow.Ticks % 1000000);
            Assert.IsTrue(wrapper.SetTag(tag));

            var repository = wrapper.WorkingDirectory;
            RunGit(repository, "remote remove origin");
            try
            {
                Assert.IsFalse(wrapper.DeleteTag(tag, deleteRemote: true));
                Assert.IsTrue(wrapper.LocalTagExists(tag));
            }
            finally
            {
                RunGit(repository, "remote add origin " + Quote(BareRepository));
            }
        }

        [TestMethod]
        public void ListRemoteTagsTest() => Assert.IsTrue(GitWrapper.ListRemoteTags().Contains("1.0.0"));

        [TestMethod]
        public void ListLocalTagsTest() => Assert.IsTrue(GitWrapper.ListLocalTags().Contains("1.0.0"));

        [TestMethod]
        public void SetWorkingDirTest()
        {
            var wrapper = new GitWrapper(project: null, verbose: true, testMode: true);

            Assert.IsTrue(wrapper.SetWorkingDir(RepositoryUrl));
            Assert.AreEqual(Path.Combine(wrapper.SourceDir, ProjectName), wrapper.WorkingDirectory);
        }

        [TestMethod]
        public void GetGitUserNameConfigurationTest() => Assert.IsFalse(string.IsNullOrWhiteSpace(GitWrapper.GetGitUserNameConfiguration()));

        [TestMethod]
        public void GetGitUserEmailConfigurationTest() => Assert.IsFalse(string.IsNullOrWhiteSpace(GitWrapper.GetGitUserEmailConfiguration()));

        [TestMethod]
        public void IsGitConfiguredTest() => Assert.IsTrue(GitWrapper.IsGitConfigured());

        private static string CreateTestDirectory()
        {
            var path = Path.Combine(FixtureRoot, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void AssertArgumentException(Action action)
        {
            try
            {
                action();
                Assert.Fail("Expected an ArgumentException.");
            }
            catch (ArgumentException)
            {
            }
        }

        private static void RunGit(string workingDirectory, string arguments)
        {
            using var process = Process.Start(new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            Assert.IsNotNull(process);
            process.WaitForExit();
            var error = process.StandardError.ReadToEnd();
            Assert.AreEqual(0, process.ExitCode, $"git {arguments} failed: {error}");
        }

        private static string Quote(string value) => $"\"{value.Replace("\"", "\\\"")}\"";

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                Directory.Delete(path, true);
            }
        }
    }
}
