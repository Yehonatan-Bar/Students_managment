using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace StudentManagementAPI.Services
{
    /// <summary>
    /// Memory cache implementation of ICacheService.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _keyTracker;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
            _keyTracker = new ConcurrentDictionary<string, bool>();
        }

        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value or null if not found.</returns>
        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return Task.FromResult<T?>(typedValue);
                }
                
                if (value is string jsonString)
                {
                    try
                    {
                        var deserializedValue = JsonSerializer.Deserialize<T>(jsonString);
                        return Task.FromResult(deserializedValue);
                    }
                    catch (JsonException)
                    {
                        return Task.FromResult<T?>(default);
                    }
                }
            }
            return Task.FromResult<T?>(default);
        }

        /// <summary>
        /// Sets a value in the cache with an expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The expiration time for the cached value.</param>
        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                PostEvictionCallbacks = { new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (k, v, reason, state) =>
                    {
                        _keyTracker.TryRemove(k.ToString()!, out _);
                    }
                }}
            };

            _cache.Set(key, value, options);
            _keyTracker.TryAdd(key, true);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _keyTracker.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes all cache entries that match a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match cache keys.</param>
        public Task RemoveByPatternAsync(string pattern)
        {
            var regex = new Regex(pattern.Replace("*", ".*"), RegexOptions.IgnoreCase);
            var keysToRemove = _keyTracker.Keys.Where(key => regex.IsMatch(key)).ToList();
            
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _keyTracker.TryRemove(key, out _);
            }
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_keyTracker.ContainsKey(key));
        }
    }
}