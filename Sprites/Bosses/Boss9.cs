using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss9 : Boss
    {
        enum MovementStyle
        {
            Starting,
            Onward,
            Backward,
            Up,
            Down,
            Idle
        }
        MovementStyle movementStyle;
        //TripleCometFireSpriteSheet fires;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        double timer;

        public Boss9()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 9", 2000, 30, 9000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            timer = 0;
            //fires = new TripleCometFireSpriteSheet(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss9SpriteSheet"));
            SetTexture(GlobalTexture.boss9Texture, 3, 2, 6, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (movementStyle == MovementStyle.Starting && position.X < Globals.GameSize.X - size.X / 2)
            {
                velocity = Vector2.Zero;
                movementStyle = MovementStyle.Idle;
            }

            if (movementStyle == MovementStyle.Idle)
            {
                velocity = Vector2.Zero;
                if (Globals.Random.Next(0, 10) % 2 == 0)
                    movementStyle = MovementStyle.Up;
                else movementStyle = MovementStyle.Down;
            }

            if (movementStyle == MovementStyle.Onward)
            {
                if (position.X - size.X / 2 <= 0)
                {
                    velocity = Vector2.Zero;
                    movementStyle = MovementStyle.Backward;
                }
                else
                    velocity = new Vector2(-7, 0);
            }
            if (movementStyle == MovementStyle.Backward)
            {
                if (position.X > Globals.GameSize.X - size.X / 2)
                {
                    movementStyle = MovementStyle.Idle;
                }
                else
                    velocity = new Vector2(3, 0);
            }
            if (movementStyle == MovementStyle.Up)
            {
                if (position.Y <= 0)
                {
                    velocity = Vector2.Zero;
                    movementStyle = MovementStyle.Down;
                }
                else
                    velocity = new Vector2(0, -3);
            }
            if (movementStyle == MovementStyle.Down)
            {
                if (position.Y >= Globals.GameSize.Y)
                {
                    velocity = Vector2.Zero;
                    movementStyle = MovementStyle.Up;
                }
                else
                    velocity = new Vector2(0, 3);
            }

            if (movementStyle == MovementStyle.Starting)
            {
                velocity = new Vector2(-2, 0);
            }

            timer += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if ((int)timer % 10 == 0 && movementStyle != MovementStyle.Onward)
            {
                movementStyle = MovementStyle.Onward;
            }

            FireSettings();

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        public void FireSettings()
        {
            //if (!fires.alive)
            //  fires = new TripleCometFireSpriteSheet(this);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(1, 2))
            {
                fireTime = 0;
                //fires = new TripleCometFireSpriteSheet(this);
                if (canFire)
                {
                    //  fires.Fire();
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
        public override void SetStartingPosition()
        {
            float width = Globals.GameSize.X;
            float height = Globals.GameSize.Y / 2 - this.size.Y / 2;
            position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), height);
            position.X = MathHelper.Clamp(position.X, width, width + width / 3);
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-2, 0);
            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static Boss9 GetBoss9()
        {
            return new Boss9();
        }
    }
}
