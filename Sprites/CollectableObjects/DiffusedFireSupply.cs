using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class DiffusedFireSupply:Supply
    {
        public DiffusedFireSupply() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Diffused Fire Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/diffusedFireSupply"));
            SetTextures(GlobalTexture.diffusedFireSupplyTexture);
            animation = new Animation(texture, 1, 1, 1, true);
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
        internal static DiffusedFireSupply GetDiffusedFireSupply()
        {
            return new DiffusedFireSupply();
        }
    }
}
