using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Laser : PlayerBullet
    {
        public Laser(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Laser", 10, 10, 10);
            SetVelocity();
        }

        public override void Load()
        {
            base.Load();
            SetTextures(GlobalTexture.laserTexture);
            animation = new Animation(texture, 1, 1, 1, true);
        }

        public override void SetTextures(Texture2D newTexture)
        {
            texture = newTexture;
            normalTexture = texture;
            //damageTexture = texture;
            //damageTexture = MakeDamageTexture(newTexture);
            //SetTextureData();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
        }

        public override void Fire()
        {
            base.Fire();
        }

        public override void SetVelocity()
        {
            this.velocity = new Vector2(15, 0);
        }

        public void SetVelocity(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

    }
}
