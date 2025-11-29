using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class UfoLaser:EnemyBullet
    {
        public UfoLaser(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Ufo Laser", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/ufoLaser"));
            SetTexture(GlobalTexture.ufoLaserTexture, 1, 2, 2, true);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
        public override void SetVelocity()
        {
            if (owner != null && targetPosition != Vector2.Zero)
            {
                Vector2 direction = Vector2.Normalize(owner.position - targetPosition);
                velocity = (-1) * direction * Globals.Random.Next(5, 7);
                double angle=Math.Atan2(velocity.Y,velocity.X);
                rotation = (float)angle;
            }
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

        public override void Fire()
        {
            base.Fire();
        }
    }
}
