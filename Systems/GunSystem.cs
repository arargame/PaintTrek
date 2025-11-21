using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class GunSystem : DrawableSystem
    {
        public static List<Bullet> bulletList;

        static GunSystem()
        {
            bulletList = new List<Bullet>();
        }
        public GunSystem()
        {
            DrawableSystem.Add(this);
        }

        public static void Add(Bullet o)
        {
            GunSystem.bulletList.Add(o);
        }
        public static void Remove(Bullet o)
        {
            GunSystem.bulletList.Remove(o);
        }
        public static void ClearList()
        {
            GunSystem.bulletList.Clear();
        }

    }
}
