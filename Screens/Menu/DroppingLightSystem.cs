using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class DroppingLightSystem
    {
        Light[] lights;
        public DroppingLightSystem()
        {
            lights = new Light[15];

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = new Light();
            }
        }
        ~DroppingLightSystem() 
        {
        }

        public void Update()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].Update();
            }
        }

        public void Draw()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].Draw();
            }
        }
    }
}
