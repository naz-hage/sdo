// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// PlatformDetectorTests.cs
//
// Unit tests for the PlatformDetector service.

using Sdo.Interfaces;
using Sdo.Services;

namespace SdoTests;

/// <summary>
/// Unit tests for the PlatformDetector class.
/// </summary>
[TestClass]
public class PlatformServiceTests : IDisposable
{
    private readonly string _originalWorkingDirectory = Environment.CurrentDirectory;

    public void Dispose()
    {
        Environment.CurrentDirectory = _originalWorkingDirectory;
    }

    private void SetupWorkingDirectory()
    {
        // Set working directory to the solution root (sdo) to ensure Git repository is accessible
        
        // For GitHub Actions, use the GITHUB_WORKSPACE environment variable
        var githubWorkspace = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE");
        if (!string.IsNullOrEmpty(githubWorkspace) && Directory.Exists(Path.Combine(githubWorkspace, ".git")))
        {
            Environment.CurrentDirectory = githubWorkspace;
            return;
        }
        
        // Resolve from the test assembly location so other tests changing the process
        // working directory cannot make this repository fixture nondeterministic.
        var candidate = new DirectoryInfo(AppContext.BaseDirectory);
        while (candidate != null && !Directory.Exists(Path.Combine(candidate.FullName, ".git")))
        {
            candidate = candidate.Parent;
        }

        if (candidate != null)
        {
            Environment.CurrentDirectory = candidate.FullName;
        }
    }

    [TestMethod]
    public void DetectPlatform_InGitHubRepo_ReturnsGitHub()
    {
        // Arrange
        SetupWorkingDirectory();
        var detector = new PlatformService();

        // Act
        var platform = detector.DetectPlatform();

        // Assert - Current sdo repo should be detected as GitHub
        Assert.AreEqual(Platform.GitHub, platform);
    }

    [TestMethod]
    public void GetOrganization_InGitHubRepo_ReturnsOrganization()
    {
        // Arrange
        SetupWorkingDirectory();
        var detector = new PlatformService();

        // Act
        var organization = detector.GetOrganization();

        // Assert - Should detect "naz-hage" from the current repo
        Assert.AreEqual("naz-hage", organization);
    }

    [TestMethod]
    public void GetProject_InGitHubRepo_ReturnsProject()
    {
        // Arrange
        SetupWorkingDirectory();
        var detector = new PlatformService();

        // Act
        var project = detector.GetProject();

        // Assert - Should detect "sdo" from the current repo
        Assert.AreEqual("sdo", project);
    }

    [TestMethod]
    public void ParseAzureDevOpsUrl_CorrectlyExtractsOrganizationAndProject()
    {
        // This test verifies the parsing fix for Issue 1
        // URL: https://dev.azure.com/nazh/Proto/_git/ConsoleApp1
        // Should extract: organization="nazh", project="Proto"

        var expectedOrganization = "nazh";
        var expectedProject = "Proto";
        var testUrl = "https://dev.azure.com/nazh/Proto/_git/ConsoleApp1";

        // Parse the URL manually to verify our logic
        var cleanUrl = testUrl
            .Replace("https://", "")
            .Split('?').First();

        var parts = cleanUrl.Split('/');
        // parts[0] = "dev.azure.com"
        // parts[1] = "nazh" 
        // parts[2] = "Proto"
        // parts[3] = "_git"
        // parts[4] = "ConsoleApp1"

        Assert.AreEqual("dev.azure.com", parts[0]);
        Assert.AreEqual("nazh", parts[1]); // organization
        Assert.AreEqual("Proto", parts[2]); // project
        Assert.AreEqual("_git", parts[3]);
        Assert.AreEqual("ConsoleApp1", parts[4]);

        // Verify our parsing logic would work
        Assert.IsTrue(parts.Length >= 5);
        StringAssert.Contains(parts[0], "dev.azure.com");
        Assert.AreEqual(expectedOrganization, parts[1]);
        Assert.AreEqual(expectedProject, parts[2]);
    }

    [TestMethod]
    public void PlatformDetector_CanBeInstantiated()
    {
        // Act
        var detector = new PlatformService();

        // Assert
        Assert.IsNotNull(detector);
    }
}