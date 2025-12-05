using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class BossExplosion
    {
        Vector2 [] positions;

        Boss boss;

        bool isActive;

        double time;

        bool[] explode;

        Animation animation;

        Texture2D texture;

        Vector2 size;

        bool exit;

        public BossExplosion(Boss boss) 
        {
            this.boss = boss;
            positions=new Vector2[4];
            isActive = false;
            time = 0;
            explode=new bool[4];
            texture = Globals.Content.Load<Texture2D>("Explosions/explosion");
            animation = new Animation(texture,12,1,12,true);
            exit = false;
            SoundManager.Load("explosion", "Sounds/SoundEffects/explosion");
        }

        public void Update() 
        {
            if (isActive)
            {
                time += Globals.GameTime.ElapsedGameTime.TotalSeconds*2;
                animation.Update();
            }

            if ((int)time == 4)
            {
                time = 0;
                isActive = false;
                for (int i = 0; i < explode.Length; i++)
                {
                    explode[i] = false;
                }
            }

            if (!boss.alive && !exit) 
            {
                positions[0] = new Vector2(boss.destinationRectangle.Center.X,boss.destinationRectangle.Top);
                positions[1]=new Vector2(boss.destinationRectangle.Left,boss.destinationRectangle.Top);
                positions[2]=new Vector2(boss.destinationRectangle.Left,boss.destinationRectangle.Center.Y);
                positions[3] = new Vector2(boss.destinationRectangle.Center.X,boss.destinationRectangle.Center.Y);
                isActive = true;
                size = boss.size;
                exit = true;
            }

            if(!explode[(int)time] && isActive)
            {
                // SoundSystem deathSound = new SoundSystem("Sounds/SoundEffects/explosion", 1f, 0f, 0f, false, "Unknown", "Unknown");
                SoundManager.Play("explosion");
                explode[(int)time] = true;
            }
        }

        public void Draw() 
        {
            Globals.SpriteBatch.Begin();
            if(isActive && explode[(int)time])
                Globals.SpriteBatch.Draw(texture, new Rectangle((int)positions[(int)time].X-100, (int)positions[(int)time].Y,(int)size.X,(int)size.Y), animation.FrameBounds, Color.White);
            Globals.SpriteBatch.End();
        }
    }
}
