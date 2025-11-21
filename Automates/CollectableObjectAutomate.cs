using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class CollectableObjectAutomate
    {
        public static bool start;
        double time;
        double limit;

        public CollectableObjectAutomate()
        {
            time = 0;
            start = false;
            limit=Globals.Random.Next(40,60);
        }

        public void Update()
        {
            time += Globals.GameTime.ElapsedGameTime.TotalSeconds;


            if ((int)time > (int)limit && start)
            {
                int x = Globals.Random.Next(0, 100);
                if (x <= 10)
                {
                    Diamond.GetDiamond();
                }
                else if (x >= 10 && x < 65)
                {
                    if (x % 15 == 0)
                        OrbitalFireSupply.GetOrbitalFireSupply();
                    else if (x % 7 == 0)
                        WaveGunSupply.GetWaveGunSupply();
                    else if (x % 4 == 0)
                        RocketSupply.GetRocketSupply();
                    else if (x % 3 == 0)
                        BouncingFireCollection.GetCollactableBouncingBall();
                    else if (x % 2 == 0 || x % 2 == 1)
                        DiffusedFireSupply.GetDiffusedFireSupply();
                }
                else if (x >= 65 && x <= 100)
                {
                    if (x % 2 == 0)
                    {
                        Bubble.GetBubble();
                    }
                    else if (x % 2 == 1)
                    {
                        Wrench.GetWrench();
                    }
                }

                time = 0;
                limit = Globals.Random.Next(40, 60);
            }

            if (BossSystem.bossHasFallen)
                CollectableObjectAutomate.Stop();
        }
        public static void Start()
        {
            start = true;
        }

        public static void Stop()
        {
            start = false;
        }

        public static CollectableObjectAutomate GetCollectableObjectAutomate()
        {
            return new CollectableObjectAutomate();
        } 
    }
}
