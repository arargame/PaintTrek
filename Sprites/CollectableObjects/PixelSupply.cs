using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class PixelSupply:Supply
    {

        public PixelSupply() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Pixel Mode Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/pixelSupply"));
            SetTexture(GlobalTexture.pixelSupplyTexture, 1, 1, 1, true);
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
        
        public void ChangePlayer(Player player) 
        {
            player.SetActivePixelatedMode(Globals.Random.Next(10,30));
        }

        internal static PixelSupply GetPixelSupply() 
        {
            return new PixelSupply();
        }

    }
}
