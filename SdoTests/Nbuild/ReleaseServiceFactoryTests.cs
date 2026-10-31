using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nbuild;
using Ntools;
using System.Net;

namespace NbuildTests
{
    [TestClass]
    [DoNotParallelize]
    public class ReleaseServiceFactoryTests
    {
        [TestMethod]
        public void DownloadApp_AllowsUnsignedDownload_WhenDownloadSucceeds()
        {
            var previousDownloader = Command.DownloadFileAsync;
            var previousTestMode = Environment.GetEnvironmentVariable("LOCAL_TEST", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("LOCAL_TEST", "true", EnvironmentVariableTarget.User);
            Command.DownloadFileAsync = (url, fileName) =>
            {
                var download = new ResultDownload(new Uri(url), fileName);
                download.Success();
                return Task.FromResult(download);
            };

            try
            {
                var result = Command.Download(CreateManifest("unsigned-app"), true);

                Assert.IsTrue(result.IsSuccess(), "An unsigned archive should not be treated as a failed download.");
            }
            finally
            {
                Command.DownloadFileAsync = previousDownloader;
                Environment.SetEnvironmentVariable("LOCAL_TEST", previousTestMode, EnvironmentVariableTarget.User);
            }
        }

        [TestMethod]
        public void DownloadApp_ReturnsExplanation_WhenUnsignedDownloadFails()
        {
            var previousDownloader = Command.DownloadFileAsync;
            var previousTestMode = Environment.GetEnvironmentVariable("LOCAL_TEST", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("LOCAL_TEST", "true", EnvironmentVariableTarget.User);
            Command.DownloadFileAsync = (url, fileName) =>
            {
                var download = new ResultDownload(new Uri(url), fileName);
                download.Fail("The downloaded file could not be validated.");
                return Task.FromResult(download);
            };

            try
            {
                var result = Command.Download(CreateManifest("failed-app"), true);

                Assert.IsFalse(result.IsSuccess());
                Assert.IsTrue(
                    result.Output.Any(message =>
                        message.Contains("not digitally signed", StringComparison.OrdinalIgnoreCase) &&
                        message.Contains("could not be validated", StringComparison.OrdinalIgnoreCase)),
                    "The failure should explain both the signature state and the downloader error.");
            }
            finally
            {
                Command.DownloadFileAsync = previousDownloader;
                Environment.SetEnvironmentVariable("LOCAL_TEST", previousTestMode, EnvironmentVariableTarget.User);
            }
        }

        [TestMethod]
        public void DownloadApp_UsesReleaseServiceFactory_OnAuthenticatedFallback_Success()
        {
            // Arrange - inject factory that returns a fake service which returns success
            Command.ReleaseServiceFactory = repo => new FakeReleaseService(HttpStatusCode.OK);

            var json = @"{
                ""Version"": ""1.2.0"",
                ""NbuildAppList"": [
                {
                ""Name"": ""private-app"",
                ""Version"": ""0.0.0"",
                ""AppFileName"": ""private.zip"",
                ""WebDownloadFile"": ""https://github.com/naz-hage/this-repo-does-not-exist/releases/download/0.0.0/0.0.0.zip"",
                ""DownloadedFile"": ""0.0.0.zip"",
                ""InstallCommand"": ""powershell.exe"",
                ""InstallArgs"": ""-Command Write-Output 'noop'"",
                ""InstallPath"": ""C:\\Temp\\nbuild2"",
                ""UninstallCommand"": ""powershell.exe"",
                ""UninstallArgs"": ""-Command Write-Output 'noop'""
                }
            ]
            }";

            // Ensure test mode and token are present so fallback is attempted
            System.Environment.SetEnvironmentVariable("LOCAL_TEST", "true", System.EnvironmentVariableTarget.User);
            var _prevToken = System.Environment.GetEnvironmentVariable("API_GITHUB_KEY", System.EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable("API_GITHUB_KEY", "fake-token", System.EnvironmentVariableTarget.Process);
            try
            {
                // Act
                var result = Command.Download(json, true);

                // Assert - since fake returned OK, Command.Download should succeed
                Assert.IsTrue(result.IsSuccess());
            }
            finally
            {
                // Restore previous token (may be null)
                System.Environment.SetEnvironmentVariable("API_GITHUB_KEY", _prevToken, System.EnvironmentVariableTarget.Process);
            }
        }

        [TestMethod]
        public void DownloadApp_UsesReleaseServiceFactory_OnAuthenticatedFallback_Failure()
        {
            // Arrange - inject factory that returns a fake service which returns NotFound
            Command.ReleaseServiceFactory = repo => new FakeReleaseService(HttpStatusCode.NotFound);

            var json = @"{
                ""Version"": ""1.2.0"",
                ""NbuildAppList"": [
                {
                ""Name"": ""private-app"",
                ""Version"": ""0.0.0"",
                ""AppFileName"": ""private.zip"",
                ""WebDownloadFile"": ""https://github.com/naz-hage/this-repo-does-not-exist/releases/download/0.0.0/0.0.0.zip"",
                ""DownloadedFile"": ""0.0.0.zip"",
                ""InstallCommand"": ""powershell.exe"",
                ""InstallArgs"": ""-Command Write-Output 'noop'"",
                ""InstallPath"": ""C:\\Temp\\nbuild2"",
                ""UninstallCommand"": ""powershell.exe"",
                ""UninstallArgs"": ""-Command Write-Output 'noop'""
                }
            ]
            }";

            // Ensure test mode and token are present so fallback is attempted
            System.Environment.SetEnvironmentVariable("LOCAL_TEST", "true", System.EnvironmentVariableTarget.User);
            var _prevToken2 = System.Environment.GetEnvironmentVariable("API_GITHUB_KEY", System.EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable("API_GITHUB_KEY", "fake-token", System.EnvironmentVariableTarget.Process);
            try
            {
                // Act
                var result = Command.Download(json, true);

                // Assert - since fake returned NotFound, Command.Download should return failure (but not throw)
                Assert.IsFalse(result.IsSuccess());
            }
            finally
            {
                // Restore previous token (may be null)
                System.Environment.SetEnvironmentVariable("API_GITHUB_KEY", _prevToken2, System.EnvironmentVariableTarget.Process);
            }
        }

        // Simple fake implementation of IReleaseService
        private class FakeReleaseService : Nbuild.IReleaseService
        {
            private readonly HttpStatusCode _status;
            public FakeReleaseService(HttpStatusCode status)
            {
                _status = status;
            }

            public Task<HttpResponseMessage> DownloadAssetByName(string tagName, string assetName, string downloadPath)
            {
                var resp = new HttpResponseMessage(_status);
                return Task.FromResult(resp);
            }

        }

        private static string CreateManifest(string name)
        {
            return $$"""
            {
                "Version": "1.2.0",
                "NbuildAppList": [
                {
                    "Name": "{{name}}",
                    "Version": "0.0.0",
                    "AppFileName": "{{name}}.zip",
                    "WebDownloadFile": "https://example.com/{{name}}.zip",
                    "DownloadedFile": "{{name}}.zip",
                    "InstallCommand": "powershell.exe",
                    "InstallArgs": "-Command Write-Output 'noop'",
                    "InstallPath": "C:\\Temp\\nbuild2",
                    "UninstallCommand": "powershell.exe",
                    "UninstallArgs": "-Command Write-Output 'noop'"
                }
                ]
            }
            """;
        }
    }
}
