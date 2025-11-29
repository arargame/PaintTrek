using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss2 : Boss
    {
        enum MovementStyle
        {
            Onward,
            Backward,
            Idle,
            Up,
            Down,
            Starting
        }
        MovementStyle movementStyle;
        double time;

        public Boss2()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 2", 1750, 30, 2000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            time = 0;
        }

        public override void Load()
        {
            SetTexture(GlobalTexture.boss2Texture, 5, 1, 5, true);
        }

        public override void Update()
        {
            base.Update();

            time += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            Rectangle rect = Globals.GameRect;
            rect.X = (int)(rect.X - this.size.X + 10);

            SimpleMovement(velocity);

            if ((int)time % 5 == 0 && movementStyle == MovementStyle.Idle)
            {
                movementStyle = MovementStyle.Onward;
            }

            if (movementStyle == MovementStyle.Backward && position.X > Globals.GameSize.X - size.X)
            {
                movementStyle = MovementStyle.Idle;
            }

            if (movementStyle == MovementStyle.Starting && position.X <= Globals.GameSize.X - size.X)
            {
                movementStyle = MovementStyle.Idle;
            }

            if (movementStyle == MovementStyle.Starting)
            {
                velocity = new Vector2(-2, 0);
            }
            else if (movementStyle == MovementStyle.Idle)
            {
                velocity = Vector2.Zero;
                ResetRotation();
            }
            else if (movementStyle == MovementStyle.Onward)
            {
                if (position.X > (size.X / 2))
                    velocity.X--;
                else
                {
                    velocity.X = 0;

                    if (Globals.Random.Next() % 2 == 0)
                        movementStyle = MovementStyle.Up;
                    else movementStyle = MovementStyle.Down;
                }

                Rotate(0.5f);
            }
            else if (movementStyle == MovementStyle.Backward)
            {
                Vector2 location = new Vector2(Globals.GameSize.X - size.X, Globals.GameSize.Y / 2);
                velocity = Vector2.Zero;
                if (position.X > location.X)
                {
                    //position.X-=2;
                    velocity.X -= 2;
                }
                else
                    //position.X+=2;
                    velocity.X += 2;

                if (position.Y > location.Y)
                {
                    //position.Y--;
                    velocity.Y--;
                }
                else
                    //position.Y++;
                    velocity.Y++;
            }
            else if (movementStyle == MovementStyle.Up)
            {
                if (position.Y > size.Y / 2)
                    velocity.Y = -4;
                else
                    movementStyle = MovementStyle.Backward;
            }
            else if (movementStyle == MovementStyle.Down)
            {
                if (position.Y < Globals.GameSize.Y - size.Y / 2)
                    velocity.Y = 4;
                else
                    movementStyle = MovementStyle.Backward;
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
        public override void SetVelocity()
        {
            velocity = Vector2.Zero;
        }

        internal static Boss2 GetBoss2()
        {
            return new Boss2();
        }
    }
}
