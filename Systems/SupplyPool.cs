using System;
using System.Collections.Generic;

namespace PaintTrek
{
    /// <summary>
    /// Generic object pool for all Supply types.
    /// Reuses inactive supplies instead of creating new ones.
    /// </summary>
    public static class SupplyPool
    {
        private static Dictionary<Type, List<Supply>> pools = new Dictionary<Type, List<Supply>>();
        
        /// <summary>
        /// Get a supply from the pool or create a new one if none available.
        /// </summary>
        public static T Get<T>() where T : Supply, new()
        {
            Type type = typeof(T);
            
            // Ensure pool exists for this type
            if (!pools.ContainsKey(type))
            {
                pools[type] = new List<Supply>();
                System.Diagnostics.Debug.WriteLine($"[SupplyPool] Created new pool for {type.Name}");
            }
            
            List<Supply> pool = pools[type];
            
            // Find an inactive supply in the pool
            for (int i = 0; i < pool.Count; i++)
            {
                Supply supply = pool[i];

                if (!supply.alive && !supply.visible)
                {
                    // Reuse this supply
                    supply.Reset(); // Only reset state, do not re-register
                    supply.alive = true;
                    supply.visible = true;
                    //System.Diagnostics.Debug.WriteLine($"[SupplyPool] Reused {type.Name} from pool (Pool size: {pool.Count})");
                    return (T)supply;
                }
            }
            
            // No inactive supply found, create a new one
            T newSupply = new T();
            pool.Add(newSupply);
            //System.Diagnostics.Debug.WriteLine($"[SupplyPool] Created new {type.Name} (Pool size: {pool.Count})");
            return newSupply;
        }
        
        /// <summary>
        /// Clear all pools (call when level ends or game resets)
        /// </summary>
        public static void ClearAll()
        {
            foreach (var pool in pools.Values)
            {
                foreach (var supply in pool)
                {
                    if (supply != null)
                        supply.alive = false;
                }
                pool.Clear();
            }
            pools.Clear();
            System.Diagnostics.Debug.WriteLine("[SupplyPool] All pools cleared");
        }
        
        /// <summary>
        /// Get pool statistics for debugging
        /// </summary>
        public static string GetStats()
        {
            string stats = "[SupplyPool Stats]\n";
            foreach (var kvp in pools)
            {
                int total = kvp.Value.Count;
                int active = 0;
                int inactive = 0;
                
                foreach (var supply in kvp.Value)
                {
                    if (supply.alive || supply.visible)
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
