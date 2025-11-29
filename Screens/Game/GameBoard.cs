using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace PaintTrek
{
    class GameBoard : GameScreen
    {
        // Singleton instance
        private static GameBoard instance = null;
        private static readonly object lockObject = new object();
        
        Level level;
        InfoSystem infoSystem;
        bool isGameActive;

        private GameBoard()
        {
            Initialize();
            level = new Level();
            infoSystem = new InfoSystem(new Vector2(400, 100));
            isGameActive = true;
        }
        
        // Singleton accessor
        public static GameBoard GetInstance()
        {
            lock (lockObject)
            {
                // If instance exists and is not inactive, return it
                if (instance != null && instance.screenState != ScreenState.Inactive)
                {
                    return instance;
                }
                
                // Create new instance
                instance = new GameBoard();
                return instance;
            }
        }
        
        // Factory method for creating new game
        public static GameBoard CreateNewGame()
        {
            lock (lockObject)
            {
                // Dispose old instance if exists
                if (instance != null)
                {
                    instance.ForceDispose();
                }
                
                // Create new instance
                instance = new GameBoard();
                return instance;
            }
        }
        
        // Force dispose for cleanup
        private void ForceDispose()
        {
            if (level != null)
            {
                level.Dispose();
            }
            UnloadContent();
            screenState = ScreenState.Inactive;
        }

        ~GameBoard() 
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            Globals.ShowCursor = false;
            // ...
            screenTitle = "Paint Trek";
            Globals.Window.Title = screenTitle;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            level.UnloadContent();
            infoSystem.UnloadContent();
        }


        public override void Update()
        {
            if (screenState == ScreenState.Active)
            {

                if (!Globals.IsActive && level.GetGameState() == GameState.Active)
                {
                    level.Pause();
                    this.screenState = ScreenState.Frozen;
                    ScreenManager.AddScreen(new PauseScreen(this));
                    isGameActive = false;
                    Globals.ShowCursor = true;
                }
                else
                {
                    level.Update();
                    if (level.ReachedExit)
                    {
                        if (Level.LevelCounter == 10) 
                        {
                            ExitScreen();
                            ScreenManager.AddScreen(new EndScreen());
                        }
                        else LoadNextLevel();
                    }

                    if (level.GetGameState() == GameState.GameOver)
                    {
                        ExitScreen();
                        ScreenManager.AddScreen(new GameOverScreen());
                    }

                    infoSystem.Update();
                    base.Update();

                    isGameActive = level.GetGameState() == GameState.Active;
                    if (isGameActive)
                    {
                        Globals.ShowCursor = false;
                    }
                }
            }
        }



        public override void Draw()
        {
            if (screenState != ScreenState.Inactive)
            {
                Globals.Graphics.GraphicsDevice.Clear(Color.Black);
                level.Draw();
                infoSystem.Draw();
            }
        }

        private void LoadNextLevel()
        {
            string path=null;

            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                path = string.Format("Content/Levels/level{0}.txt", ++Level.LevelCounter);

                if (File.Exists(path))
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (Level.LevelCounter == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                Level.LevelCounter = -1;
            }

            ExitScreen();
            ScreenManager.AddScreen(GameBoard.CreateNewGame());
        }
        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            
            // Dispose level and clear all resources
            if (level != null)
            {
                level.Dispose();
            }
            
            UnloadContent();
            
            // Clear singleton instance
            lock (lockObject)
            {
                instance = null;
            }
            
            GC.ReRegisterForFinalize(this);
        }

        public override void HandleInput()
        {
            if (screenState != ScreenState.Active || inputState == null)
                return;

            inputState.Update();

            if (isGameActive && inputState.PauseGame && level.GetGameState() == GameState.Active)
            {
                level.Pause();
                this.screenState = ScreenState.Frozen;
                ScreenManager.AddScreen(new PauseScreen(this));
                Globals.ShowCursor = true;
            }
            

            if (screenState == ScreenState.Active)
            {
                level.HandleInput(inputState);
            }

        }

    }
}
