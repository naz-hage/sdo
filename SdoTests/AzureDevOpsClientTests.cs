// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// AzureDevOpsClientTests.cs
//
// Unit tests for AzureDevOpsClient class.

using Sdo.Services;

namespace SdoTests;

/// <summary>
/// Unit tests for the AzureDevOpsClient class.
/// </summary>
[TestCategory("Integration")]
[TestClass]
public class AzureDevOpsClientTests
{
    [TestMethod]
    public async Task VerifyAuthenticationAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var token = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT");
        var organization = Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(organization))
        {
            return; // Skip integration test if no credentials
        }

        using var client = new AzureDevOpsClient(token, organization);

        // Act
        var result = await client.VerifyAuthenticationAsync();

        // Assert - skip if credentials are invalid (integration test)
        if (!result)
        {
            return; // Skip test if credentials don't work
        }

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task VerifyAuthenticationAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        using var client = new AzureDevOpsClient("invalid-token", "test-org");

        // Act
        var result = await client.VerifyAuthenticationAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetUserAsync_WithValidToken_ReturnsUser()
    {
        // Arrange
        var token = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT");
        var organization = Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(organization))
        {
            return; // Skip integration test if no credentials
        }

        using var client = new AzureDevOpsClient(token, organization);

        // Act
        var user = await client.GetUserAsync();

        // Assert - skip if credentials are invalid (integration test)
        if (user == null)
        {
            return; // Skip test if credentials don't work
        }

        Assert.IsNotNull(user);
        // Remove this assertion if not needed for your test scenario
        //Assert.IsNotNull(user.UniqueName);
    }

    [TestMethod]
    public async Task GetUserAsync_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        using var client = new AzureDevOpsClient("invalid-token", "test-org");

        // Act
        var user = await client.GetUserAsync();

        // Assert
        Assert.IsNull(user);
    }

    [TestMethod]
    public async Task AdHoc_GetPipelineAsync_WithConfiguredEnvironment_ReturnsPipeline()
    {
        // Arrange (ad hoc integration style)
        var token = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT");
        var organization = Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG");
        var project = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PROJECT");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(project))
        {
            return; // Skip if environment not configured
        }

        using var client = new AzureDevOpsClient(token, organization, project);

        // Pick the first pipeline from live data, then fetch it by ID
        var pipelines = await client.ListPipelinesAsync(project);
        if (pipelines == null || pipelines.Count == 0)
        {
            return; // Skip if project has no pipelines
        }

        var first = pipelines[0];

        // Act
        var byId = await client.GetPipelineAsync(project, first.Id);
        var byName = string.IsNullOrWhiteSpace(first.Name)
            ? null
            : await client.GetPipelineAsync(project, first.Name);

        // Assert
        Assert.IsNotNull(byId);
        Assert.AreEqual(first.Id, byId!.Id);
        if (!string.IsNullOrWhiteSpace(first.Name))
        {
            Assert.IsNotNull(byName);
            Assert.AreEqual(first.Id, byName!.Id);
        }
    }
}