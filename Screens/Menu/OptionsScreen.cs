using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.IO.IsolatedStorage;

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

            if(Globals.GameSoundsActivated)
            AddEntry(new MenuEntry("Sounds : On", true, 1));
            else AddEntry(new MenuEntry("Sounds : Off", true, 1));

            // Resolution selection enabled
            if(Globals.Graphics.IsFullScreen)
                AddEntry(new MenuEntry("Resolution : 1280x800", true, 2));
            else
                AddEntry(new MenuEntry("Resolution : 800x600", true, 2));

            if(Globals.AutoAttack)
            AddEntry(new MenuEntry("Auto-Attack : On", true, 3));
            else AddEntry(new MenuEntry("Auto-Attack : Off", true, 3));

            AddEntry(new MenuEntry("You Played the game "+TimeKeeper.time, true, 4));

            AddEntry(new MenuEntry("Back", true, 5));
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
            MenuEntries[4].Text = "You Played the game " + TimeKeeper.time;
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
                    Globals.GameSoundsActivated = !Globals.GameSoundsActivated;
                    LoadMenuEntries();
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
                    LoadMenuEntries();
                    break;

                case 3:
                    Globals.AutoAttack = !Globals.AutoAttack;
                    LoadMenuEntries();
                    break;

                case 4:

                    #if WINDOWS_PHONE
                        LoadMenuEntries();
                    #elif WINDOWS
                        MenuCancel(selectedEntry);
                    #endif
                        LoadMenuEntries();
                    break;

                case 5:

                    MenuCancel(SelectedEntry);
                    break;

                default:
                    break;
            }
        }

        public override void MenuLeft(int selectedEntry)
        {
            if (selectedEntry == 1) 
            {
                Globals.GameSoundsActivated = !Globals.GameSoundsActivated;
                LoadMenuEntries();
            }
            else if(selectedEntry==2)
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
                LoadMenuEntries();
            }
            else if(selectedEntry==3)
            {
                Globals.AutoAttack = !Globals.AutoAttack;
                LoadMenuEntries();
            }


        }

        public override void MenuRight(int selectedEntry)
        {
            if (selectedEntry == 1)
            {
                Globals.GameSoundsActivated = !Globals.GameSoundsActivated;
                LoadMenuEntries();
            }
            else if (selectedEntry == 2)
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
                LoadMenuEntries();
            }
            else if (selectedEntry == 3)
            {
                Globals.AutoAttack = !Globals.AutoAttack;
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
            // Save settings including resolution
            bool b = false;
            do
            {
                StreamWriter streamWriter = null;

                IsolatedStorageFileStream isolatedStorageFileStream = null;

                string filePath = "settings.info";

                try
                {
                    IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                    if (!isolatedStorage.FileExists(filePath))
                    {
                        isolatedStorageFileStream = new IsolatedStorageFileStream(filePath, FileMode.CreateNew, isolatedStorage);
                    }
                    else
                    {
                        isolatedStorageFileStream = new IsolatedStorageFileStream(filePath, FileMode.Open, isolatedStorage);
                    }

                    streamWriter = new StreamWriter(isolatedStorageFileStream);

                    streamWriter.WriteLine(Globals.AutoAttack);
                    streamWriter.WriteLine(Globals.GameSoundsActivated);
                    streamWriter.WriteLine(Globals.Graphics.IsFullScreen);
                }
                catch (IOException exc)
                {
                    Console.WriteLine("I/O Error\n: " + exc.Message);
                }
                finally
                {
                    if (streamWriter != null)
                        streamWriter.Close();

                    if (isolatedStorageFileStream != null)
                        isolatedStorageFileStream.Close();
                }
                b = true;
            } while (b == false);

            base.ExitScreen();
        }
    }
}
