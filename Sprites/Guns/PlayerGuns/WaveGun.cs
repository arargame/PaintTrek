using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class WaveGun:PlayerBullet
    {

        public WaveGun(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
            scale=0.2f;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Wave Gun", 10, 20, 10);
            SetVelocity();
        }

        public override void Load()
        {
            base.Load();
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/rocketSpriteSheet"));
            SetTextures(GlobalTexture.waveGunTexture);
            animation = new Animation(texture, 3, 1, 3, true);
        }

        public override void Update()
        {
            base.Update();
            scale += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Fire()
        {
            base.Fire();
        }
        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
        }

        public override void SetVelocity()
        {
            this.velocity = new Vector2(10, 0);
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

    }
}
