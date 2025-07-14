using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    /// <summary>
    /// Interface for caching service that provides methods to cache and retrieve data.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        /// <typeparam name="T">The type of the cached value.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value or null if not found.</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Sets a value in the cache with an expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiration">The expiration time for the cached value.</param>
        Task SetAsync<T>(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Removes a value from the cache.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Removes all cache entries that match a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match cache keys.</param>
        Task RemoveByPatternAsync(string pattern);

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        Task<bool> ExistsAsync(string key);
    }
}