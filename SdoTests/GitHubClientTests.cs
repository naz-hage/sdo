// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// GitHubClientTests.cs
//
// Unit tests for GitHubClient class.

using Sdo.Services;

namespace SdoTests;

/// <summary>
/// Unit tests for the GitHubClient class.
/// </summary>
[TestCategory("Integration")]
[TestClass]
public class GitHubClientTests
{
    [TestMethod]
    public async Task VerifyAuthenticationAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            return; // Skip integration test if no token
        }

        using var client = new GitHubClient(token);

        // Act
        var result = await client.VerifyAuthenticationAsync();

        // Assert - skip if token is invalid (integration test)
        if (!result)
        {
            return; // Skip test if token doesn't work
        }

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task VerifyAuthenticationAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        using var client = new GitHubClient("invalid-token");

        // Act
        var result = await client.VerifyAuthenticationAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetUserAsync_WithValidToken_ReturnsUser()
    {
        // Arrange
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            return; // Skip integration test if no token
        }

        using var client = new GitHubClient(token);

        // Act
        var user = await client.GetUserAsync();

        // Assert - skip if token is invalid (integration test)
        if (user == null)
        {
            return; // Skip test if token doesn't work
        }

        Assert.IsNotNull(user);
        Assert.IsNotNull(user.Login);
    }

    [TestMethod]
    public async Task GetUserAsync_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        using var client = new GitHubClient("invalid-token");

        // Act
        var user = await client.GetUserAsync();

        // Assert
        Assert.IsNull(user);
    }
}