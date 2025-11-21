using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static SoundSystem exitLevelSound = new SoundSystem("Sounds/SoundEffects/exitReached", false);

        public bool ReachedExit
        {
            get { return reachedExit; }
            set { reachedExit = value; }
        }

        public Level() 
        {
            Initialize();
            Load();
        }

        ~Level() 
        {
            UnloadContent();
        }

        private void Initialize() 
        {
            if (LevelCounter == 1)
                Score = 0;

            if (Level.LevelCounter == 1)
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
            HUD = new HeadUpDisplay(player);
            explosionSystem = new ExplosionSystem();
            scenarioScene = new ScenarioScene();
            loadingScene = new LoadingScene();
            levelSoundtrack = new LevelSoundtrack();
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
                bgSystem.Update();
                spriteSystem.Update();
                levelBuilder.Update();
                levelSoundtrack.Update();
                explosionSystem.Update();

                exitDoor.Update();

                HUD.Update();

                OnGameOver();

                if (exitDoor.IsOpen() )
                {
                    if (player.CollisionWithExitDoor(exitDoor))
                    {
                        canExit = true;
                        OnExitReached();
                    }
                }

                if (canExit)
                {
                    timeToExit -= Globals.GameTime.ElapsedGameTime.TotalSeconds;
                }

                if((int)timeToExit<=0)
                    ReachedExit = true;

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
                if (input.MenuSelect || scenarioScene.ClickableArea.IsClicked)
                    gameState = GameState.Loading;
            }

            if (gameState == GameState.Loading && loadingScene.GetKeyForStarting())
            {
                if (input.MenuSelect || loadingScene.ClickableArea.IsClicked)
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
            levelSoundtrack.Pause();
        }

        private void OnExitReached()
        {
           
            exitLevelSound.Play();
            levelSoundtrack.Pause();
         //   reachedExit = true;
            if(LevelCounter!=10)
            {
                FileSystem file = new FileSystem("game.save");
                file.SaveFile(Score, LevelCounter);
            }
        }

        public void OnGameOver()
        {
            if (player != null && player.OnKilled() && gameState == GameState.Active)
            {
                gameState = GameState.GameOver;
            }
        }

        public void Dispose() 
        {
            levelSoundtrack.Dispose();
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
