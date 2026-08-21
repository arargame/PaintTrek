using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaintTrek.Shared.Statistics;

namespace PaintTrek
{
    #region GameState enum
    public enum GameState
    {
        Scenario,
        Loading,
        Active,
        GameOver
    }
    #endregion 

    class Level
    {
        LevelBuilder levelBuilder;
        LoadingScene loadingScene;
        ScenarioScene scenarioScene;
        BackgroundSystem bgSystem;
        LevelSoundtrack levelSoundtrack;

        HeadUpDisplay HUD;
        ExitDoor exitDoor;

        GameState gameState;

        SpriteSystem spriteSystem;
        EnemySystem enemySystem;
        BossSystem bossSystem;
        GunSystem gunSystem;
        ExplosionSystem explosionSystem;

        public static int LevelCounter;
        public static int Score;

        public static bool isPaused;
        bool reachedExit;

        Player player;

        bool gameOver;

        double timeToExit;
        bool canExit;
        // public static SoundSystem exitLevelSound = new SoundSystem("Sounds/SoundEffects/exitReached", false);

        public bool ReachedExit
        {
            get { return reachedExit; }
            set { reachedExit = value; }
        }

        public Level() 
        {
            Initialize();
            Load();
            
            // İstatistikleri başlat
            StatisticsManager.Instance.StartSession(LevelCounter, GameSettings.Instance.PlayerId);
        }

        ~Level() 
        {
            UnloadContent();
        }

        private void Initialize() 
        {
            if (LevelCounter == 1)
                Score = 0;

            if (Globals.IsWaveMode)
            {
                gameState = GameState.Active;
                EndlessManager.Instance.Reset();
            }
            else if (Level.LevelCounter == 1)
                gameState = GameState.Scenario;
            else
                gameState = GameState.Loading;

            reachedExit = false;
            gameOver = false;

            levelBuilder = new LevelBuilder();
            exitDoor = new ExitDoor();

            DrawableSystem.Clear();
            isPaused = false;
            timeToExit = 1.5;
            canExit = false;
        }

        private void Load() 
        {
            bgSystem = new BackgroundSystem();
            spriteSystem = new SpriteSystem();
            enemySystem = new EnemySystem();
            gunSystem = new GunSystem();
            bossSystem = new BossSystem();
            player = new Player();
            HUD = new HeadUpDisplay(player, levelBuilder);
            explosionSystem = new ExplosionSystem();
            scenarioScene = new ScenarioScene();
            loadingScene = new LoadingScene();
            levelSoundtrack = new LevelSoundtrack();
            SoundManager.Load("exitReached", "Sounds/SoundEffects/exitReached");
        }

        public void UnloadContent() 
        {
            
        }


        public void Update() 
        {
            if (gameState == GameState.Scenario)
            {
                scenarioScene.Update();
            }
            else if (gameState == GameState.Loading)
            {
                loadingScene.Update();
            }
            else if (gameState == GameState.Active)
            {
                // PERFORMANS: SpatialGrid'i her frame başında temizle ve populate et
                SpatialGrid.Instance.Clear();
                
                // Enemy'leri spatial grid'e ekle (PlayerBullet collision için)
                foreach (Enemy enemy in EnemySystem.enemyList)
                {
                    if (enemy != null && enemy.alive)
                    {
                        SpatialGrid.Instance.Add(enemy);
                    }
                }
                
                // PERFORMANS: PlayerBullet'leri de grid'e ekle (EnemyBullet collision için)
                foreach (Sprite bullet in GunSystem.bulletList)
                {
                    if (bullet is PlayerBullet && bullet.alive)
                    {
                        SpatialGrid.Instance.Add(bullet);
                    }
                }

                bgSystem.Update();
                spriteSystem.Update();
                levelBuilder.Update();
                levelSoundtrack.Update();
                explosionSystem.Update();

                if (Globals.IsWaveMode)
                    EndlessManager.Instance.Update();
                else
                    exitDoor.Update();

                HUD.Update();

                OnGameOver();

                if (!Globals.IsWaveMode && exitDoor.IsOpen() )
                {
                    if (!canExit && player.CollisionWithExitDoor(exitDoor))
                    {
                        canExit = true;
                        OnExitReached();
                    }
                }

                if (!Globals.IsWaveMode && canExit)
                {
                    timeToExit -= Globals.GameTime.ElapsedGameTime.TotalSeconds;
                }

                if((int)timeToExit<=0)
                {
                    ReachedExit = true;
                    
                    // Complete statistics session (level completed)
                    StatisticsManager.Instance.CompleteSession(
                        finalScore: Score,
                        isCompleted: true,
                        isGameOver: false
                    );
                }

                for (int i = 0; i < EnemySystem.enemyList.Count; i++)
                {
                    if(EnemySystem.enemyList[i] is Boss){
                       if(!EnemySystem.enemyList[i].alive || EnemySystem.enemyList[i].GetHealth()<=0)
                        {
                            explosionSystem.AddBossExplosion((Boss)EnemySystem.enemyList[i]);
                        }
                    }
                    else continue;
                }
            }

            CheckGameState();
        }

