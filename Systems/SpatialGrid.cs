using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    /// <summary>
    /// Spatial Grid for efficient collision detection.
    /// Divides game world into grid cells to reduce collision checks.
    /// PERFORMANS: O(n*m) → O(n*k) where k = nearby objects only
    /// </summary>
    public class SpatialGrid
    {
        private static SpatialGrid instance;
        public static SpatialGrid Instance
        {
            get
            {
                if (instance == null)
                    instance = new SpatialGrid();
                return instance;
            }
        }

        private Dictionary<Vector2, List<Sprite>> grid;
        private int cellSize;
        private Vector2 gridBounds;

        // Stats for debugging
        private int totalChecks;
        private int culledChecks;

        public SpatialGrid()
        {
            // 100 pixel cell size - balance between granularity and overhead
            cellSize = 100;
            grid = new Dictionary<Vector2, List<Sprite>>();
            gridBounds = Globals.GameSize;
            totalChecks = 0;
            culledChecks = 0;
        }

        /// <summary>
        /// Clear all grid cells (call at start of frame or level)
        /// </summary>
        public void Clear()
        {
            foreach (var cell in grid.Values)
            {
                cell.Clear();
            }
            // Don't clear dictionary itself - reuse cell lists
        }

        /// <summary>
        /// Add sprite to appropriate grid cell(s)
        /// Large sprites may occupy multiple cells
        /// </summary>
        public void Add(Sprite sprite)
        {
            if (sprite == null || !sprite.alive)
                return;

            Vector2 cell = GetCell(sprite.position);
            
            if (!grid.ContainsKey(cell))
                grid[cell] = new List<Sprite>();

            grid[cell].Add(sprite);
        }

        /// <summary>
        /// Get all sprites in cells near the given sprite.
        /// Checks 3x3 grid (9 cells) around sprite.
        /// </summary>
        public List<T> GetNearby<T>(Sprite sprite) where T : Sprite
        {
            List<T> nearby = new List<T>();
            
            if (sprite == null)
                return nearby;

            Vector2 cell = GetCell(sprite.position);

            // Check 3x3 grid around sprite
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 checkCell = new Vector2(cell.X + x, cell.Y + y);
                    
                    if (grid.ContainsKey(checkCell))
                    {
                        foreach (Sprite s in grid[checkCell])
                        {
                            if (s is T && s.alive)
                                nearby.Add((T)s);
                        }
                    }
                }
            }

            return nearby;
        }

        /// <summary>
        /// Get nearby sprites within distance threshold (early culling)
        /// </summary>
        public List<T> GetNearbyWithinDistance<T>(Sprite sprite, float maxDistance) where T : Sprite
        {
            List<T> candidates = GetNearby<T>(sprite);
            List<T> result = new List<T>();

            float maxDistSq = maxDistance * maxDistance;

            foreach (T candidate in candidates)
            {
                totalChecks++;
                
                // Quick distance check before expensive collision
                float dx = sprite.position.X - candidate.position.X;
                float dy = sprite.position.Y - candidate.position.Y;
                float distSq = dx * dx + dy * dy;

                if (distSq <= maxDistSq)
                {
                    result.Add(candidate);
                }
                else
                {
                    culledChecks++;
                }
            }

            return result;
        }

        /// <summary>
        /// Convert world position to grid cell coordinates
        /// </summary>
        private Vector2 GetCell(Vector2 position)
        {
            int cellX = (int)(position.X / cellSize);
            int cellY = (int)(position.Y / cellSize);
            return new Vector2(cellX, cellY);
        }

        /// <summary>
        /// Get statistics for debugging
        /// </summary>
        public string GetStats()
        {
            int cellsWithObjects = 0;
            int totalObjects = 0;

            foreach (var cell in grid.Values)
            {
                if (cell.Count > 0)
                {
                    cellsWithObjects++;
                    totalObjects += cell.Count;
                }
            }

            float cullPercentage = totalChecks > 0 ? (culledChecks * 100f / totalChecks) : 0;

            return $"[SpatialGrid] Cells: {cellsWithObjects}, Objects: {totalObjects}, " +
                   $"Culled: {cullPercentage:F1}% ({culledChecks}/{totalChecks})";
        }

        /// <summary>
        /// Reset stats (call at level start)
        /// </summary>
        public void ResetStats()
        {
            totalChecks = 0;
            culledChecks = 0;
        }
    }
}
