using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    abstract class Supply:CollectableObject
    {
        bool isIncreasing;
        double maxScale;
        float R;

        public override void Initialize()
        {
            base.Initialize();
            isIncreasing = false;
            maxScale = 1.5f;
            scale = Globals.Random.Next(1, 2);
            R = (float)(Globals.Random.Next(5, 8) / 10f);
        }

        public override void Update()
        {
            base.Update();

            if (isIncreasing)
            {
                scale += 0.008f;
                if (scale > maxScale)
                    isIncreasing = false;
            }
            else
            {
                scale -= 0.008f;
                if (scale <= 0.5f)
                    isIncreasing = true;
            }
            Rotate(R);
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-Globals.Random.Next(1,4),0);
        }
    }
}
