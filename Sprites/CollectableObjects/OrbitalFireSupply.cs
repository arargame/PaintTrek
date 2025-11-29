using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class OrbitalFireSupply:Supply
    {
        public OrbitalFireSupply() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Orbital Fire Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/orbitalFireSupply"));
            SetTexture(GlobalTexture.orbitalFireSupplyTexture, 1, 1, 1, true);
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

        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
        }

        public override void SetVelocity()
        {
            base.SetVelocity();
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }


        internal static OrbitalFireSupply GetOrbitalFireSupply()
        {
            return new OrbitalFireSupply();
        }
        
    }
}
