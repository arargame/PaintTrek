using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss10Fire : EnemyBullet
    {
        public Boss10Fire(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 10 Fire", 50, 25, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/boss10Fire"));
            SetTexture(GlobalTexture.boss10FireTexture, 1, 2, 2, true);
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
                velocity = (-1) * direction * Globals.Random.Next(7, 10);
                double angle = Math.Atan2(velocity.Y, velocity.X);
                rotation = (float)angle;
            }
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }
    }
}
