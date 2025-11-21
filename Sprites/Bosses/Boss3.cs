using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss3 : Boss
    {
        enum MovementStyle
        {
            Vacuum,
            Idle,
            Up,
            Down,
            Starting
        }

        MovementStyle movementStyle;
        TimeSpan timeKeeper;
        double timeToVacuum;

        public Boss3()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 3", 3000, 35, 3000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            timeToVacuum = 20;
            timeKeeper = Time.TotalGameTime();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss3SpriteSheet"));
            SetTextures(GlobalTexture.boss3Texture);
            animation = new Animation(texture, 6, 1, 10, true);
        }

        public override void Update()
        {
            base.Update();

            Rectangle rect = Globals.GameRect;
            rect.X = (int)(rect.X - this.size.X + 10);

            if (!rect.Intersects(this.destinationRectangle))
            {
                this.velocity = new Vector2(-1, 0);
            }
            else
            {
                if (movementStyle == MovementStyle.Starting)
                {
                    movementStyle = MovementStyle.Idle;
                }

                if (movementStyle == MovementStyle.Idle)
                {
                    movementStyle = MovementStyle.Up;
                }

                if (movementStyle == MovementStyle.Up && position.Y <= 0)
                {
                    movementStyle = MovementStyle.Down;
                }

                if (movementStyle == MovementStyle.Down && position.Y >= Globals.GameSize.Y)
                {
                    movementStyle = MovementStyle.Up;
                }

                if (movementStyle != MovementStyle.Vacuum && timeToVacuum <= 10)
                {
                    movementStyle = MovementStyle.Vacuum;
                }
            }

            if (movementStyle == MovementStyle.Up)
            {
                velocity = new Vector2(0, -2);
                color = Color.White;
            }
            else if (movementStyle == MovementStyle.Down)
            {
                velocity = new Vector2(0, 2);
                color = Color.White;
            }
            else if (movementStyle == MovementStyle.Idle)
            {
                velocity = Vector2.Zero;
                color = Color.White;
            }
            else if (movementStyle == MovementStyle.Vacuum)
            {
                velocity = Vector2.Zero;
                color = Color.Red;
                Vacuum();
            }



            SimpleMovement(velocity);

            if (Time.TotalGameTime() - timeKeeper > TimeSpan.FromSeconds(1.0f))
            {
                timeKeeper = Time.TotalGameTime();

                timeToVacuum--;
                if (0 < timeToVacuum && timeToVacuum < 11)
                {
                    animation = new Animation(texture, 6, 1, 15 + (11 - timeToVacuum), true);
                }
                else
                {
                    animation = new Animation(texture, 6, 1, 10, true);
                }

                if (timeToVacuum <= 0)
                {
                    movementStyle = MovementStyle.Idle;
                    timeToVacuum = Globals.Random.Next(15, 30);

                }
            }
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

        private void Vacuum()
        {
            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Sprite sprite = SpriteSystem.spriteList[i];

                if (sprite != null && sprite.visible && sprite.position.X < this.position.X)
                {
                    sprite.position.X += (float)(15 - timeToVacuum);

                    if (sprite.position.Y < this.position.Y)
                    {
                        sprite.position.Y++;
                    }
                    else if (sprite.position.Y >= this.position.Y)
                    {
                        sprite.position.Y--;
                    }
                }
            }
        }


        internal static Boss3 GetBoss3()
        {
            return new Boss3();
        }
    }
}
