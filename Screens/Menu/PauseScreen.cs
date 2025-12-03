using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class PauseScreen:MenuScreen
    {
        GameBoard gameBoard;
        FileSystem fs;
        Texture2D pixel;

        public PauseScreen(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
            Initialize();
            LoadMenuEntries();
            fs = new FileSystem("game.save");
        }
        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Pause Screen";
            Globals.Window.Title = screenTitle;
            Globals.ShowCursor = true;
        }
        public override void Load()
        {
            base.Load();
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            Globals.ShowCursor = true;
            base.Update();
        }
        public void LoadMenuEntries()
        {
            if (MenuEntries.Count > 0)
                MenuEntries.Clear();

            AddEntry(new MenuEntry("Resume Game", true, 0));
            AddEntry(new MenuEntry("Restart", true, 1) );
            AddEntry(new MenuEntry("Main Menu", true, 2));
            AddEntry(new MenuEntry("Sound Settings", true, 3));
            AddEntry(new MenuEntry("Quit Game", true, 4));
        }

        public override void MenuSelect(int selectedEntry)
        {
            switch (selectedEntry)
            {
                case 0:
                    MenuCancel(SelectedEntry);
                    break;
                case 1:
                    if (Level.LevelCounter == 1)
                    {
                        Level.Score = 0;
                        gameBoard.ExitScreen();
                        ScreenManager.AddScreen(GameBoard.CreateNewGame());
                    }
                    else if (fs.LoadFile() != null)
                    {
                        Level.LevelCounter = Convert.ToInt32(fs.LoadFile()[1]);
                        Level.Score = Convert.ToInt32(fs.LoadFile()[0]);
                        gameBoard.ExitScreen();
                        ScreenManager.AddScreen(GameBoard.CreateNewGame());
                    }
                    ExitScreen();
                    break;
                case 2:
                    gameBoard.ExitScreen();
                    ScreenManager.AddScreen(new MainMenuScreen());
                    ExitScreen();
                    break;
                case 3:
                    // Sound Settings screen
                    ExitScreen();
                    var soundSettings = new SoundSettingsScreen(typeof(PauseScreen), gameBoard);
                    ScreenManager.AddScreen(soundSettings);
                    break;
                case 4:
                    ExitScreen();
                    Globals.exitGame = true;
                    break;
            }
        }

        public override void MenuRight(int selectedEntry)
        {
            // No toggle actions needed anymore
        }
        
        public override void MenuLeft(int selectedEntry)
        {
            // No toggle actions needed anymore
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            gameBoard.screenState = ScreenState.Active;
            ExitScreen();
        }
        public override void ExitScreen()
        {
            base.ExitScreen();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Draw()
        {
            Fade();
            base.Draw();
        }

        public void Fade()
        {

            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(pixel, new Rectangle(0, 0, Globals.Graphics.GraphicsDevice.Viewport.Width, Globals.Graphics.GraphicsDevice.Viewport.Height),
                new Color((byte)0, (byte)0, (byte)0, (byte)(0.5 * 255)));
            Globals.SpriteBatch.End();
        }
    }
}
