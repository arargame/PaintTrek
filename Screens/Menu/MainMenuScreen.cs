using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
// using Microsoft.Xna.Framework.GamerServices; // Not available in MonoGame

namespace PaintTrek
{
    class MainMenuScreen:MenuScreen
    {
        FileSystem fileSystem;
        
        public MainMenuScreen()
        {
            Initialize();
            fileSystem = new FileSystem("game.save");

            AddEntry(new MenuEntry("New Game",true,0));

            int[] array = fileSystem.LoadFile();
            if (array != null || array.Length == 4) 
            {
                if(array[2]<=1)
                    AddEntry(new MenuEntry("Continue", false, 1));
                else AddEntry(new MenuEntry("Continue", true, 1));
            }
            else 
            AddEntry(new MenuEntry("Continue", false, 1));
            


            AddEntry(new MenuEntry("Options", true, 2));
            AddEntry(new MenuEntry("Extra", true, 3));
            AddEntry(new MenuEntry("Credits", true, 4));
            AddEntry(new MenuEntry("Exit", true, 5));

        }

        ~MainMenuScreen()
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            screenTitle = "Main Menu";
            Globals.Window.Title = screenTitle;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
        }

        public override void MenuSelect(int selectedEntry)
        {
            switch (selectedEntry)
            {
                case 0:
                    ExitScreen();
                    Level.LevelCounter = 1;
                    Level.Score = 0;
                    // Create new game with singleton pattern
                    ScreenManager.AddScreen(GameBoard.CreateNewGame());
                    break;
                case 1:
                    if (MenuEntries[selectedEntry].Enabled) 
                    {
                        ExitScreen();
                        ScreenManager.AddScreen(new ContinueScreen());
                    }
                    break;
                case 2:
                    ExitScreen();
                    ScreenManager.AddScreen(new OptionsScreen());
                    break;
                case 3:
                    ExitScreen();
                    ScreenManager.AddScreen(new ExtraScreen());
                    break;
                case 4:
                    ExitScreen();
                    ScreenManager.AddScreen(new CreditsScreen());
                    break;
                case 5:
                    MenuCancel(SelectedEntry);
                    break;
                default:
                    break;
            }

        }

        public override void MenuCancel(int selectedEntry)
        {
            ExitScreen();
            Globals.exitGame = true;
        }

       

    }
}
