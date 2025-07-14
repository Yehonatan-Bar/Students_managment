using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace StudentManagementAPI.Services
{
    /// <summary>
    /// Redis cache implementation of ICacheService.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDatabase _database;

        public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis)
        {
            _distributedCache = distributedCache;
            _database = redis.GetDatabase();
        }

        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value or null if not found.</returns>
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            if (value == null)
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        /// <summary>
        /// Sets a value in the cache with an expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The expiration time for the cached value.</param>
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        /// <summary>
        /// Removes all cache entries that match a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match cache keys.</param>
        public async Task RemoveByPatternAsync(string pattern)
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}