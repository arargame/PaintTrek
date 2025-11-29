using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class WaveGunSupply:Supply
    {
          public WaveGunSupply() 
        {
            Initialize();
            
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Wave Gun Supply", 10, 0, 10);
            SetVelocity();
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/rocketSupply"));
            SetTexture(GlobalTexture.waveGunSupplyTexture, 1, 1, 1, true);
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
        internal static WaveGunSupply GetWaveGunSupply() 
        {
            return new WaveGunSupply();
        }
    }
}
