using Microsoft.Build.Framework;
using Ntools;
using System.Threading.Tasks;

namespace NbuildTasks
{
    public class WebDownload : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string WebUri { get; set; } = string.Empty;

        [Required]
        public string FileName { get; set; } = string.Empty;

        public override bool Execute()
        {
            var result = Task.Run(async () => await Nfile.DownloadAsync(WebUri, FileName)).Result;

            if (result.IsSuccess())
            {
                Log.LogMessage($"Downloaded {WebUri} to {FileName}");
            }
            else
            {

                Log.LogError($"Failed to download {WebUri} to {FileName}: {result.GetFirstOutput()}");
            }

            return !Log.HasLoggedErrors;
        }
    }
}
