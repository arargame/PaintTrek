using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class SecondGun
    {
        PlayerBullet gun;
        int bullet;
        bool isActive;
        Player owner;

        // PERFORMANS: Gun type'ı cache'le
        private Type gunType;

        public SecondGun(Player owner ) 
        {
            isActive = false;
            this.owner = owner;
        }

        public void AddGun(PlayerBullet newGun,int bullet) 
        {
            this.gun = newGun;
            this.bullet = bullet;
            this.gunType = newGun.GetType(); // Type'ı cache'le
            isActive = true;
        }

        public void Update() 
        {
            if (bullet <= 0) 
                isActive = false;
        }

        public void Reload() 
        {
            bullet--;
            
            // PERFORMANS: BulletPool kullanımı
            if (gunType == typeof(Rocket))
            {
                gun = new Rocket(owner);
            }
            else if (gunType == typeof(BouncingFire))
            {
                gun = new BouncingFire(owner);
            }
            else if (gunType == typeof(DiffusedPlayerFire))
            {
                gun = new DiffusedPlayerFire(owner);
            }
            else if (gunType == typeof(OrbitalFire))
            {
                gun = new OrbitalFire(owner);
            }
            else if (gunType == typeof(TripleFire))
            {
                gun = new TripleFire(owner);
            }
            else if (gunType == typeof(WaveGun))
            {
                gun = new WaveGun(owner);
            }
        }

        public void Fire() 
        {
            Reload();
            if (isActive && bullet > 0) 
            {
                gun.Fire();
            }
        }
    }
}
