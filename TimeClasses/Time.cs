using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class Time
    {
        bool isActivated = false;
        TimeSpan timeKeeper;
        int counter;
        int maxSecond;
        bool isChronometer = false;

        public Time()
        {

        }

        public Time(int second)
        {
            maxSecond = second;
            isChronometer = true;
        }

        public void Activate()
        {
            isActivated = true;
            timeKeeper = TotalGameTime();
        }

        public void Stop()
        {
            isActivated = false;
        }

        public void Update()
        {
            if (isActivated)
            {
                if (TotalGameTime() - timeKeeper > TimeSpan.FromSeconds(1.0f))
                {
                    counter++;
                    timeKeeper = TotalGameTime();
                }
                if (isChronometer && counter >= maxSecond)
                {
                    isActivated = false;
                }
            }
        }

        public int GetCounter()
        {
            return counter;
        }

        public static TimeSpan ElapsedGameTime()
        {
            return Globals.GameTime.ElapsedGameTime;
        }

        public static TimeSpan TotalGameTime()
        {
            return Globals.GameTime.TotalGameTime;
        }

    }
}
