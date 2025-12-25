using System;
using System.Collections.Generic;

namespace PaintTrek
{
    /// <summary>
    /// Generic object pool for all bullet types.
    /// Reuses inactive bullets instead of creating new ones.
    /// </summary>
    public static class BulletPool
    {
        private static Dictionary<Type, List<Bullet>> pools = new Dictionary<Type, List<Bullet>>();
        
        /// <summary>
        /// Get a bullet from the pool or create a new one if none available.
        /// </summary>
        public static T Get<T>(Sprite owner) where T : Bullet
        {
            Type type = typeof(T);
            
            // Ensure pool exists for this type
            if (!pools.ContainsKey(type))
            {
                pools[type] = new List<Bullet>();
                //System.Diagnostics.Debug.WriteLine($"[BulletPool] Created new pool for {type.Name}");
            }
            
            List<Bullet> pool = pools[type];
            
            // Find an inactive bullet in the pool
            for (int i = 0; i < pool.Count; i++)
            {
                Bullet bullet = pool[i];

                if (!bullet.alive && !bullet.visible)
                {
                    // Reuse this bullet
                    bullet.Reset(owner);
                    //System.Diagnostics.Debug.WriteLine($"[BulletPool] Reused {type.Name} from pool (Pool size: {pool.Count})");
                    return (T)bullet;
                }
            }
            
            // No inactive bullet found, create a new one
            T newBullet = (T)Activator.CreateInstance(type, owner);
            pool.Add(newBullet);
            //System.Diagnostics.Debug.WriteLine($"[BulletPool] Created new {type.Name} (Pool size: {pool.Count})");
            return newBullet;
        }
        
        /// <summary>
        /// Clear all pools (call when level ends or game resets)
        /// </summary>
        public static void ClearAll()
        {
            foreach (var pool in pools.Values)
            {
                foreach (var bullet in pool)
                {
                    if (bullet != null)
                        bullet.alive = false;
                }
                pool.Clear();
            }
            pools.Clear();
            System.Diagnostics.Debug.WriteLine("[BulletPool] All pools cleared");
        }
        
        /// <summary>
        /// Get pool statistics for debugging
        /// </summary>
        public static string GetStats()
        {
            string stats = "[BulletPool Stats]\n";
            foreach (var kvp in pools)
            {
                int total = kvp.Value.Count;
                int active = 0;
                int inactive = 0;
                
                foreach (var bullet in kvp.Value)
                {
                    if (bullet.alive || bullet.visible)
                        active++;
                    else
                        inactive++;
                }
                
                stats += $"{kvp.Key.Name}: Total={total}, Active={active}, Inactive={inactive}\n";
            }
            return stats;
        }
    }
}
