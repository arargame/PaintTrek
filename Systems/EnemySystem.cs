using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class EnemySystem : DrawableSystem
    {
        public static List<Enemy> enemyList;

        static EnemySystem()
        {
            enemyList = new List<Enemy>();
        }
        public EnemySystem()
        {
            DrawableSystem.Add(this);
        }

        public static void Add(Enemy o)
        {
            EnemySystem.enemyList.Add(o);
        }
        public static void Remove(Enemy o)
        {
            EnemySystem.enemyList.Remove(o);
        }
        public static void ClearList()
        {
            EnemySystem.enemyList.Clear();
        }
    }
}
