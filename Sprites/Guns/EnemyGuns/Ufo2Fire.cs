using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Ufo2Fire:EnemyBullet
    {
        public Ufo2Fire(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Ufo2 Fire", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/ufo2Fire"));
            SetTexture(GlobalTexture.ufo2FireTexture, 1, 1, 1, true);
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
            velocity = new Vector2(-10,0);
        }

        public override void Fire()
        {
            base.Fire();
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }


    }
}
