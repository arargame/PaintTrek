using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Invader3 : Enemy
    {
        enum MovementStyle
        {
            Starting,
            Left,
            Right,
            Down
        }
        double time;
        MovementStyle movementStyle;

        public Invader3()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Space Invader 3", 30, 30, 30);
            SetVelocity();
            movementStyle = MovementStyle.Starting;
            time = 0;
        }

        public override void Load()
        {
          //  SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/invader3SpriteSheet"));
            SetTexture(GlobalTexture.Invader3Texture, 7, 1, 5, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            time += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (time >= 3) time = 0;

            if (movementStyle == MovementStyle.Starting && position.Y >= size.Y)
            {
                velocity = Vector2.Zero;

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
            position = new Vector2((float)Globals.Random.Next((int)(size.X / 2), (int)(Globals.GameSize.X - size.X / 2)), (float)Globals.Random.Next((int)(-Globals.GameSize.Y / 2), (int)(-size.Y)));
        }

        private void RandomMovement()
        {
            if ((int)time == 2)
            {

                int r = Globals.Random.Next(0, 100);
                if (r <= 60)
                {
                    movementStyle = MovementStyle.Down;

                }
                else
                {
                    if (r % 2 == 0)
                    {
                        movementStyle = MovementStyle.Right;
                    }
                    else
                    {
                        movementStyle = MovementStyle.Left;
                    }
                }
                time = 0;
            }
        }

        internal static Invader3 GetInvader3()
        {
            return new Invader3();
        }
    }
}
