using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class InfoSystem
    {
        Vector2 position;
        List<string> items;
        double FPS;
        double counter;

        public InfoSystem(Vector2 position)
        {
            this.position = position;
            items = new List<string>();

            items.Add("InfoSystem :");
            for (int i = 0; i < 12; i++)
            {
                items.Add("");
            }
            counter = 0;
        }

        ~InfoSystem() 
        {
            UnloadContent();
        }

        public void UnloadContent()
        {
            items.Clear();
        }

        public void Update()
        {
            FPS = 1 / Globals.GameTime.ElapsedGameTime.TotalSeconds;
            items[1] = "FPS :" + FPS;
            items[2] = "Systems :" + DrawableSystem.drawableSystemList.Count;
            items[3] = "Sprites :" + SpriteSystem.spriteList.Count;
            items[4] = "Enemies :" + EnemySystem.enemyList.Count;
            items[5] = "Guns :" + GunSystem.bulletList.Count;
            items[6] = "Collectables :" + CollectableObjectSystem.collactableObjectList.Count;
            items[7] = "Score :" + Level.Score;
            items[8] = "Level :" + Level.LevelCounter;
            counter += Globals.GameTime.TotalGameTime.Ticks;
            items[9] = "TIME :" + Globals.XX;
            items[10] = "Bosses :" + BossSystem.bossList.Count;
            items[11] = "Explosions :" + ExplosionSystem.expoInfos.Count;
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            for (int i = 0; i < items.Count; i++)
            {
             //   Globals.SpriteBatch.DrawString(Globals.MenuFont, items[i], new Vector2(position.X, (position.Y + i * 20)), Color.White);
            }

            Globals.SpriteBatch.End();
        }

        
    }
}
