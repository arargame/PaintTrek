using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Invader1 : Enemy
    {
        enum MovementStyle
        {
            Starting,
            Left,
            Right,
            Down
        }
        Invader1Gun gun;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;
        MovementStyle movementStyle;
        double time;

        public Invader1()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Space Invader 1", 30, 30, 30);
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            gun = new Invader1Gun(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
            time = 0;
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/invader1SpriteSheet"));
            SetTextures(GlobalTexture.Invader1Texture);
            animation = new Animation(texture, 5, 2, 5, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            time += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;


            if (movementStyle == MovementStyle.Starting && position.Y >= (float)Globals.Random.Next((int)size.Y / 2, (int)(2 * size.Y)))
            {
                velocity = Vector2.Zero;
                if (time >= 2 && time < 10)
                    RandomMovement();
            }

            if (movementStyle != MovementStyle.Starting)
            {
                RandomMovement();
            }

            if (movementStyle == MovementStyle.Left)
            {
                velocity = new Vector2(-2, 0);
            }
            else if (movementStyle == MovementStyle.Right)
            {
                velocity = new Vector2(2, 0);
            }
            else if (movementStyle == MovementStyle.Down)
            {
                velocity = new Vector2(0, 2);
            }


            if (!this.destinationRectangle.Intersects(Globals.GameRect) && movementStyle != MovementStyle.Starting)
            {
                alive = false;
            }

            FireSettings();
        }

        private void FireSettings()
        {
            if (!gun.alive)
                gun = new Invader1Gun(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 5))
            {
                fireTime = 0;
                gun = new Invader1Gun(this);
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

        private void RandomMovement()
        {
            if (movementStyle == MovementStyle.Starting)
            {

                int r = Globals.Random.Next(0, 100);
                if (r % 2 == 0)
                {
                    movementStyle = MovementStyle.Right;
                }
                else
                {
                    movementStyle = MovementStyle.Left;
                }
            }

            if (movementStyle == MovementStyle.Right && position.X >= Globals.GameSize.X - size.X / 2)
            {
                movementStyle = MovementStyle.Left;
            }
            else if (movementStyle == MovementStyle.Left && position.X <= size.X / 2)
            {
                movementStyle = MovementStyle.Right;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(0, 2);
        }
        public override void SetStartingPosition()
        {
            position = new Vector2((float)(Globals.GameSize.X / 2 - size.X / 2), (float)Globals.Random.Next((int)(-Globals.GameSize.Y / 2), (int)(-size.Y)));
        }

        internal static Invader1 GetInvader1()
        {
            return new Invader1();
        }
    }
}
