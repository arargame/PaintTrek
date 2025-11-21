using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class BossSystem:DrawableSystem
    {
        public static List<Boss> bossList;
        public static bool bossHasFallen = false;

        static BossSystem()
        {
            bossList = new List<Boss>();
            bossHasFallen = false;
        }

        public BossSystem()
        {
            DrawableSystem.Add(this);
            bossHasFallen = false;
        }

        public static void Add(Boss o)
        {
            BossSystem.bossList.Add(o);
        }
        public static void Remove(Boss o)
        {
            BossSystem.bossList.Remove(o);
            bossHasFallen = true;
        }
        public static void ClearList()
        {
            BossSystem.bossList.Clear();
        }
    }
}
