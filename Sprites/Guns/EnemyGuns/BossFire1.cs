using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BossFire1 : EnemyBullet
    {
        public BossFire1(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss Fire 1", 50, 20, 20);
            scale = Globals.Random.Next(1, 2);
        }

        public override void Load()
        {
            SetTextures(GlobalTexture.bossFire1Texture);
            animation = new Animation(texture, 4, 1, 4, true);
        }


        public override void Update()
        {
            base.Update();
            Rotate(5f);
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
            velocity = new Vector2((-1) * Globals.Random.Next(7, 10), Globals.Random.Next(-2, 2));
            if (Globals.Graphics.IsFullScreen)
            {
                velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
            }
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }
    }
}
