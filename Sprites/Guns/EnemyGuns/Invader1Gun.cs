using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Invader1Gun : EnemyBullet
    {
        public Invader1Gun(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Invader 1 Bullet", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/basicEnemyBullet"));
            SetTexture(GlobalTexture.basicEnemyBulletTexture, 1, 1, 1, true);
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
            if (owner != null)
            {
                velocity = new Vector2(0, Globals.Random.Next(4, 8));
            }
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }
    }
}
