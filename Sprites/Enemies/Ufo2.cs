using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Ufo2 : Enemy
    {
        Ufo2Fire ufoFire;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        public Ufo2()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetVelocity();
            SetCharacterInfo("Ufo 2", 50, 20, 10);
            ufoFire = new Ufo2Fire(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/ufo2SpriteSheet"));
            SetTextures(GlobalTexture.ufo2Texture);
            animation = new Animation(texture, 4, 2, 8, true);
        }

        public override void Update()
        {
            base.Update();

            if (!ufoFire.alive)
                ufoFire = new Ufo2Fire(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 5))
            {
                fireTime = 0;
                ufoFire = new Ufo2Fire(this);
                if (canFire)
                {
                    ufoFire.Fire();
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;

            SimpleMovement(velocity);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-Globals.Random.Next(3, 8), 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static Ufo2 GetUfo2()
        {
            return new Ufo2();
        }
    }
}
