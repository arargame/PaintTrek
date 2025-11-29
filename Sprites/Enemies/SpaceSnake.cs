using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class SpaceSnake : Enemy
    {
        public SpaceSnake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Space Snake", 30, 30, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/spaceSnakeSpriteSheet"));
            SetTexture(GlobalTexture.spaceSnakeTexture, 6, 1, 12, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Kill()
        {
            base.Kill();
        }

        public override void MakeVisible()
        {
            base.MakeVisible();
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-6, 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static SpaceSnake GetSpaceSnake()
        {
            return new SpaceSnake();
        }
    }
}
