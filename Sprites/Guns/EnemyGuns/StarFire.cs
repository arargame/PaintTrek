using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class StarFire:EnemyBullet
    {
        public StarFire(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Star Fire", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/starFireSpriteSheet"));
            SetTexture(GlobalTexture.starFireTexture, 4, 2, 16, true);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }
    }
}
