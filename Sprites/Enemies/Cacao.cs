using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Cacao : Enemy
    {
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        public Cacao()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Cacao", 10, 10, 10);
            SetVelocity();
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
            SetTexture(GlobalTexture.cacaoTexture, 5, 1, 5, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 5))
            {
                fireTime = 0;
                var bullet = BulletPool.Get<BasicEnemyBullet>(this);
                // Bullet is auto-fired by Get->Reset logic, but we can ensure it
                // bullet.Fire(); // Already called/setup in Reset effectively
            }
        }

        public override void Draw()
        {
          //  bullet.Draw();
            base.Draw();
        }

        public override void MakeVisible()
        {
            base.MakeVisible();
        }

        public override void Kill()
        {
            base.Kill();
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }
        public override void SetVelocity()
        {
            this.velocity = new Vector2((-1) * Globals.Random.Next(3, 6), 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static Cacao GetCacaos()
        {
            return new Cacao();
        }
    }
}
