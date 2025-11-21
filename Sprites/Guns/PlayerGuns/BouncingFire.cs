using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BouncingFire : PlayerBullet
    {

        public BouncingFire(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Bouncing Ball", 500, 20, 10);
        }

        public override void Load()
        {
            base.Load();
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/bouncingFireSpriteSheet"));
            SetTextures(GlobalTexture.bouncingFireTexture);
            animation = new Animation(texture, 4, 2, 8, true);
        }

        public override void Update()
        {
            base.Update();


            if (position.X <= 0)
            {
                velocity.X *= -1;
                velocity.Y *= 1;
            }

            if (position.X + animation.Width >= Globals.GameSize.X)
            {
                velocity.X *= -1;
                velocity.Y *= 1;
            }

            if (position.Y <= 0)
            {
                velocity.X *= 1;
                velocity.Y *= -1;
            }

            if (position.Y + animation.Height >= Globals.GameSize.Y)
            {
                velocity.X *= 1;
                velocity.Y *= -1;
            }

        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            if (owner != null)
            {
                if (owner.position.Y > Globals.GameSize.Y / 2)
                    velocity = new Vector2(Globals.Random.Next(2, 5), Globals.Random.Next(5, 10) * (-1));
                else
                    velocity = new Vector2(Globals.Random.Next(2, 5), Globals.Random.Next(5, 10));
            }
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
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
