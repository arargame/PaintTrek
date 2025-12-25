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
        String bulletType;
        int fireSpeed;

        public DiffusedLightSystem(Sprite owner, int particleCount, string bulletType, int fireSpeed)
        {
            this.owner = owner;
            this.particleCount = particleCount;
            this.bulletType = bulletType;
            this.fireSpeed = fireSpeed;
            // Load removed - bullets are now retrieved from pool on Fire()
        }
        
        // Reload removed - unnecessary with pooling

        public void Fire()
        {
            int counter = particleCount;
            
            // PERFORMANS FIX: Use BulletPool instead of array allocation
            // Creates 8 bullets in different directions
            
            for (int i = 0; i < particleCount; i++)
            {
                Bullet bullet = null;

                if (bulletType == "Ufo2Fire")
                {
                    bullet = BulletPool.Get<Ufo2Fire>(owner);
                }
                else if (bulletType == "BasicEnemyBullet")
                {
                    bullet = BulletPool.Get<BasicEnemyBullet>(owner);
                }
                else if (bulletType == "StarFire")
                {
                    bullet = BulletPool.Get<StarFire>(owner);
                }
                
                if (bullet != null)
                {
                    // Set direction based on counter
                    if (counter == 8) bullet.velocity = new Vector2(-fireSpeed, -fireSpeed);
                    else if (counter == 7) bullet.velocity = new Vector2(fireSpeed, -fireSpeed);
                    else if (counter == 6) bullet.velocity = new Vector2(-fireSpeed, fireSpeed);
                    else if (counter == 5) bullet.velocity = new Vector2(fireSpeed, fireSpeed);
                    else if (counter == 4) bullet.velocity = new Vector2(fireSpeed, 0);
                    else if (counter == 3) bullet.velocity = new Vector2(-fireSpeed, 0);
                    else if (counter == 2) bullet.velocity = new Vector2(0, -fireSpeed);
                    else if (counter == 1) bullet.velocity = new Vector2(0, fireSpeed);

                    bullet.Fire();
                }
                
                counter--;
            }
        }
    }
}
