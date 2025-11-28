using System;

namespace CrossCutting
{
    public interface ICacheService
    {
        bool Add(object key, object value);

        bool AddOrUpdate(object key, object value);

        object Get(object key);

        bool Remove(object key);

        void Clear();
    }

    public sealed class CacheService : ICacheService
    {
        private System.Collections.Concurrent.ConcurrentDictionary<object, object> _concurrentDictionary = new System.Collections.Concurrent.ConcurrentDictionary<object, object>();
        private static readonly CacheService _cacheService = new CacheService();

        // The Singleton Instance will be returned by the subsequent Static Method.
        // A thread-safe method that makes use of eager loading
        public static CacheService GetInstance()
        {
            return _cacheService;
        }

        // To prevent class instantiation from outside of this class, the constructor must be private.
        private CacheService()
        {
        }

        // The Singleton Instance can be used to access the following methods from outside of the class.

        // A Key-Value Pair can be added to the Cache using this function.
        public bool Add(object key, object value)
        {
            return _concurrentDictionary.TryAdd(key, value);
        }

        // A Key-Value Pair can be added or updated into the Cache using this function.
        // Add the key-value pair if the key is unavailable.
        // Update the key's value if it has already been added.
        public bool AddOrUpdate(object key, object value)
        {
            if (_concurrentDictionary.ContainsKey(key))
            {
                _concurrentDictionary.TryRemove(key, out _);
            }
            return _concurrentDictionary.TryAdd(key, value);
        }

        // If the specified key is in the cache, this function is used to return its value.
        // If not, return null.
        public object Get(object key)
        {
            if (_concurrentDictionary.ContainsKey(key))
            {
                return _concurrentDictionary[key];
            }
            return null;
        }

        // Using this technique, the specified key is deleted from the cache.
        // Return true if removed; return false otherwise.
        public bool Remove(object key)
        {
            return _concurrentDictionary.TryRemove(key, out _);
        }

        // Using this technique, the specified key is deleted from the cache.
        // Return true if removed; return false otherwise.
        public void Clear()
        {
            // Eliminates every key and value from the Cache, or the ConcurrentDictionary.
            _concurrentDictionary.Clear();
        }


    }
}
