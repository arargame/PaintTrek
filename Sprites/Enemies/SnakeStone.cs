using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class SnakeStone : Enemy
    {
        public SnakeStone()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Snake Stone", 350, 30, 100);
            SetVelocity();
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/snakeStone"));
            SetTextures(GlobalTexture.snakeStoneTexture);
            animation = new Animation(texture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (GetHealth() < 250)
            {
                color = Color.Gray;
            }
            else color = Color.White;

            if (BossSystem.bossHasFallen)
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
            velocity = Vector2.Zero;
        }

        internal static SnakeStone GetSnakeStone()
        {
            return new SnakeStone();
        }
    }
}
