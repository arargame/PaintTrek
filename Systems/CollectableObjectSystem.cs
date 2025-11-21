using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class CollectableObjectSystem : DrawableSystem
    {
        public static List<CollectableObject> collactableObjectList;

        static CollectableObjectSystem()
        {
            collactableObjectList = new List<CollectableObject>();
        }

        public CollectableObjectSystem()
        {
            DrawableSystem.Add(this);
        }
        public static void Add(CollectableObject o)
        {
            CollectableObjectSystem.collactableObjectList.Add(o);
        }
        public static void Remove(CollectableObject o)
        {
            CollectableObjectSystem.collactableObjectList.Remove(o);
        }
        public static void ClearList()
        {
            CollectableObjectSystem.collactableObjectList.Clear();
        }
    }
}
