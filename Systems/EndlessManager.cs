using System;
using System.Collections.Generic;
using System.Linq;

namespace PaintTrek
{
    /// <summary>
    /// Desktop wave-mode controller.  It deliberately contains no store, billing or Google
    /// dependency: Microsoft Store/Steam builds expose every mode through PlatformServices.
    /// </summary>
    public sealed class EndlessManager
    {
        private static readonly EndlessManager instance = new EndlessManager();
        public static EndlessManager Instance => instance;

        public const int EnemiesPerWave = 100;
        public int WaveNumber { get; private set; } = 1;
        public int EnemiesSpawnedInWave { get; private set; }
        public int EnemiesKilledInWave { get; private set; }
        public TimeSpan SurvivalTime { get; private set; }
        public double WaveTextTimer { get; private set; }

        private readonly Random random = new Random();
        private readonly List<int> bossOrder = new List<int>();
        private double spawnTimer;
        private double supplyTimer;
        private bool bossSpawned;
        private bool bossKilled;
        private int bossOrderIndex;

        public float DifficultyScaling => Globals.CurrentMode == GameMode.UfoInvasion
            ? 1f + (WaveNumber - 1) * .05f
            : 1f + (WaveNumber - 1) * .02f;

        public int CurrentBossRushBossId => ((WaveNumber - 1) % 10) + 1;
        public float BossRushHealthMultiplier => 1f + (((WaveNumber - 1) / 10) * .10f);

        public void Reset()
        {
            WaveNumber = 1;
            EnemiesSpawnedInWave = 0;
            EnemiesKilledInWave = 0;
            SurvivalTime = TimeSpan.Zero;
            WaveTextTimer = 4d;
            spawnTimer = supplyTimer = 0d;
            bossSpawned = bossKilled = false;
            bossOrderIndex = 0;
            ShuffleBosses();
        }

        public void Update()
        {
            if (Level.isPaused) return;

            double elapsed = Globals.GameTime.ElapsedGameTime.TotalSeconds;
            SurvivalTime += Globals.GameTime.ElapsedGameTime;
            WaveTextTimer = Math.Max(0d, WaveTextTimer - elapsed);
            supplyTimer += elapsed;
            if (supplyTimer >= 15d)
            {
                SpawnSupply();
                supplyTimer = 0d;
            }

            if (Globals.CurrentMode == GameMode.AgainstAllBosses)
            {
                if (!bossSpawned) { SpawnBoss(CurrentBossRushBossId); bossSpawned = true; }
                else if (bossKilled) NextWave();
                return;
            }

            if (EnemiesSpawnedInWave >= EnemiesPerWave)
            {
                if (!bossSpawned)
                {
                    SpawnBoss(Globals.CurrentMode == GameMode.UfoInvasion ? 7 : NextRandomBoss());
                    bossSpawned = true;
                }
                else if (bossKilled) NextWave();
                else return;
            }

            spawnTimer += elapsed;
            if (spawnTimer >= Math.Max(.5d, 2d - WaveNumber * .05d))
            {
                SpawnEnemy();
                EnemiesSpawnedInWave++;
                spawnTimer = 0d;
            }
        }

        public void NotifyEnemyKilled() => EnemiesKilledInWave = Math.Min(EnemiesPerWave, EnemiesKilledInWave + 1);
        public void NotifyBossKilled() => bossKilled = true;

        private void NextWave()
        {
            WaveNumber++;
            EnemiesSpawnedInWave = EnemiesKilledInWave = 0;
            bossSpawned = bossKilled = false;
            WaveTextTimer = 4d;
        }

        private void SpawnEnemy()
        {
            if (Globals.CurrentMode == GameMode.UfoInvasion)
            {
                switch (random.Next(5))
                {
                    case 0: Invader1.GetInvader1(); break;
                    case 1: Invader2.GetInvader2(); break;
                    case 2: Invader3.GetInvader3(); break;
                    case 3: Ufo.GetUfo(); break;
                    default: Ufo2.GetUfo2(); break;
                }
                return;
            }

            switch (random.Next(16))
            {
                case 0: Eye.GetEyes(); break; case 1: Cacao.GetCacaos(); break;
                case 2: MonsterFish.GetMonsterFish(); break; case 3: Bristle.GetBristle(); break;
                case 4: Invader1.GetInvader1(); break; case 5: Invader2.GetInvader2(); break;
                case 6: Invader3.GetInvader3(); break; case 7: Comet.GetComet(); break;
                case 8: Asteroid.GetAstreoid(); break; case 9: Bubble.GetBubble(); break;
                case 10: JellyFish.GetSquid(); break; case 11: SharpCube.GetSharpCube(); break;
                case 12: SpaceSnake.GetSpaceSnake(); break; case 13: Ufo.GetUfo(); break;
                case 14: ChildTrilobit.GetChildTrilobit(); break; default: MRBrain.GetMRBrain(); break;
            }
        }

        private void SpawnSupply()
        {
            switch (random.Next(9))
            {
                case 0: Wrench.GetWrench(); break; case 1: RocketSupply.GetRocketSupply(); break;
                case 2: PixelSupply.GetPixelSupply(); break; case 3: BouncingFireCollection.GetCollactableBouncingBall(); break;
                case 4: TripleFireSupply.GetTripleFireSupply(); break; case 5: DiffusedFireSupply.GetDiffusedFireSupply(); break;
                case 6: OrbitalFireSupply.GetOrbitalFireSupply(); break; case 7: WaveGunSupply.GetWaveGunSupply(); break;
                default: RandomSupply.GetRandomSupply(); break;
            }
        }

        private void ShuffleBosses()
        {
            bossOrder.Clear();
            bossOrder.AddRange(Enumerable.Range(1, 10).OrderBy(_ => random.Next()));
        }

        private int NextRandomBoss()
        {
            if (bossOrderIndex >= bossOrder.Count) { ShuffleBosses(); bossOrderIndex = 0; }
            return bossOrder[bossOrderIndex++];
        }

        private void SpawnBoss(int id)
        {
            switch (id)
            {
                case 1: Boss1.GetBoss1(); break; case 2: Boss2.GetBoss2(); break;
                case 3: Boss3.GetBoss3(); break; case 4: Boss4.GetBoss4(); break;
                case 5: Boss5.GetBoss5(); break; case 6: Boss6.GetBoss6(); break;
                case 7: Boss7.GetBoss7(); break; case 8: Boss8.GetBoss8(); break;
                case 9: Boss9.GetBoss9(); break; default: Boss10.GetBoss10(); break;
            }
        }
    }
}
