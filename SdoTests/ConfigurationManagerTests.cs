// Copyright (c) 2020-2026 naz-hage. All rights reserved.
// Licensed under the MIT License.

using Sdo.Utilities;

namespace SdoTests
{
    /// <summary>
    /// Unit tests for ConfigurationManager.
    /// Tests YAML parsing, environment variable interpolation, and configuration management.
    /// </summary>
    [TestClass]
    public class ConfigurationManagerTests
    {
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), "ConfigurationManagerTests");

        public ConfigurationManagerTests()
        {
            Directory.CreateDirectory(_testDirectory);
        }

        private string CreateConfigFile(string content)
        {
            var filePath = Path.Combine(_testDirectory, $".sdo_{Guid.NewGuid()}.yaml");
            File.WriteAllText(filePath, content);
            return filePath;
        }

        private void CleanupFile(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        [TestMethod]
        public void Load_WithValidYamlFile_LoadsConfiguration()
        {
            // Arrange
            var content = @"# SDO Configuration
platform: github
organization: my-org
project: my-project
verbose: true
batch_size: 10";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                var result = config.Load(filePath);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual("github", config.GetValue("platform"));
                Assert.AreEqual("my-org", config.GetValue("organization"));
                Assert.AreEqual("10", config.GetValue("batch_size"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithSections_ParsesSectionedConfiguration()
        {
            // Arrange - Using YAML nested structure instead of INI [section] format
            var content = @"github:
  token: ghp_xxxxxxxxxxxx
  owner: my-org
azure_devops:
  pat: ${AZURE_DEVOPS_PAT}
  organization: my-org";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                var result = config.Load(filePath);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual("ghp_xxxxxxxxxxxx", config.GetValue("github:token"));
                Assert.AreEqual("my-org", config.GetValue("azure_devops:organization"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetValue_WithEnvironmentVariable_InterpolatesCorrectly()
        {
            // Arrange
            Environment.SetEnvironmentVariable("TEST_VAR", "test_value");
            var content = @"config_value: ${TEST_VAR}
other_value: static";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var value = config.GetValue("config_value");

                // Assert
                Assert.AreEqual("test_value", value);
            }
            finally
            {
                CleanupFile(filePath);
                Environment.SetEnvironmentVariable("TEST_VAR", null);
            }
        }

        [TestMethod]
        public void GetInt_WithValidInteger_ReturnsInt()
        {
            // Arrange
            var content = "batch_size: 25";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var value = config.GetInt("batch_size");

                // Assert
                Assert.AreEqual(25, value);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetInt_WithInvalidValue_ReturnsDefault()
        {
            // Arrange
            var content = "batch_size: not_a_number";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var value = config.GetInt("batch_size", 10);

                // Assert
                Assert.AreEqual(10, value);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetBool_WithTrueVariations_ReturnsTrue()
        {
            // Arrange
            var content = @"option_a: true
option_b: yes
option_c: 1";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.IsTrue(config.GetBool("option_a"));
                Assert.IsTrue(config.GetBool("option_b"));
                Assert.IsTrue(config.GetBool("option_c"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetBool_WithFalseVariations_ReturnsFalse()
        {
            // Arrange
            var content = @"option_a: false
option_b: no
option_c: 0";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.IsFalse(config.GetBool("option_a"));
                Assert.IsFalse(config.GetBool("option_b"));
                Assert.IsFalse(config.GetBool("option_c"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void SetValue_OverridesConfigurationValue()
        {
            // Arrange
            var content = "platform: github";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                config.SetValue("platform", "azure_devops");
                var value = config.GetValue("platform");

                // Assert
                Assert.AreEqual("azure_devops", value);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetKeys_ReturnsAllConfigurationKeys()
        {
            // Arrange
            var content = @"platform: github
organization: my-org
project: my-project";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var keys = config.GetKeys();

                // Assert
                Assert.IsTrue(keys.Contains("platform"));
                Assert.IsTrue(keys.Contains("organization"));
                Assert.IsTrue(keys.Contains("project"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithComments_IgnoresCommentLines()
        {
            // Arrange
            var content = @"# This is a comment
platform: github  # inline comment ignored
# organization: commented_out
project: my-project";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.AreEqual("github", config.GetValue("platform"));
                Assert.IsNull(config.GetValue("organization"));
                Assert.AreEqual("my-project", config.GetValue("project"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithQuotedValues_StripsQuotes()
        {
            // Arrange
            var content = @"value_single: 'single quoted'
value_double: ""double quoted""
value_unquoted: unquoted";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.AreEqual("single quoted", config.GetValue("value_single"));
                Assert.AreEqual("double quoted", config.GetValue("value_double"));
                Assert.AreEqual("unquoted", config.GetValue("value_unquoted"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithNonexistentFile_ReturnsFalse()
        {
            // Arrange
            var config = new ConfigurationManager();

            // Act
            var result = config.Load("/nonexistent/path/.sdo.yaml");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotEmpty(config.GetErrors());
        }

        [TestMethod]
        public void GetValue_WithMissingKey_ReturnsDefault()
        {
            // Arrange
            var content = "platform: github";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var value = config.GetValue("missing_key", "default_value");

                // Assert
                Assert.AreEqual("default_value", value);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetAllValues_WithSensitiveKeys_RedactsValues()
        {
            // Arrange
            var content = @"platform: github
github_token: ghp_xxxxxxxxxxxx
azure_pat: dev.azure.com_token";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var allValues = config.GetAllValues(includeSensitive: false);

                // Assert
                Assert.AreEqual("github", allValues["platform"]);
                Assert.AreEqual("***REDACTED***", allValues["github_token"]);
                Assert.AreEqual("***REDACTED***", allValues["azure_pat"]);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void IsValid_WithLoadedConfiguration_ReturnsTrue()
        {
            // Arrange
            var content = "platform: github";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.IsTrue(config.IsValid());
                Assert.IsEmpty(config.GetErrors());
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void ExportAsJson_ExportsConfigurationAsJson()
        {
            // Arrange
            var content = @"platform: github
organization: my-org";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var json = config.ExportAsJson();

                // Assert
                StringAssert.Contains(json, "platform");
                StringAssert.Contains(json, "github");
                StringAssert.Contains(json, "organization");
                StringAssert.Contains(json, "my-org");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithoutFile_UsesDefaults()
        {
            // Arrange
            var config = new ConfigurationManager();

            // Act
            var result = config.Load();

            // Assert
            Assert.IsTrue(result); // Should succeed with no errors even if file not found
            Assert.IsTrue(config.IsValid());
        }

        #region Nested YAML and Config Defaults Tests

        [TestMethod]
        public void Load_WithNestedYaml_FlattensTooDotNotation()
        {
            // Arrange
            var content = @"commands:
  wi:
    list:
      project: Proto
      area_path: Proto\Warriors
      state: To Do,In Progress
      top: 50";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.AreEqual("Proto", config.GetValue("commands:wi:list:project"));
                Assert.AreEqual("Proto\\Warriors", config.GetValue("commands:wi:list:area_path"));
                Assert.AreEqual("To Do,In Progress", config.GetValue("commands:wi:list:state"));
                Assert.AreEqual("50", config.GetValue("commands:wi:list:top"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void LoadedConfigPath_AfterLoad_StoresAbsolutePath()
        {
            // Arrange
            var content = "project: TestProject";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.IsNotNull(config.LoadedConfigPath);
                Assert.AreEqual(Path.GetFullPath(filePath), config.LoadedConfigPath);
                Assert.IsTrue(Path.IsPathRooted(config.LoadedConfigPath));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void LoadedConfigPath_WithoutLoad_ReturnsNull()
        {
            // Arrange
            var config = new ConfigurationManager();

            // Act - don't call Load()

            // Assert
            Assert.IsNull(config.LoadedConfigPath);
        }

        [TestMethod]
        public void Load_WithCustomPath_StoresLoadedPath()
        {
            // Arrange
            var content = "setting: value";
            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.IsNotNull(config.LoadedConfigPath);
                StringAssert.Contains(config.LoadedConfigPath, "ConfigurationManagerTests");
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetValue_WithDotNotation_RetrievesNestedValues()
        {
            // Arrange
            var content = @"azure_devops:
  organization: myorg
  project: myproject
commands:
  wi:
    list:
      state: To Do,In Progress";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.AreEqual("myorg", config.GetValue("azure_devops.organization"));
                Assert.AreEqual("myproject", config.GetValue("azure_devops.project"));
                Assert.AreEqual("To Do,In Progress", config.GetValue("commands.wi.list.state"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetValue_WithColonNotation_RetrievesNestedValues()
        {
            // Arrange
            var content = @"azure_devops:
  organization: myorg
commands:
  wi:
    list:
      state: To Do,In Progress";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                Assert.AreEqual("myorg", config.GetValue("azure_devops:organization"));
                Assert.AreEqual("To Do,In Progress", config.GetValue("commands:wi:list:state"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void GetInt_WithNestedYamlValue_ParsesInteger()
        {
            // Arrange
            var content = @"commands:
  wi:
    list:
      top: 50";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);
                var top = config.GetInt("commands:wi:list:top");

                // Assert
                Assert.AreEqual(50, top);
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithComplexNestedStructure_HandlesMultipleLevels()
        {
            // Arrange
            var content = @"project:
  name: TestProject
  description: Test Description
azure_devops:
  organization: testorg
  project: TestProject
  area_path: TestProject\Team
  default_iteration: TestProject\Sprint01
commands:
  wi:
    list:
      project: TestProject
      area_path: TestProject\Team
      state: To Do,In Progress
      type: PBI,Task
      top: 100";

            var filePath = CreateConfigFile(content);
            var config = new ConfigurationManager();

            try
            {
                // Act
                config.Load(filePath);

                // Assert
                // Verify all nested values are accessible
                Assert.AreEqual("TestProject", config.GetValue("project:name"));
                Assert.AreEqual("testorg", config.GetValue("azure_devops:organization"));
                Assert.AreEqual("TestProject\\Team", config.GetValue("azure_devops:area_path"));
                Assert.AreEqual("To Do,In Progress", config.GetValue("commands:wi:list:state"));
                Assert.AreEqual("100", config.GetValue("commands:wi:list:top"));
            }
            finally
            {
                CleanupFile(filePath);
            }
        }

        [TestMethod]
        public void Load_WithTempFolderStructure_FindsConfigInTempFolder()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), $"ConfigTempTest_{Guid.NewGuid()}");
            var tempFolderDir = Path.Combine(tempDir, ".temp");
            Directory.CreateDirectory(tempFolderDir);

            var configPath = Path.Combine(tempFolderDir, "sdo-config.yaml");
            var content = @"commands:
  wi:
    list:
      project: TestProject
      state: To Do,In Progress";
            File.WriteAllText(configPath, content);

            var config = new ConfigurationManager();
            var originalCwd = Directory.GetCurrentDirectory();

            try
            {
                // Act
                Directory.SetCurrentDirectory(tempDir);
                var result = config.Load(); // Should find config in .temp subfolder

                // Assert
                Assert.IsTrue(result);
                Assert.IsNotNull(config.LoadedConfigPath);
                StringAssert.Contains(config.LoadedConfigPath, ".temp");
                Assert.AreEqual("TestProject", config.GetValue("commands:wi:list:project"));
                Assert.AreEqual("To Do,In Progress", config.GetValue("commands:wi:list:state"));
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCwd);
                // Force garbage collection to release file handles
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect(); // Second GC to ensure cleanup
                // Retry logic for file system operations on Windows
                int maxAttempts = 5;
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    try
                    {
                        if (Directory.Exists(tempDir))
                        {
                            Directory.Delete(tempDir, recursive: true);
                        }
                        break;
                    }
                    catch (IOException) when (attempt < maxAttempts - 1)
                    {
                        System.Threading.Thread.Sleep(200 + (attempt * 100)); // Exponential backoff
                    }
                    catch (UnauthorizedAccessException) when (attempt < maxAttempts - 1)
                    {
                        System.Threading.Thread.Sleep(200 + (attempt * 100)); // Exponential backoff
                    }
                }
            }
        }

        [TestMethod]
        public void Load_SearchOrder_CurrentDirectoryFirst()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), $"ConfigSearchTest_{Guid.NewGuid()}");
            var tempFolderDir = Path.Combine(tempDir, ".temp");
            Directory.CreateDirectory(tempFolderDir);

            // Create config in both current dir and .temp folder
            var currentDirConfig = Path.Combine(tempDir, "sdo-config.yaml");
            var tempFolderConfig = Path.Combine(tempFolderDir, "sdo-config.yaml");

            File.WriteAllText(currentDirConfig, "location: current_directory");
            File.WriteAllText(tempFolderConfig, "location: temp_folder");

            var config = new ConfigurationManager();
            var originalCwd = Directory.GetCurrentDirectory();

            try
            {
                // Act
                Directory.SetCurrentDirectory(tempDir);
                config.Load(); // Should prefer current directory

                // Assert
                Assert.AreEqual("current_directory", config.GetValue("location"));
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCwd);
                Directory.Delete(tempDir, recursive: true);
            }
        }

        #endregion
    }
}
