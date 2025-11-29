using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Invader2 : Enemy
    {
        enum MovementStyle
        {
            Starting,
            Left,
            Right
        }
        MovementStyle movementStyle;

        Invader2Gun gun;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;
        double time;
        double speed;

        public Invader2()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Space Invader 2", 10, 10, 30);
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            gun = new Invader2Gun(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
            time = 0;
            speed = 1;
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/invader2SpriteSheet"));
            SetTexture(GlobalTexture.Invader2Texture, 5, 2, 5, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            time += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;


            if (movementStyle == MovementStyle.Starting && position.Y >= 0)
            {
                velocity = Vector2.Zero;

                if (position.X > Globals.GameSize.X / 2)
                    movementStyle = MovementStyle.Left;
                else movementStyle = MovementStyle.Right;
            }
            speed += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if (movementStyle == MovementStyle.Left)
            {
                velocity.X = -(float)speed;
                velocity.Y = 2;

            }
            else if (movementStyle == MovementStyle.Right)
            {
                velocity.X = (float)speed;
                velocity.Y = 2;
            }


            if (!this.destinationRectangle.Intersects(Globals.GameRect) && movementStyle != MovementStyle.Starting)
            {
                alive = false;
            }

            FireSettings();
        }

        public override void Draw()
        {
            base.Draw();
        }

        private void FireSettings()
        {
            if (!gun.alive)
                gun = new Invader2Gun(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 5))
            {
                fireTime = 0;
                gun = new Invader2Gun(this);
                if (canFire)
                {
                    gun.Fire();
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;
        }


        public override void SetVelocity()
        {
            velocity = new Vector2(0, 2);
        }
        public override void SetStartingPosition()
        {
            if (Globals.Random.Next(10) % 2 == 0)
            {
                position = new Vector2(size.X / 2, -size.Y);
            }
            else
            {
                position = new Vector2(Globals.GameSize.X - size.X / 2, -size.Y);
            }
        }

        internal static Invader2 GetInvader2()
        {
            return new Invader2();
        }
    }
}
