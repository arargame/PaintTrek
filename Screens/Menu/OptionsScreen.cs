using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    class OptionsScreen:MenuScreen
    {
        public OptionsScreen() 
        {
            Initialize();
            LoadMenuEntries();
            
        }

        private void LoadMenuEntries()
        {
            if (MenuEntries.Count > 0)
                MenuEntries.Clear();

            AddEntry(new MenuEntry(Loc.T(LocKeys.Options.Controllers), true, 0));
            
            AddEntry(new MenuEntry(Loc.T(LocKeys.Options.SoundSettings), true, 1));

            if (Globals.AutoAttack)
                AddEntry(new MenuEntry(Loc.T(LocKeys.Options.AutoAttack) + " : " + Loc.T(LocKeys.Sound.On), true, 2));
            else 
                AddEntry(new MenuEntry(Loc.T(LocKeys.Options.AutoAttack) + " : " + Loc.T(LocKeys.Sound.Off), true, 2));
            
            if (Globals.DeveloperMode)
                AddEntry(new MenuEntry(Loc.T(LocKeys.Options.DeveloperMode) + " : " + Loc.T(LocKeys.Sound.On), true, 3));
            else 
                AddEntry(new MenuEntry(Loc.T(LocKeys.Options.DeveloperMode) + " : " + Loc.T(LocKeys.Sound.Off), true, 3));

            AddEntry(new MenuEntry(string.Format(Loc.T(LocKeys.Options.TimePlayed), TimeKeeper.time), true, 4));

            AddEntry(new MenuEntry(Loc.T(LocKeys.Options.Language), true, 5));

            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Back), true, 6));
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Options Screen";
            Globals.Window.Title = screenTitle;
            
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();
            MenuEntries[4].Text = string.Format(Loc.T(LocKeys.Options.TimePlayed), TimeKeeper.time);
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
                    Globals.AutoAttack = !Globals.AutoAttack;
                    GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                    LoadMenuEntries();
                    break;

                case 3:
                    Globals.DeveloperMode = !Globals.DeveloperMode;
                    // DeveloperMode is runtime-only, not saved
                    LoadMenuEntries();
                    break;

                case 4:
                    // Play time display - do nothing
                    LoadMenuEntries();
                    break;

                case 5:
                    // Language screen
                    ExitScreen();
                    ScreenManager.AddScreen(new LanguageScreen(typeof(OptionsScreen)));
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
                Globals.AutoAttack = !Globals.AutoAttack;
                GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                LoadMenuEntries();
            }
            else if(selectedEntry==3)
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
                Globals.AutoAttack = !Globals.AutoAttack;
                GameSettings.Instance.UpdateSettings(autoAttack: Globals.AutoAttack);
                LoadMenuEntries();
            }
            else if (selectedEntry == 3)
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
