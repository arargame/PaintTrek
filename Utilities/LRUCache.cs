using System;
using System.Collections.Generic;

namespace PaintTrek.Android.Systems
{
    public class LRUCache<K, V>
    {
        private int capacity;
        private Dictionary<K, V> cache;
        private LinkedList<K> lruList;

        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            this.cache = new Dictionary<K, V>(capacity);
            this.lruList = new LinkedList<K>();
        }

        public bool TryGet(K key, out V value)
        {
            if (cache.TryGetValue(key, out value))
            {
                lruList.Remove(key);
                lruList.AddLast(key);
                return true;
            }
            value = default(V);
            return false;
        }

        public void Add(K key, V value)
        {
            if (cache.ContainsKey(key))
            {
                lruList.Remove(key);
            }
            else if (cache.Count >= capacity)
            {
                K first = lruList.First.Value;
                lruList.RemoveFirst();
                cache.Remove(first);
            }

            cache[key] = value;
            lruList.AddLast(key);
        }
    }
}
