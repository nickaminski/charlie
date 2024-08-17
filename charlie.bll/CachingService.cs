using charlie.bll.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace charlie.bll
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions _defaultOptions = new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(60) };

        public CachingService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            var value = await GetStringAsync(key, token);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public Task<string> GetStringAsync(string key, CancellationToken token = default)
        {
            return _cache.GetStringAsync(key, token);
        }

        public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var data = JsonConvert.SerializeObject(value);
            return SetStringAsync(key, data, options, token);
        }

        public Task SetAsync<T>(string key, T value, CancellationToken token = default)
        {
            return SetAsync<T>(key, value, _defaultOptions, token);
        }

        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            return SetStringAsync(key, value, options, token);
        }

        public Task SetStringAsync(string key, string value, CancellationToken token = default)
        {
            return _cache.SetStringAsync(key, value, _defaultOptions, token);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return _cache.RemoveAsync(key, token);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public string GetString(string key)
        {
            return _cache.GetString(key);
        }

        public void SetString(string key, string value, DistributedCacheEntryOptions options)
        {
            _cache.SetString(key, value, options);
        }

        public void SetString(string key, string value)
        {
            SetString(key, value, _defaultOptions);
        }

        public T Get<T>(string key)
        {
            var value = GetString(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public void Set<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var data = JsonConvert.SerializeObject(value);
            SetString(key, data, options);
        }

        public void Set<T>(string key, T value)
        {
            Set(key, value, _defaultOptions);
        }
    }
}
