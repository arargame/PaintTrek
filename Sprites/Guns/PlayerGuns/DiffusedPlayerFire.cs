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
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new Laser(owner);
            }
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
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/rocketSpriteSheet"));
            SetTextures(GlobalTexture.rocketTexture);
            animation = new Animation(texture, 1, 5, 5, true);
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
                if (bullets[i] == null || !bullets[i].alive || owner != null)
                {
                    bullets[i] = new Laser(owner);
                }
            }
        }

        public override void Fire()
        {
            float fireSpeed = 15f;
            int counter = bullets.Length;
            for (int i = 0; i < bullets.Length; i++)
            {

                if (bullets[i] == null)
                    Reload();
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

                counter--;
            }
        }
        public override void SetVelocity()
        {
            velocity = new Vector2(0, 0);
        }
    }
}
