using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class RocketSupply:Supply
    {
        public RocketSupply() 
        {
            Initialize();
            
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Rocket Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/rocketSupply"));
            SetTexture(GlobalTexture.rocketSupplyTexture, 1, 1, 1, true);
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

        public override void SetVelocity()
        {
            base.SetVelocity();
        }
        internal static RocketSupply GetRocketSupply() 
        {
            return new RocketSupply();
        }
    }
}
