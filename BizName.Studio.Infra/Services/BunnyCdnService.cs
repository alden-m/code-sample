using  BizName.Studio.App.Services;
using System.Web;

namespace BizName.Studio.Infra.Services
{
    internal class BunnyCdnService(/* TODO: Move this to options */string? apiKey = null) : ICdnService
    {
        public async Task<bool> PurgeCacheAsync(string fileUrl)
        {
            // Argument validation
            if (string.IsNullOrWhiteSpace(fileUrl)) throw new ArgumentException("File URL cannot be null or empty", nameof(fileUrl));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("API key is not configured");
            
            // Construct the purge request URL
            string purgeUrl = $"https://api.bunny.net/purge?url={HttpUtility.UrlEncode(fileUrl)}&async=false";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("AccessKey", apiKey);

            var response = await httpClient.PostAsync(purgeUrl, null);
            return response.IsSuccessStatusCode;
        }
    }
}
