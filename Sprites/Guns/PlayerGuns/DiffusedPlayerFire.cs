using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class DiffusedPlayerFire : PlayerBullet
    {
        Bullet[] bullets;

        public DiffusedPlayerFire(Sprite owner)
        {
            bullets = new Bullet[8];
            bullets = new Bullet[8];
            // No allocation here - will be pulled from pool on Fire()
            // for (int i = 0; i < bullets.Length; i++) { ... }
            this.owner = owner;
            AssignOwner(owner);
            SetCharacterInfo("Diffused Player Fire", 10, 0, 10);

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load()
        {
            base.Load();
            SetTexture(GlobalTexture.rocketTexture, 1, 5, 5, true);
        }

        public override void Update()
        {
            base.Update();
            Reload();
        }

        public override void Draw()
        {
            base.Draw();
        }
        public void Reload()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                // Only reload if slot is empty (bullet fired/detached) or dead
                if (bullets[i] == null || !bullets[i].alive)
                {
                    bullets[i] = new Laser(owner);
                    // bullets[i].Reset(owner); // Not needed for new instance
                    // visible should be FALSE until fired (Reset does this)
                }
            }
        }

        public override void Fire()
        {
            // Ensure bullets are ready
            Reload();
        
            float fireSpeed = 15f;
            int counter = bullets.Length;
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] == null) continue;

                if (counter == 8)
                {
                    bullets[counter - 1].velocity = new Vector2(-fireSpeed, -fireSpeed);
                }
                else if (counter == 7)
                {
                    bullets[counter - 1].velocity = new Vector2(fireSpeed, -fireSpeed);
                }
                else if (counter == 6)
                {
                    bullets[counter - 1].velocity = new Vector2(-fireSpeed, fireSpeed);
                }
                else if (counter == 5)
                {
                    bullets[counter - 1].velocity = new Vector2(fireSpeed, fireSpeed);
                }
                else if (counter == 4)
                {
                    bullets[counter - 1].velocity = new Vector2(fireSpeed, 0);
                }
                else if (counter == 3)
                {
                    bullets[counter - 1].velocity = new Vector2(-fireSpeed, 0);
                }
                else if (counter == 2)
                {
                    bullets[counter - 1].velocity = new Vector2(0, -fireSpeed);
                }
                else if (counter == 1)
                {
                    bullets[counter - 1].velocity = new Vector2(0, fireSpeed);
                }

                double angle = Math.Atan2(-bullets[counter - 1].velocity.Y, -bullets[counter - 1].velocity.X);
                bullets[counter - 1].rotation = (float)angle;
                bullets[i].Fire();
                
                // Do NOT detach here; the loop logic needs references to cross-configure velocities
                // bullets[i] = null;

                counter--;
            }
            
            // Detach all bullets after firing completes so Reload() gets fresh ones next time
            Array.Clear(bullets, 0, bullets.Length);
        }
        public override void SetVelocity()
        {
            velocity = new Vector2(0, 0);
        }
    }
}
