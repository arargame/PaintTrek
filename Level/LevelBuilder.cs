using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    #region ItemType enum
    enum ItemType
    {
        Boss1,
        Boss2,
        Boss3,
        Boss4,
        Boss5,
        Boss6,
        Boss7,
        Boss8,
        Boss9,
        Boss10,
        Cacao,
        Eye,
        MrBrain,
        ChildTrilobit,
        Squid,
        Time,
        Ufo1,
        Ufo2,
        Comet,
        Bone,
        Invader1,
        Invader2,
        Invader3,
        Astreoid,
        EnemyAutomate,
        CollectableObjectAutomate,
        SharpCube,
        SpaceSnake,
        SnakeStone,
        HeartBreaker,
        Bristle,
        MonsterFish,
        Bubble,
        Diamond,
        Wrench,
        RocketSupply,
        PixelSupply,
        BouncingFireCollection,
        DiffusedFireSupply,
        OrbitalFireSupply,
        TripleFireSupply,
        WaveGunSupply,
        RandomSupply
    }
    #endregion

    #region LevelItem struct
    struct LevelItem
    {
        public double Time;
        public ItemType Type;
    }
    #endregion

    class LevelBuilder
    {
        EnemyAutomate enemyAutomate;
        CollectableObjectAutomate collectableObjectAutomate;
        List<LevelItem> levelItems;
        int timeCursor = 0;
        int itemIndex = 0;

        double timer;

        public LevelBuilder()
        {
            levelItems = new List<LevelItem>();
            BuildLevel();
            enemyAutomate = new EnemyAutomate();
            collectableObjectAutomate = new CollectableObjectAutomate();
        }

        public void Update()
        {
            timer += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            Globals.XX = timer;
 

            for (int i = itemIndex; i < levelItems.Count; i++)
            {
                if (levelItems[i].Time > timer)
                    break;
                SpawnItem(levelItems[i]);
                itemIndex++;
            }

            enemyAutomate.Update();
            collectableObjectAutomate.Update();
        }

        public void AddItem(ItemType type)
        {
            LevelItem item = new LevelItem();
            item.Type = type;
            item.Time = timeCursor;
            levelItems.Add(item);
        }

        private void SpawnItem(LevelItem levelItem)
        {
            if (levelItem.Type == ItemType.Eye)
            {
                Eye.GetEyes();
            }
            else if (levelItem.Type == ItemType.Cacao)
            {
                Cacao.GetCacaos();
            }
            else if (levelItem.Type == ItemType.Boss1)
            {
                Boss1.GetBoss1();
            }
            else if (levelItem.Type == ItemType.Boss2)
            {
                Boss2.GetBoss2();
            }
            else if (levelItem.Type == ItemType.Boss3)
            {
                Boss3.GetBoss3();
            }
            else if (levelItem.Type == ItemType.Boss4)
            {
                Boss4.GetBoss4();
            }
            else if (levelItem.Type == ItemType.Boss5)
            {
                Boss5.GetBoss5();
            }
            else if (levelItem.Type == ItemType.Boss6)
            {
                Boss6.GetBoss6();
            }
            else if (levelItem.Type == ItemType.Boss7)
            {
                Boss7.GetBoss7();
            }
            else if (levelItem.Type == ItemType.Boss8)
            {
                Boss8.GetBoss8();
            }
            else if (levelItem.Type == ItemType.Boss9)
            {
                Boss9.GetBoss9();
            }
            else if (levelItem.Type == ItemType.Boss10)
            {
                Boss10.GetBoss10();
            }
            else if (levelItem.Type == ItemType.MrBrain)
            {
                MRBrain.GetMRBrain();
            }
            else if (levelItem.Type == ItemType.MonsterFish)
            {
                MonsterFish.GetMonsterFish();
            }
            else if (levelItem.Type == ItemType.Diamond)
            {
                Diamond.GetDiamond();
            }
            else if (levelItem.Type == ItemType.Wrench)
            {
                Wrench.GetWrench();
            }
            else if (levelItem.Type == ItemType.RocketSupply)
            {
                RocketSupply.GetRocketSupply();
            }
            else if (levelItem.Type == ItemType.PixelSupply)
            {
                PixelSupply.GetPixelSupply();
            }
            else if(levelItem.Type==ItemType.WaveGunSupply)
            {
                WaveGunSupply.GetWaveGunSupply();
            }
            else if (levelItem.Type == ItemType.Bone)
            {
                Bone.GetBone();
            }
            else if (levelItem.Type == ItemType.Bristle)
            {
                Bristle.GetBristle();
            }
            else if (levelItem.Type == ItemType.Invader1)
            {
                Invader1.GetInvader1();
            }
            else if (levelItem.Type == ItemType.Invader2)
            {
                Invader2.GetInvader2();
            }
            else if (levelItem.Type == ItemType.Invader3)
            {
                Invader3.GetInvader3();
            }
            else if (levelItem.Type == ItemType.Comet)
            {
                Comet.GetComet();
            }
            else if (levelItem.Type == ItemType.BouncingFireCollection)
            {
                BouncingFireCollection.GetCollactableBouncingBall();
            }
            else if (levelItem.Type == ItemType.Astreoid)
            {
                Asteroid.GetAstreoid();
            }
            else if (levelItem.Type == ItemType.Bubble)
            {
                Bubble.GetBubble();
            }
            else if (levelItem.Type == ItemType.EnemyAutomate)
            {
                EnemyAutomate.Start();
            }
            else if (levelItem.Type == ItemType.CollectableObjectAutomate)
            {
                CollectableObjectAutomate.Start();
            }
            else if (levelItem.Type == ItemType.Squid)
            {
                JellyFish.GetSquid();
            }
            else if (levelItem.Type == ItemType.SharpCube)
            {
                SharpCube.GetSharpCube();
            }
            else if (levelItem.Type == ItemType.SpaceSnake)
            {
                SpaceSnake.GetSpaceSnake();
            }
            else if (levelItem.Type == ItemType.SnakeStone)
            {
                SnakeStone.GetSnakeStone();
            }
            else if (levelItem.Type == ItemType.RandomSupply)
            {
                RandomSupply.GetRandomSupply();
            }
            else if (levelItem.Type == ItemType.DiffusedFireSupply)
            {
                DiffusedFireSupply.GetDiffusedFireSupply();
            }
            else if (levelItem.Type == ItemType.TripleFireSupply)
            {
                TripleFireSupply.GetTripleFireSupply();
            }
            else if (levelItem.Type == ItemType.OrbitalFireSupply)
            {
                OrbitalFireSupply.GetOrbitalFireSupply();
            }
            else if (levelItem.Type == ItemType.Ufo1)
            {
                Ufo.GetUfo();
            }

            else if (levelItem.Type == ItemType.Ufo2)
            {
                Ufo2.GetUfo2();
            }

            else if (levelItem.Type == ItemType.ChildTrilobit)
            {
                ChildTrilobit.GetChildTrilobit();
            }


        }

        private void BuildLevel()
        {
             //===== GEÇİCİ TEST MODU -BOSS1 TESTİ =====
             //Boss1'i hızlıca test etmek için
            //if (Level.LevelCounter == 1)
            //{
            //    Console.WriteLine($"[LevelBuilder] TEST MODE: Building test level 1 with Boss1");

            //    // Boss1'i hemen spawn et
            //    timeCursor = 2; // 2 saniye sonra
            //    AddItem(ItemType.Boss1);

            //    Console.WriteLine($"[LevelBuilder] Test level built with {levelItems.Count} items");
            //    return; // Dosyadan okuma yapma
            //}
             //===== GEÇİCİ TEST MODU SONU =====

            string path = "Content/Levels/level" + Level.LevelCounter + ".txt";
            Console.WriteLine($"[LevelBuilder] Building level {Level.LevelCounter}");
            
            FileSystem fs = new FileSystem(path);

            fs.ReadLevel();
            
            Console.WriteLine($"[LevelBuilder] FileSystem loaded {fs.Amount.Count} items");

            for (int i = 0; i < fs.Amount.Count; i++)
            {
                if (fs.Type[i] == "Wait")
                {
                    timeCursor += fs.Amount[i];
                }
                if (fs.Type[i] == "Diamond")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Diamond);
                    }
                }
                if (fs.Type[i] == "Cacao")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Cacao);
                    }
                }
                if (fs.Type[i] == "Eye")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Eye);
                    }
                }
                if (fs.Type[i] == "Bone")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Bone);
                    }
                }
                if (fs.Type[i] == "Comet")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Comet);
                    }
                }
                if (fs.Type[i] == "Ufo1")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Ufo1);
                    }
                }
                if (fs.Type[i] == "Ufo2")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Ufo2);
                    }
                }

                if (fs.Type[i] == "Invader1")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Invader1);
                    }
                }

                if (fs.Type[i] == "Invader2")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Invader2);
                    }
                }

                if (fs.Type[i] == "Invader3")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Invader3);
                    }
                }
                if (fs.Type[i] == "MonsterFish")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.MonsterFish);
                    }
                }
                if (fs.Type[i] == "ChildTrilobit")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.ChildTrilobit);
                    }
                }
                if (fs.Type[i] == "Boss1")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss1);
                    }
                }
                if (fs.Type[i] == "Boss2")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss2);
                    }
                }
                if (fs.Type[i] == "Boss3")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss3);
                    }
                }
                if (fs.Type[i] == "Boss4")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss4);
                    }
                }
                if (fs.Type[i] == "Boss5")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss5);
                    }
                }
                if (fs.Type[i] == "Boss6")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss6);
                    }
                }
                if (fs.Type[i] == "Boss7")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss7);
                    }
                }

                if (fs.Type[i] == "Boss8")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss8);
                    }
                }

                if (fs.Type[i] == "Boss9")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss9);
                    }
                }
                if (fs.Type[i] == "Boss10")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Boss10);
                    }
                }
                if (fs.Type[i] == "MrBrain")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.MrBrain);
                    }
                }
                if (fs.Type[i] == "SnakeStone")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.SnakeStone);
                    }
                }
                if (fs.Type[i] == "Wrench")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Wrench);
                    }
                }
                if (fs.Type[i] == "RocketSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.RocketSupply);
                    }
                }
                if (fs.Type[i] == "PixelSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.PixelSupply);
                    }
                }
                if (fs.Type[i] == "BouncingFireCollection")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.BouncingFireCollection);
                    }
                }
                if (fs.Type[i] == "TripleFireSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.TripleFireSupply);
                    }
                }
                if (fs.Type[i] == "DiffusedFireSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.DiffusedFireSupply);
                    }
                }

                if (fs.Type[i] == "OrbitalFireSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.OrbitalFireSupply);
                    }
                }
                if (fs.Type[i] == "WaveGunSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.WaveGunSupply);
                    }
                }

                if (fs.Type[i] == "RandomSupply")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.RandomSupply);
                    }
                }
                if (fs.Type[i] == "Bristle")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Bristle);
                    }
                }

                if (fs.Type[i] == "Astreoid")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Astreoid);
                    }
                }

                if (fs.Type[i] == "CollectableObjectAutomate")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.CollectableObjectAutomate);
                    }
                }

                if (fs.Type[i] == "Bubble")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Bubble);
                    }
                }

                if (fs.Type[i] == "EnemyAutomate")
                {
                    AddItem(ItemType.EnemyAutomate);
                }

                if (fs.Type[i] == "Squid")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.Squid);
                    }
                }
                if (fs.Type[i] == "SharpCube")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.SharpCube);
                    }
                }

                if (fs.Type[i] == "SpaceSnake")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.SpaceSnake);
                    }
                }
                if (fs.Type[i] == "HeartBreaker")
                {
                    for (int k = 0; k < fs.Amount[i]; k++)
                    {
                        AddItem(ItemType.HeartBreaker);
                    }
                }

            }
        }

    }
}
