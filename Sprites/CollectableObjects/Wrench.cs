using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Wrench:CollectableObject
    {
        public Wrench() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Wrench", 10, 0, 10);
            SetVelocity();
        }
        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/wrenchSpriteSheet"));
            SetTexture(GlobalTexture.wrenchTexture, 3, 1, 5, true);
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

        public double GiveHealth() 
        {
            return 20;
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-4,0);
            if (Globals.Graphics.IsFullScreen)
            {
                velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
            }
        }
        internal static Wrench GetWrench()
        {
            return new Wrench();
        }
    }
}
