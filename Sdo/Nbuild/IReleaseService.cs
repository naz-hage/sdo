namespace Nbuild
{
    public interface IReleaseService
    {
        Task<HttpResponseMessage> DownloadAssetByName(string tagName, string assetName, string downloadPath);
        Task<HttpResponseMessage> DeleteReleaseByTagAsync(string tagName);
    }
}
