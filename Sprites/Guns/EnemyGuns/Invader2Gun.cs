using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Invader2Gun : EnemyBullet
    {
        public Invader2Gun(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Invader 2 Bullet", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/basicEnemyBullet"));
            SetTextures(GlobalTexture.basicEnemyBulletTexture);
            animation = new Animation(texture, 1, 1, 1, true);
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
                velocity = (-1) * direction * Globals.Random.Next(3, 5);
            }
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }
    }
}
