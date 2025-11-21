using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class ExplosionSystem
    {
        BossExplosion bossExplosions;
        public static Dictionary<Sprite, Animation> expoInfos;
        Rectangle explosionRect;
        public Rectangle ExplosionRect
        {
            set { explosionRect = value; }
            get { return explosionRect; }
        }

        public ExplosionSystem()
        {
            expoInfos = new Dictionary<Sprite, Animation>();
        }

        public void Update()
        {
            ICollection<Animation> animations = expoInfos.Values;
            foreach (var a in animations)
            {
                a.Update();
            }

            Clear();

            if (bossExplosions != null)
                bossExplosions.Update();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            Explode();

            Globals.SpriteBatch.End();

            if (bossExplosions != null)
                bossExplosions.Draw();
        }

        public static void AddExplosion(Sprite sprite)
        {

            Animation animation = new Animation(GlobalTexture.explosionTexture, 12, 1, 10, false);
            expoInfos.Add(sprite, animation);

        }

        public void AddBossExplosion(Boss b) 
        {
            bossExplosions=new BossExplosion(b);
        }

        public void Explode()
        {
            ICollection<Sprite> sprites = expoInfos.Keys;

            foreach (Sprite s in sprites)
            {
                ExplosionRect = s.destinationRectangle;

                if (expoInfos[s].Active)
                    Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Explosions/explosion"), ExplosionRect, expoInfos[s].FrameBounds, Color.White);
            }

        }

        public void Clear()
        {
            List<Sprite> s = new List<Sprite>(expoInfos.Keys);
            foreach (Sprite item in s)
            {
                if (!expoInfos[item].Active)
                    expoInfos.Remove(item);
            }

        }
    }
}
