// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.

using Sdo.Utilities;

namespace SdoTests
{
    /// <summary>
    /// Unit tests for MarkdownParser class.
    /// Tests YAML frontmatter, titles, metadata, code blocks, and acceptance criteria parsing.
    /// </summary>
    [TestClass]
    public class MarkdownParserTests
    {
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "MarkdownParserTests");

        public MarkdownParserTests()
        {
            Directory.CreateDirectory(_testDirectory);
        }

        private string CreateTestFile(string content)
        {
            var filePath = Path.Combine(_testDirectory, $"test_{Guid.NewGuid()}.md");
            File.WriteAllText(filePath, content);
            return filePath;
        }

        private void CleanupFile(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        [TestMethod]
        public void ParseFile_WithSimpleTitleAndDescription_ParsesCorrectly()
        {
            // Arrange
            var content = @"# Work Item Title

This is the description.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual("Work Item Title", result.Title);
                Assert.AreEqual("This is the description.", result.Description);
                Assert.IsEmpty(result.Errors);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithYamlFrontmatter_ParsesMetadata()
        {
            // Arrange
            var content = @"---
work_item_type: PBI
assignee: user@example.com
labels: ""feature, backlog""
---
# PBI Title

Description here.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual("PBI Title", result.Title);
                Assert.AreEqual("PBI", result.Metadata["work_item_type"]);
                Assert.AreEqual("user@example.com", result.Metadata["assignee"]);
                Assert.AreEqual("feature, backlog", result.Metadata["labels"]);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithMetadataHeaders_ParsesMetadata()
        {
            // Arrange
            var content = @"# Work Item Title

## Work Item Type: PBI
## Priority: High
## Assignee: user@example.com

Description here.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual("Work Item Title", result.Title);
                Assert.AreEqual("PBI", result.Metadata["work_item_type"]);
                Assert.AreEqual("High", result.Metadata["priority"]);
                Assert.AreEqual("user@example.com", result.Metadata["assignee"]);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithAcceptanceCriteria_ParsesCorrectly()
        {
            // Arrange
            var content = @"# Feature Title

This feature does X.

## Acceptance Criteria
- [ ] Criterion 1
- [x] Criterion 2 (completed)
- Criterion 3 (no checkbox)";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual(3, result.AcceptanceCriteria.Count);
                Assert.AreEqual("Criterion 1", result.AcceptanceCriteria[0]);
                Assert.AreEqual("Criterion 2 (completed)", result.AcceptanceCriteria[1]);
                Assert.AreEqual("Criterion 3 (no checkbox)", result.AcceptanceCriteria[2]);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithCodeBlocks_ExtractsCodeBlocks()
        {
            // Arrange
            var content = @"# Technical Task

## Steps
```bash
npm install
npm run build
```

## Expected Output
```json
{
  ""status"": ""success""
}
```";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual(2, result.CodeBlocks.Count);
                StringAssert.Contains(result.CodeBlocks[0], "npm install");
                StringAssert.Contains(result.CodeBlocks[1], "status");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithMalformedHeader_WarnsButParses()
        {
            // Arrange
            var content = @"#Malformed Title

Description here.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotEmpty(result.Warnings);
                StringAssert.Contains(result.Warnings[0].Message, "Malformed header");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithMissingTitle_ReturnsError()
        {
            // Arrange
            var content = @"Just some description without a title.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.IsNotEmpty(result.Errors);
                StringAssert.Contains(result.Errors[0].Message, "Title is required");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithNonexistentFile_ReturnsError()
        {
            // Act
            var result = MarkdownParser.ParseFile("/nonexistent/path/file.md");

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
            StringAssert.Contains(result.Errors[0].Message, "File not found");
        }

        [TestMethod]
        public void ParseFile_WithEmptyFile_ReturnsError()
        {
            // Arrange
            var filePath = CreateTestFile("");

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsFalse(result.Success);
                Assert.IsNotEmpty(result.Errors);
                StringAssert.Contains(result.Errors[0].Message.ToLower(), "empty");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithComplexDocument_ParsesAllSections()
        {
            // Arrange
            var content = @"---
type: Epic
priority: High
---
# Epic: Authentication System

Implement comprehensive authentication.

## Labels: Security, Auth

### Overview
Complete authentication overhaul with OAuth2 support.

## Acceptance Criteria
- [ ] OAuth2 integration
- [ ] Token refresh mechanism
- [ ] Rate limiting

## Code Examples
```csharp
public void ConfigureAuth(IServiceCollection services)
{
    services.AddAuthentication();
}
```";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual("Epic: Authentication System", result.Title);
                Assert.AreEqual("Epic", result.Metadata["type"]);
                Assert.AreEqual("High", result.Metadata["priority"]);
                Assert.AreEqual(3, result.AcceptanceCriteria.Count);
                Assert.IsNotEmpty(result.CodeBlocks);
                StringAssert.Contains(result.CodeBlocks[0], "ConfigureAuth");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithUnclosedCodeBlock_WarnsButIncludesBlock()
        {
            // Arrange
            var content = @"# Task

## Implementation
```csharp
public void DoSomething()
{
    // Implementation here
}";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.IsNotEmpty(result.Warnings);
                StringAssert.Contains(result.Warnings[0].Message, "Unclosed code block");
                Assert.IsNotEmpty(result.CodeBlocks);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithMultipleMetadataFormats_MergesAll()
        {
            // Arrange
            var content = @"---
source: yaml
---
# Title

## Target: azdo
## Project: MyProject

Content here.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                Assert.AreEqual("yaml", result.Metadata["source"]);
                Assert.AreEqual("azdo", result.Metadata["target"]);
                Assert.AreEqual("MyProject", result.Metadata["project"]);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ParseFile_WithVerboseMode_IncludesDetailedErrors()
        {
            // Arrange
            var filePath = "/invalid/path/test.md";

            // Act
            var result = MarkdownParser.ParseFile(filePath, verbose: true);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
            // Verbose mode may include stack traces
        }

        [TestMethod]
        public void ParseFile_WithSpecialCharactersInMetadata_ParsesCorrectly()
        {
            // Arrange
            var content = @"---
description: ""Complex value with: colons and quotes""
---
# Test Title

Content.";
            var filePath = CreateTestFile(content);

            try
            {
                // Act
                var result = MarkdownParser.ParseFile(filePath);

                // Assert
                Assert.IsTrue(result.Success);
                StringAssert.Contains(result.Metadata["description"], "Complex value with: colons and quotes");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }
    }
}
