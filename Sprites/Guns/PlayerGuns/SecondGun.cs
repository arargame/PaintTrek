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

        public SecondGun(Player owner ) 
        {
            isActive = false;
            this.owner = owner;
        }

        public void AddGun(PlayerBullet newGun,int bullet) 
        {
            this.gun = newGun;
            this.bullet = bullet;
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
            if (gun is Rocket)
            {
                gun = new Rocket(owner);
            }
            else if(gun is BouncingFire)
            {
                gun = new BouncingFire(owner);
            }
            else if(gun is DiffusedPlayerFire)
            {
                gun = new DiffusedPlayerFire(owner);
            }
            else if (gun is OrbitalFire)
            {
                gun = new OrbitalFire(owner);
            }
            else if(gun is TripleFire)
            {
                gun = new TripleFire(owner);
            }
            else if(gun is WaveGun)
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
