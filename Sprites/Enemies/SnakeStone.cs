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
        private readonly bool isBossSegment;

        public SnakeStone()
            : this(false)
        {
        }

        internal SnakeStone(bool isBossSegment)
        {
            this.isBossSegment = isBossSegment;
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
            SetTexture(GlobalTexture.snakeStoneTexture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity); // Redundant, handled by base.Update()

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

        public override void Kill()
        {
            // A snake boss wraps around the arena. Its tail can briefly be outside
            // the game rectangle while following the head, which is not an escape.
            if (isBossSegment)
                return;

            base.Kill();
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
