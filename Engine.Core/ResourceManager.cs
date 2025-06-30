using System.Collections.Concurrent;

namespace Engine.Core
{
    /// <summary>
    /// Manages creation, lookup, and disposal of named resources
    /// </summary>
    public class ResourceManager : IDisposable
    {
        //Thread-safe container for any kind of resource
        private readonly ConcurrentDictionary<string, object> _resources = new ConcurrentDictionary<string, object>(StringComparer.Ordinal);
        private bool _disposed;

        /// <summary>
        /// Gets an existing resource by key, or uses the factory to create, cache, and return it.
        /// </summary>
        /// <typeparam name="T">Type of the resource.</typeparam>
        /// <param name="key">Unique identifier for the resource.</param>
        /// <param name="factory">Function to create the resource on cache-miss.</param>
        /// <returns>The cached or newly created resource.</returns>
        public T GetOrCreate<T>(string key, Func<T> factory) where T: class
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ResourceManager));
            }

            //If it already exists, return it.
            if (_resources.TryGetValue(key, out var existing) && existing is T typed)
                return typed;

            //Otherwise, create, cache, and return it.
            var created = factory()
                ?? throw new InvalidOperationException($"Factory for '{key}' returned null");

            if (!_resources.TryAdd(key, created))
                throw new InvalidOperationException($"Failed to add resource under key '{key}'.");

            return created;
        }

        /// <summary>
        /// Attempts to get a cached resource by key
        /// </summary>
        public bool TryGet<T>(string key, out T? resource) where T : class
        {
            resource = null;
            if (_disposed)
            {
                return false;
            }
            if (_resources.TryGetValue(key, out var existing) && existing is T typed)
            {
                resource = typed;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes and disposes a resource (if it emplements IDisposable)
        /// </summary>
        public bool Remove(string key)
        {
            if (_disposed)
                return false;

            if (_resources.TryRemove(key, out var removed))
            {
                if (removed is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disposes all cached resources and clears the cache.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (KeyValuePair<string, object> kv in _resources)
            {
                if (kv.Value is IDisposable d)
                {
                    try { d.Dispose(); }
                    catch { /*Swallow to ensure all get cleaned up */ }
                }
            }
            _resources.Clear();
        }
    }
}
