using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    class EnemyAutomate
    {
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        public static bool start;

        double timeforSomeObject;

        public EnemyAutomate()
        {
            previousSpawnTime = TimeSpan.Zero;

            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            start = false;

            timeforSomeObject = 0;
        }

        public void Update()
        {
            timeforSomeObject += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (Globals.GameTime.TotalGameTime - previousSpawnTime > enemySpawnTime && start)
            {
                previousSpawnTime = Globals.GameTime.TotalGameTime;

                if (Level.LevelCounter == 1)
                {
                    Cacao.GetCacaos();
                }
                else if (Level.LevelCounter == 2)
                {
                    Eye.GetEyes();

                    
                    Eye.GetEyes();
                }
                else if (Level.LevelCounter == 3)
                {
                    Bone.GetBone();
                    Bone.GetBone();
                }
                else if (Level.LevelCounter == 4)
                {
                    SpaceSnake.GetSpaceSnake();
                }
                else if (Level.LevelCounter == 5)
                {
                    if ((int)timeforSomeObject > 5)
                    {
                        Bristle.GetBristle();
                        timeforSomeObject = 0;
                    }

                    if ((int)timeforSomeObject > 3)
                    {
                        Cacao.GetCacaos();
                    }
                }
                else if (Level.LevelCounter == 6)
                {
                    if ((int)timeforSomeObject % 30 == 0)
                    {
                        Wrench.GetWrench();
                    }
                    else if ((int)timeforSomeObject % 20 == 0)
                    {
                        Diamond.GetDiamond();
                    }
                    else if ((int)timeforSomeObject % 3 == 0)
                    {
                        Ufo2.GetUfo2();
                    }
                }
                else if (Level.LevelCounter == 7)
                {
                    if((int)timeforSomeObject%2==0)
                    Invader2.GetInvader2();
                }
                else if (Level.LevelCounter == 8)
                {
                    if ((int)timeforSomeObject > 20)
                    {
                        Ufo.GetUfo();
                        timeforSomeObject = 0;
                        Diamond.GetDiamond();
                    }
                }
                else if (Level.LevelCounter == 9)
                {
                    if ((int)timeforSomeObject % 30 == 0)
                        Diamond.GetDiamond();
                    else if ((int)timeforSomeObject % 2 == 1)
                        SharpCube.GetSharpCube();
                    else if ((int)timeforSomeObject % 2 == 0)
                        Ufo2.GetUfo2();
                }
                else if (Level.LevelCounter == 10)
                {
                    if ((int)timeforSomeObject % 30 == 0)
                        Diamond.GetDiamond();
                    else if ((int)timeforSomeObject % 20 == 0)
                        Invader1.GetInvader1();
                    else if ((int)timeforSomeObject % 10 == 0)
                        Invader3.GetInvader3();
                    else if ((int)timeforSomeObject % 3 == 1)
                        Invader2.GetInvader2();
                }
            }


            if (BossSystem.bossHasFallen)
                EnemyAutomate.Stop();
        }
        public static void Start()
        {
            start = true;
        }
        public static void Stop()
        {
            start = false;
        }

        public static EnemyAutomate GetEnemyAutomate()
        {
            return new EnemyAutomate();
        }
    }
}
