using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class SpriteSystem:DrawableSystem
    {
        public static List<Sprite> spriteList;

        static SpriteSystem()
        {
            spriteList = new List<Sprite>();

        }
        public SpriteSystem()
        {
            DrawableSystem.Add(this);
        }
        public override void Update()
        {
            // Reverse loop to safely remove items while iterating
            for (int i = spriteList.Count - 1; i >= 0; i--)
            {
                // Texture null veya disposed ise kaldır (ama Bullet değilse - Bullet'lar kendilerini yönetir)
                if (!(spriteList[i] is Bullet) && 
                    (spriteList[i].texture == null || spriteList[i].texture.IsDisposed))
                {
                    Remove(spriteList[i]);
                    continue;
                }
            }

            for (int i = spriteList.Count - 1; i >= 0; i--)
            {
                // Texture null check
                if (i < spriteList.Count && spriteList[i] != null && 
                    spriteList[i].texture != null && !spriteList[i].texture.IsDisposed)
                {
                    spriteList[i].Update();
                }
            }

        }

        public override void Draw()
        {
            for (int i = 0; i < GunSystem.bulletList.Count; i++)
            {
                if (GunSystem.bulletList[i] != null && 
                    GunSystem.bulletList[i].texture != null && 
                    !GunSystem.bulletList[i].texture.IsDisposed)
                {
                    GunSystem.bulletList[i].Draw();
                }
            }


            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                if (SpriteSystem.spriteList[i] != null && 
                    SpriteSystem.spriteList[i].texture != null && 
                    !SpriteSystem.spriteList[i].texture.IsDisposed)
                {
                    if (SpriteSystem.spriteList[i] is Bullet) continue;

                    SpriteSystem.spriteList[i].Draw();
                }
            }


        }

        public override int GetCountofList()
        {
            return SpriteSystem.spriteList.Count;
        }
        public static void Add(Sprite o)
        {
            SpriteSystem.spriteList.Add(o);
        }

        public static void Remove(Sprite o)
        {
            SpriteSystem.spriteList.Remove(o);
        }
        public static void ClearList()
        {
            SpriteSystem.spriteList.Clear();
        }
    }
}