        public void Draw() 
        {
            if (gameState == GameState.Scenario)
            {
                scenarioScene.Draw();
            }
            else if (gameState == GameState.Loading)
            {
                loadingScene.Draw();
            }
            else if (gameState == GameState.Active)
            {
                bgSystem.Draw();
                spriteSystem.Draw();
                explosionSystem.Draw();
                exitDoor.Draw();
                HUD.Draw();
            }
        }

        public void HandleInput(InputState i)
        {
            InputState input = i;
            if (gameState == GameState.Scenario && scenarioScene.GetKeyForStarting())
            {
                if (input.MenuSelect || scenarioScene.ClickableArea.IsClicked || input.IsLeftClicked())
                    gameState = GameState.Loading;
            }

            if (gameState == GameState.Loading && loadingScene.GetKeyForStarting())
            {
                if (input.MenuSelect || loadingScene.ClickableArea.IsClicked || input.IsLeftClicked())
                    gameState = GameState.Active;
            }

            player.HandleInput(input);
        }

        public static void AddScore(int score)
        {
            Score += score;
        }

        public void Pause() 
        {
            isPaused = true;
            levelSoundtrack.Pause();
        }

        public void Resume()
        {
            isPaused = false;
            levelSoundtrack.Update(); // Force update to check Resume condition
        }

        private void OnExitReached()
        {
            if(LevelCounter!=10)
            {
                try
                {
                    SoundManager.Play("exitReached");
                    levelSoundtrack.Pause();
                    // Direct save using GameSettings (Windows.Storage compatible)
                    GameSettings.Instance.SaveLevelProgress(LevelCounter, Score);
                }
                catch (Exception ex)
                {
                    // Log save error
                    try
                    {
                        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string logPath = System.IO.Path.Combine(docPath, "PaintTrek_CrashLog.txt");
                        using (var writer = new System.IO.StreamWriter(logPath, true))
                        {
                            writer.WriteLine("--------------------------------------------------");
                            writer.WriteLine($"[Level.OnExitReached] Save Error Date: {DateTime.Now}");
                            writer.WriteLine($"Exception: {ex.Message}");
                            writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                            writer.WriteLine("--------------------------------------------------");
                        }
                    }
                    catch { }
                }
            }
        }

        public void OnGameOver()
        {
            if (player != null && player.OnKilled() && gameState == GameState.Active)
            {
                gameState = GameState.GameOver;
                
                // Complete statistics session (game over)
                StatisticsManager.Instance.CompleteSession(
                    finalScore: Score,
                    isCompleted: false,
                    isGameOver: true
                );
            }
        }

        public void Dispose() 
        {
            Logger.Log("[Level] Disposing...");
            // Dispose soundtrack
            if (levelSoundtrack != null)
            {
                levelSoundtrack.Dispose();
            }

            // Clear all sprite lists to prevent memory leaks
            SpriteSystem.ClearList();
            EnemySystem.ClearList();
            GunSystem.ClearList();
            BossSystem.ClearList();
            CollectableObjectSystem.ClearList();
            
            // Clear pool systems
            BulletPool.ClearAll();
            SupplyPool.ClearAll();
            
            // Clear drawable system
            DrawableSystem.Clear();

            GC.Collect();
            Logger.Log("[Level] Disposed.");
        }


        #region GameState Methods
        public GameState GetGameState()
        {
            return gameState;
        }

        public void SetGameState(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void CheckGameState()
        {
            if (gameOver)
                gameState = GameState.GameOver;
        }
        #endregion


        
    }
}
