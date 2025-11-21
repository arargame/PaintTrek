using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class DrawableSystem
    {
        public static List<DrawableSystem> drawableSystemList;

        static DrawableSystem()
        {
            drawableSystemList = new List<DrawableSystem>();
        }
        public DrawableSystem()
        {

        }

        public virtual void Update()
        {

            for (int i = 0; i < drawableSystemList.Count; i++)
            {
                if (drawableSystemList[i] != null)
                    drawableSystemList[i].Update();
            }
        }

        public virtual void Draw()
        {
            for (int i = 0; i < drawableSystemList.Count; i++)
            {
                if (drawableSystemList[i] != null)
                    drawableSystemList[i].Draw();
            }
        }

        public virtual int GetCountofList()
        {
            return drawableSystemList.Count;
        }
        public static void Add(DrawableSystem o)
        {
            DrawableSystem.drawableSystemList.Add(o);
        }
        public static void Remove(DrawableSystem o)
        {
            DrawableSystem.drawableSystemList.Remove(o);
        }
        public static void Clear()
        {
            SpriteSystem.ClearList();
            EnemySystem.ClearList();
            BossSystem.ClearList();
            GunSystem.ClearList();
            CollectableObjectSystem.ClearList();
            DrawableSystem.drawableSystemList.Clear();
        }

    }
}
