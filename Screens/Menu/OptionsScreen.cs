using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace PaintTrek
{
    class OptionsScreen:MenuScreen
    {
        // Track the screen mode when entering options to detect changes
        private bool wasFullScreenOnEntry;

        public OptionsScreen() 
        {
            Initialize();
            LoadMenuEntries();
            
        }

        private void LoadMenuEntries()
        {
            if (MenuEntries.Count > 0)
                MenuEntries.Clear();

            AddEntry(new MenuEntry("Controllers", true, 0));
            
            AddEntry(new MenuEntry("Sound Settings", true, 1));

            // Resolution selection enabled
            if(Globals.Graphics.IsFullScreen)
                AddEntry(new MenuEntry("Resolution : 1280x800", true, 2));
            else
                AddEntry(new MenuEntry("Resolution : 800x600", true, 2));

            if(Globals.AutoAttack)
                AddEntry(new MenuEntry("Auto-Attack : On", true, 3));
            else 
                AddEntry(new MenuEntry("Auto-Attack : Off", true, 3));
            
            if(Globals.DeveloperMode)
                AddEntry(new MenuEntry("Developer Mode : On", true, 4));
            else 
                AddEntry(new MenuEntry("Developer Mode : Off", true, 4));

            AddEntry(new MenuEntry("You Played the game "+TimeKeeper.time, true, 5));

            AddEntry(new MenuEntry("Back", true, 6));
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Options Screen";
            Globals.Window.Title = screenTitle;
            
            // Remember the initial fullscreen state
            wasFullScreenOnEntry = Globals.Graphics.IsFullScreen;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();
            MenuEntries[5].Text = "You Played the game " + TimeKeeper.time;
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();
        }

        public override void MenuSelect(int selectedEntry)
        {

            switch (SelectedEntry) 
            { 
                case 0:
                    ExitScreen();
                    ScreenManager.AddScreen(new ControllersScreen());
                    break;

                case 1:
                    // Sound Settings screen
                    ExitScreen();
                    ScreenManager.AddScreen(new SoundSettingsScreen());
                    break;

                case 2:
                    // Toggle resolution between 1280x800 and 800x600
                    if (Globals.Graphics.IsFullScreen)
                    {
                        GraphicSettings.MakeWindowed();
                    }
                    else
                    {
                        GraphicSettings.MakeFullScreen();
                    }
                    GameSettings.Instance.UpdateSettings(fullScreen: Globals.Graphics.IsFullScreen);
                    LoadMenuEntries();
                    break;

                case 3:
                    Globals.AutoAttack = !Globals.AutoAttack;
                    GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                    LoadMenuEntries();
                    break;

                case 4:
                    Globals.DeveloperMode = !Globals.DeveloperMode;
                    // DeveloperMode is runtime-only, not saved
                    LoadMenuEntries();
                    break;

                case 5:
                    // Play time display - do nothing
                    LoadMenuEntries();
                    break;

                case 6:
                    MenuCancel(SelectedEntry);
                    break;

                default:
                    break;
            }
        }

        public override void MenuLeft(int selectedEntry)
        {
            if(selectedEntry==2)
            {
                // Toggle resolution with left arrow
                if (Globals.Graphics.IsFullScreen)
                {
                    GraphicSettings.MakeWindowed();
                }
                else
                {
                    GraphicSettings.MakeFullScreen();
                }
                GameSettings.Instance.UpdateSettings(fullScreen: Globals.Graphics.IsFullScreen);
                LoadMenuEntries();
            }
            else if(selectedEntry==3)
            {
                Globals.AutoAttack = !Globals.AutoAttack;
                GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                LoadMenuEntries();
            }
            else if(selectedEntry==4)
            {
                Globals.DeveloperMode = !Globals.DeveloperMode;
                // DeveloperMode is runtime-only, not saved
                LoadMenuEntries();
            }
        }

        public override void MenuRight(int selectedEntry)
        {
            if (selectedEntry == 2)
            {
                // Toggle resolution with right arrow
                if (Globals.Graphics.IsFullScreen)
                {
                    GraphicSettings.MakeWindowed();
                }
                else
                {
                    GraphicSettings.MakeFullScreen();
                }
                GameSettings.Instance.UpdateSettings(fullScreen: Globals.Graphics.IsFullScreen);
                LoadMenuEntries();
            }
            else if (selectedEntry == 3)
            {
                Globals.AutoAttack = !Globals.AutoAttack;
                GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                LoadMenuEntries();
            }
            else if (selectedEntry == 4)
            {
                Globals.DeveloperMode = !Globals.DeveloperMode;
                // DeveloperMode is runtime-only, not saved
                LoadMenuEntries();
            }
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }
        public override void ExitScreen()
        {
            // Yeni sistem: GameSettings kullan (otomatik kaydediliyor)
            GameSettings.Instance.SyncFromGlobals();
            GameSettings.Instance.Save();
            
            System.Diagnostics.Debug.WriteLine("[OptionsScreen] Settings saved via GameSettings");
            
            base.ExitScreen();
        }
    }
}
