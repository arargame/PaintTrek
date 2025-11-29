using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Boss5 : Boss
    {
        enum MovementStyle
        {
            starting,
            bouncing
        }
        MovementStyle movementStyle;
        DiffusedLightSystem dls;

        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        public Boss5()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 5", 2000, 30, 5000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            dls = new DiffusedLightSystem(this, 8, "Ufo2Fire", 5);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
            movementStyle = MovementStyle.starting;
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss5SpriteSheet"));
            SetTexture(GlobalTexture.boss5Texture, 4, 1, 5, true);
        }

        public override void Update()
        {
            base.Update();

            if (position.X <= (400) && movementStyle == MovementStyle.starting)
            {
                this.velocity = new Vector2(-2, -2);
                movementStyle = MovementStyle.bouncing;
            }
            else if (movementStyle == MovementStyle.bouncing)
            {

                if (position.X <= 0)
                {
                    this.velocity.X *= -1;
                    this.velocity.Y *= 1;
                }
                else if (position.X + size.X / 2 >= Globals.GameSize.X)
                {
                    this.velocity.X *= -1;
                    this.velocity.Y *= 1;
                }

                if (position.Y <= 0)
                {
                    this.velocity.X *= 1;
                    this.velocity.Y *= -1;
                }
                else if (position.Y + size.Y / 2 >= Globals.GameSize.Y)
                {
                    this.velocity.X *= 1;
                    this.velocity.Y *= -1;
                }
            }

            SimpleMovement(velocity);

            FireSettings();
        }

        public void FireSettings()
        {
            //dls.Reload();

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 6))
            {
                fireTime = 0;
                dls = new DiffusedLightSystem(this, 8, "Ufo2Fire", 5);
                if (canFire)
                {
                    dls.Fire();
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-1, 0);
        }

        internal static Boss5 GetBoss5()
        {
            return new Boss5();
        }
    }
}
