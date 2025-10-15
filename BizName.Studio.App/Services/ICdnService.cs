namespace BizName.Studio.App.Services
{
    public interface ICdnService
    {
        Task<bool> PurgeCacheAsync(string fileUrl);
    }
}
