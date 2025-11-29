using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class JellyFish : Enemy
    {
        public JellyFish()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("JellyFish", 40, 30, 30);
            SetVelocity();
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/squidSpriteSheet"));
            SetTexture(GlobalTexture.jellyFishTexture, 3, 2, 5, true);
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
            velocity = new Vector2(-2, 0);
        }

        internal static JellyFish GetSquid()
        {
            return new JellyFish();
        }
    }
}
