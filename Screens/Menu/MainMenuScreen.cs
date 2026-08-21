using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    class MainMenuScreen:MenuScreen
    {
        FileSystem fileSystem;
        
        public MainMenuScreen()
        {
            Initialize();
            fileSystem = new FileSystem("game.save");

            int[] array = fileSystem.LoadFile();
            
            bool continueActive = false;
            // Check if save data exists and player passed level 1
            if (array != null && array.Length >= 3) 
            {
                if(array[2] > 1)
                    continueActive = true;
            }

            if (!continueActive)
            {
                AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.NewGame), true, 0));
            }

            if (continueActive)
                AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Continue), true, 1));
            else
                AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Continue), false, 1));
            


            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Options), true, 2));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Extra), true, 3));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Mods), true, 4));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Credits), true, 5));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Exit), true, 6));

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
                    Globals.CurrentMode = GameMode.Normal;
                    Level.LevelCounter = 1;
                    Level.Score = 0;
                    // Create new game with singleton pattern
                    ScreenManager.AddScreen(GameBoard.CreateNewGame());
                    break;
                case 1:
                    var continueEntry = MenuEntries.FirstOrDefault(e => e.entryNumber == 1);
                    if (continueEntry != null && continueEntry.Enabled) 
                    {
                        ExitScreen();
                        ScreenManager.AddScreen(new ContinueScreen());
                    }
                    else if (continueEntry == null)
                    { 
                         // Fallback or defensive check
                         System.Diagnostics.Debug.WriteLine("[MainMenu] Continue entry not found!");
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
                    ScreenManager.AddScreen(new ModsScreen());
                    break;
                case 5:
                    ExitScreen();
                    ScreenManager.AddScreen(new CreditsScreen());
                    break;
                case 6:
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
