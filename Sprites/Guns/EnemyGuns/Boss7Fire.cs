using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss7Fire : EnemyBullet
    {
        public Boss7Fire(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 7 Bullet", 10, 10, 10);
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

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            if (owner != null)
            {
                velocity = new Vector2(Globals.Random.Next(-3, 3), Globals.Random.Next(6, 10));
            }
        }

        public override void Fire()
        {
            base.Fire();
        }
    }
}
