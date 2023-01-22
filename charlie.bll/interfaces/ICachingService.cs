using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll.interfaces
{
    public interface ICachingService
    {
        Task<string> GetStringAsync(string key, CancellationToken token = default);
        Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token = default);
        Task SetStringAsync(string key, string value, CancellationToken token = default);
        Task<T> GetAsync<T>(string key, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default);
        Task SetAsync<T>(string key, T value, CancellationToken token = default);
        Task RemoveAsync(string key, CancellationToken token = default);
        string GetString(string key);
        void SetString(string key, string value, DistributedCacheEntryOptions options);
        void SetString(string key, string value);
        T Get<T>(string key);
        void Set<T>(string key, T value, DistributedCacheEntryOptions options);
        void Set<T>(string key, T value);
        void Remove(string key);
    }
}
