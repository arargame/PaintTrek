using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class RandomSupply
    {
        public RandomSupply() 
        {
            int r = Globals.Random.Next(0,120);

            if (r>=0 && r<=15) 
            {
                BouncingFireCollection.GetCollactableBouncingBall();
            }
            else if(r>15 && r<=45)
            {
                RocketSupply.GetRocketSupply();
            }
            else if(r>45 && r<=65)
            {
                DiffusedFireSupply.GetDiffusedFireSupply();
            }
            else if(r>65 && r<=85)
            {
                TripleFireSupply.GetTripleFireSupply();
            }
            else if(r>85 && r<=100)
            {
                OrbitalFireSupply.GetOrbitalFireSupply();
            }
            else if(r>100 && r<=120)
            {
                WaveGunSupply.GetWaveGunSupply();
            }

        }

        internal static RandomSupply GetRandomSupply()
        {
            return new RandomSupply();
        }
        
    }
}
