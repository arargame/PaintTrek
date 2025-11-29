using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class GameOverScreen:MenuScreen
    {
        Texture2D gameOverTexture;
        public GameOverScreen()
        {
            Initialize();
            AddEntry(new MenuEntry("Play Again,Your Score:" + Level.Score, true, 0) );
            AddEntry(new MenuEntry("Return to Menu", true, 1));
            AddEntry(new MenuEntry("Quit Game", true, 2));
            gameOverTexture = Globals.Content.Load<Texture2D>("Backgrounds/gameOver");

        }
        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Game Over";
            Globals.Window.Title = screenTitle;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void MenuSelect(int selectedEntry)
        {
            switch (selectedEntry)
            {
                case 0:
                    ExitScreen();
                    // Create new game with singleton pattern
                    ScreenManager.AddScreen(GameBoard.CreateNewGame()); 
                    break;
                case 1:
                    MenuCancel(selectedEntry);
                    break;
                case 2: Globals.exitGame = true;
                    ExitScreen();
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied);
            Globals.SpriteBatch.Draw(gameOverTexture, new Rectangle(0, 0, (int)Globals.GameSize.X, (int)Globals.GameSize.Y), new Color(255, 255, 255, 64));
            Globals.SpriteBatch.End();
            base.Draw();
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void ExitScreen()
        {
            FileSystem fs = new FileSystem("game.save");
            if (fs.LoadFile() != null)
            {
                int lvlCounter=Convert.ToInt32(fs.LoadFile()[1]);

                if(lvlCounter==Level.LevelCounter)
                Level.LevelCounter = lvlCounter;

                Level.Score = Convert.ToInt32(fs.LoadFile()[0]);
            }

            base.ExitScreen();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }
    }
}
