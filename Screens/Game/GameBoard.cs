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
        Level level;
        InfoSystem infoSystem;
        bool isGameActive;

        public GameBoard()
        {
            Initialize();
            level = new Level();
            infoSystem = new InfoSystem(new Vector2(400, 100));
            isGameActive = true;
        }

        ~GameBoard() 
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
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

                    isGameActive = true;
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
            ScreenManager.AddScreen(new GameBoard());
        }
        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            GC.ReRegisterForFinalize(this);
            level.Dispose();
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
            }
            

            if (screenState == ScreenState.Active)
            {
                level.HandleInput(inputState);
            }

        }

    }
}
