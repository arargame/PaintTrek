using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss7 : Boss
    {
        enum MovementStyle
        {
            Starting,
            Left,
            Right,
            Up,
            Down
        }
        Boss7Fire gun;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;
        MovementStyle movementStyle;
        double time;

        public Boss7()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 7", 1000, 30, 7000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            time = 0;
            gun = new Boss7Fire(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss7SpriteSheet"));
            SetTexture(GlobalTexture.boss7Texture, 1, 5, 5, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            time += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;


            if (movementStyle == MovementStyle.Starting && position.Y >= size.Y)
            {
                velocity = Vector2.Zero;
                movementStyle = MovementStyle.Left;
                RandomMovement();
            }

            if (movementStyle != MovementStyle.Starting)
            {
                RandomMovement();
            }

            if (movementStyle == MovementStyle.Left)
            {
                velocity = new Vector2(-4, 0);
            }
            else if (movementStyle == MovementStyle.Right)
            {
                velocity = new Vector2(4, 0);
            }
            else if (movementStyle == MovementStyle.Down)
            {
                velocity = new Vector2(0, 6);
            }
            else if (movementStyle == MovementStyle.Up)
            {
                velocity = new Vector2(0, -6);
            }
            else if (movementStyle == MovementStyle.Starting)
            {
                velocity = new Vector2(0, 2);
            }

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;

            if (!this.destinationRectangle.Intersects(Globals.GameRect) && movementStyle != MovementStyle.Starting)
            {
                alive = false;
            }

            FireSettings();

        }

        private void FireSettings()
        {
            if (!gun.alive)
                gun = new Boss7Fire(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(1, 2))
            {
                fireTime = 0;
                gun = new Boss7Fire(this);
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

        public override void Draw()
        {
            base.Draw();
        }

        private void RandomMovement()
        {

            if ((int)time % Globals.Random.Next(7,9) == 1 && movementStyle != MovementStyle.Down && movementStyle != MovementStyle.Up)
                movementStyle = MovementStyle.Down;

            if (movementStyle == MovementStyle.Right && position.X >= Globals.GameSize.X - size.X / 2)
            {
                movementStyle = MovementStyle.Left;
            }
            else if (movementStyle == MovementStyle.Left && position.X <= size.X / 2)
            {
                movementStyle = MovementStyle.Right;
            }

            if (movementStyle == MovementStyle.Down && position.Y >= Globals.GameSize.Y - size.Y)
            {
                movementStyle = MovementStyle.Up;
            }
            else if (movementStyle == MovementStyle.Up && position.Y <= size.Y)
            {
                movementStyle = MovementStyle.Starting;
            }
        }

        public override void SetStartingPosition()
        {
            position = new Vector2((float)(Globals.GameSize.X / 2 - size.X / 2), (float)Globals.Random.Next((int)(-Globals.GameSize.Y / 2), (int)(-size.Y)));
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(0, 2);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static Boss7 GetBoss7()
        {
            return new Boss7();
        }
    }
}
