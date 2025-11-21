using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss8 : Boss
    {
        enum MovementStyle
        {
            starting,
            Up,
            Down
        }

        double timer;
        MovementStyle movementStyle;
        public Boss8()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 8", 3000, 30, 8000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            timer = 0;
            movementStyle = MovementStyle.starting;
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss8SpriteSheet"));
            SetTextures(GlobalTexture.boss8Texture);
            animation = new Animation(texture, 3, 1, 3, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (movementStyle == MovementStyle.starting && position.X <= Globals.GameSize.X - size.X / 2)
            {
                velocity = Vector2.Zero;
                int r = Globals.Random.Next(0, 10);

                if (r % 2 == 0)
                    movementStyle = MovementStyle.Down;
                else movementStyle = MovementStyle.Up;
            }

            if (movementStyle == MovementStyle.Up)
            {
                if (position.Y <= 0)
                    movementStyle = MovementStyle.Down;
                else
                    velocity = new Vector2(0, -1);

            }
            if (movementStyle == MovementStyle.Down)
            {
                if (position.Y >= Globals.GameSize.Y)
                    movementStyle = MovementStyle.Up;
                else
                    velocity = new Vector2(0, 1);
            }

            timer += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if ((int)timer == 2)
            {
                ChildTrilobit.GetChildTrilobit();
                timer = 0;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-1, 0);
        }
        public override void SetStartingPosition()
        {
            float width = Globals.GameSize.X;
            float height = Globals.GameSize.Y / 2 - this.size.Y / 2;
            position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), height);
            position.X = MathHelper.Clamp(position.X, width, width + width / 3);
        }

        internal static Boss8 GetBoss8()
        {
            return new Boss8();
        }
    }
}
