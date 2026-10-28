// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.
//
// ResultTests.cs
//
// Unit tests for the Result pattern classes.

using Sdo.Services;

namespace SdoTests;

/// <summary>
/// Unit tests for the Result pattern classes.
/// </summary>
[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Result_Success_CreatesSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.IsNull(result.Error);
        Assert.AreEqual(0, result.ExitCode);
    }

    [TestMethod]
    public void Result_Failure_CreatesFailedResult()
    {
        // Arrange
        const string errorMessage = "Test error";
        const int exitCode = 42;

        // Act
        var result = Result.Failure(errorMessage, exitCode);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(errorMessage, result.Error);
        Assert.AreEqual(exitCode, result.ExitCode);
    }

    [TestMethod]
    public void Result_Failure_DefaultExitCode_IsOne()
    {
        // Act
        var result = Result.Failure("Test error");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Test error", result.Error);
        Assert.AreEqual(1, result.ExitCode);
    }

    [TestMethod]
    public void ResultT_Success_CreatesSuccessfulResultWithValue()
    {
        // Arrange
        const string testValue = "test value";

        // Act
        var result = Result<string>.Success(testValue);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.IsNull(result.Error);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual(testValue, result.Value);
    }

    [TestMethod]
    public void ResultT_Failure_CreatesFailedResult()
    {
        // Arrange
        const string errorMessage = "Test error";
        const int exitCode = 42;

        // Act
        var result = Result<string>.Failure(errorMessage, exitCode);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(errorMessage, result.Error);
        Assert.AreEqual(exitCode, result.ExitCode);
        Assert.IsNull(result.Value);
    }

    [TestMethod]
    public void ResultT_Failure_DefaultExitCode_IsOne()
    {
        // Act
        var result = Result<int>.Failure("Test error");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Test error", result.Error);
        Assert.AreEqual(1, result.ExitCode);
        Assert.AreEqual(0, result.Value); // default value for int
    }
}