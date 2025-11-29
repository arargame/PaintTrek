using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Ufo : Enemy
    {
        enum MovementStyle
        {
            starting,
            bouncing
        }

        UfoLaser laser;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        MovementStyle movementStyle;

        public Ufo()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetVelocity();
            SetCharacterInfo("Ufo 1", 150, 30, 10);
            laser = new UfoLaser(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
            movementStyle = MovementStyle.starting;
        }

        public override void Load()
        {
          //  SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/ufoSpriteSheet"));
            SetTexture(GlobalTexture.ufo1Texture, 7, 1, 10, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (!laser.alive)
                laser = new UfoLaser(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(1, 3))
            {
                fireTime = 0;
                laser = new UfoLaser(this);
                if (canFire)
                {
                    laser.Fire();
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;

            if (movementStyle == MovementStyle.starting && position.X < Globals.GameSize.X - size.X)
            {
                movementStyle = MovementStyle.bouncing;
                velocity = new Vector2(-2, -1);
            }

            if (movementStyle == MovementStyle.bouncing)
            {
                Bounce();
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-2, 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }

        private void Bounce()
        {
            if (position.X <= 0)
            {

                if (Globals.Random.Next(0, 10) % 2 == 0)
                {
                    velocity.X *= -1;
                    velocity.Y *= 1;
                }
                else
                {
                    velocity.X *= -1;
                    velocity.Y *= -1;
                }
            }
            else if (position.X >= Globals.GameSize.X)
            {
                if (Globals.Random.Next(0, 10) % 2 == 0)
                {
                    velocity.X *= -1;
                    velocity.Y *= 1;
                }
                else
                {
                    velocity.X *= -1;
                    velocity.Y *= -1;
                }
            }

            if (position.Y <= 0)
            {
                if (Globals.Random.Next(0, 10) % 2 == 0)
                {
                    velocity.X *= 1;
                    velocity.Y *= -1;
                }
                else
                {
                    velocity.X *= -1;
                    velocity.Y *= -1;
                }
            }
            else if (position.Y >= Globals.GameSize.Y)
            {
                if (Globals.Random.Next(0, 10) % 2 == 0)
                {
                    velocity.X *= 1;
                    velocity.Y *= -1;
                }
                else
                {
                    velocity.X *= -1;
                    velocity.Y *= -1;
                }
            }

        }
        internal static Ufo GetUfo()
        {
            return new Ufo();
        }

    }
}
