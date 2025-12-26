using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class CollectableObjectSystem : DrawableSystem
    {
        public static List<CollectableObject> collactableObjectList;

        public static List<Bubble> activeBubbles; // OPTIMIZATION: Cache for bubble collisions

        static CollectableObjectSystem()
        {
            collactableObjectList = new List<CollectableObject>();
            activeBubbles = new List<Bubble>();
        }

        public CollectableObjectSystem()
        {
            DrawableSystem.Add(this);
        }
        public static void Add(CollectableObject o)
        {
            // System.Diagnostics.Debug.WriteLine($"[CollectableObjectSystem] Adding {o.GetType().Name}");
            CollectableObjectSystem.collactableObjectList.Add(o);
            
            // Add to cache if Bubble
            if (o is Bubble bubble)
            {
                activeBubbles.Add(bubble);
            }
        }
        public static void Remove(CollectableObject o)
        {
            CollectableObjectSystem.collactableObjectList.Remove(o);
            
            // Remove from cache if Bubble
            if (o is Bubble bubble)
            {
                activeBubbles.Remove(bubble);
            }
        }
        public static void ClearList()
        {
            CollectableObjectSystem.collactableObjectList.Clear();
            activeBubbles.Clear();
        }
    }
}
