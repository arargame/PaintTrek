using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class DiffusedLightSystem
    {
        int particleCount;
        Sprite owner;
        List <Bullet> bullets;
        string bulletType;
        int fireSpeed;

        public DiffusedLightSystem(Sprite owner, int particleCount, string bulletType, int fireSpeed)
        {
            this.owner = owner;
            this.particleCount = particleCount;
            bullets =new List<Bullet>();
            this.bulletType = bulletType;
            Load(bulletType);
            this.fireSpeed = fireSpeed;
        }
        private void Load(string bulletType)
        {
            bullets.Clear();
            if (bulletType == "Ufo2Fire")
            {
                for (int i = 0; i < particleCount; i++)
                {
                    bullets.Add(new Ufo2Fire(owner));
                }
            }
            else if (bulletType == "BasicEnemyBullet")
            {
                for (int i = 0; i < particleCount; i++)
                {
                    bullets.Add(new BasicEnemyBullet(owner));
                }
            }
            else if (bulletType == "StarFire")
            {
                for (int i = 0; i < particleCount; i++)
                {
                    bullets.Add(new StarFire(owner));
                }
            }
        }

        public void Reload()
        {
            bullets.Clear();
            for (int i = 0; i < particleCount; i++)
            {
                    if (bulletType == "Ufo2Fire")
                    {
                        bullets.Add(new Ufo2Fire(owner));
                    }
                    else if (bulletType == "BasicEnemyBullet")
                    {
                        bullets.Add(new BasicEnemyBullet(owner));
                    }
                    else if (bulletType == "StarFire")
                    {
                        bullets.Add(new StarFire(owner));
                    }
            }
        }

        public void Fire()
        {
            int counter = particleCount;
            for (int i = 0; i < particleCount; i++)
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

                bullets[i].Fire();
                counter--;
            }
        }
    }
}
