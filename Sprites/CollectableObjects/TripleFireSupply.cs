using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class TripleFireSupply:Supply
    {
        public TripleFireSupply() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Triple Fire Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/tripleFireSupply"));
            SetTextures(GlobalTexture.tripleFireSupplyTexture);
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

        public override void SetVelocity()
        {
            base.SetVelocity();
        }

        internal static TripleFireSupply GetTripleFireSupply()
        {
            return new TripleFireSupply();
        }
    }
}
