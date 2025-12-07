using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Loader
    {
        string filePath;

        public Loader()
        {
            filePath = "settings.info";
            FileSettingsLoad();
        }


        private void FileSettingsLoad()
        {
            List<string> str = new List<string>();
            string s = null;

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
            string path = Path.Combine(folder, filePath);

            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (!File.Exists(path))
                {
                    // Create default settings file
                    File.WriteAllLines(path, new string[]
                    {
                        "false",  // AutoAttack
                        "true",   // Sound
                        "true"    // FullScreen
                    });
                }

                using (var reader = new StreamReader(path))
                {
                    while ((s = reader.ReadLine()) != null)
                    {
                        str.Add(s);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (str.Count >= 3)
                {
                    Globals.AutoAttack = Convert.ToBoolean(str[0]);
                    Globals.GameSoundsActivated = Convert.ToBoolean(str[1]);
                    // Always use fullscreen (1280x800) - low resolution option removed
                    Globals.Graphics.IsFullScreen = true;
                }
                else
                {
                    Globals.AutoAttack = false;
                    Globals.GameSoundsActivated = true;
                    // Always use fullscreen (1280x800) - low resolution option removed
                    Globals.Graphics.IsFullScreen = true;
                }
            }
        }

        public bool FileSettingsSave()
        {
            try
            {
                // Use GameSettings system instead of old settings.info
                GameSettings.Instance.SyncFromGlobals();
                GameSettings.Instance.Save();
                System.Diagnostics.Debug.WriteLine("[Loader] Settings saved via GameSettings");
                return true;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine($"[Loader] Settings save error: {exc.Message}");
                return false;
            }
        }

        public static void Reset()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
            
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Game.Save - Use GameSettings.Instance.Reset()
            GameSettings.Instance.Reset();

            // Settings.info
            try
            {
                string settingsPath = Path.Combine(folder, "settings.info");
                File.WriteAllLines(settingsPath, new string[]
                {
                    "false",  // Auto-Attack false
                    "true",   // Sound true
                    "true"    // Full-screen true
                });
            }
            catch (Exception exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }

            // time.info
            TimeKeeper TK = new TimeKeeper();
            TK.Reset();

            // lock.info - isFirstTime FALSE
            try
            {
                string lockPath = Path.Combine(folder, "lock.info");
                File.WriteAllText(lockPath, "1"); // is not first time
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Load()
        {
            if (!Globals.Graphics.IsFullScreen)
            {
                Globals.GameFont = Globals.Content.Load<SpriteFont>("Fonts/GameFont_1");
                Globals.MenuFont = Globals.Content.Load<SpriteFont>("Fonts/MenuFont_1");
            }
            else
            {
                Globals.GameFont = Globals.Content.Load<SpriteFont>("Fonts/GameFont_2");
                Globals.MenuFont = Globals.Content.Load<SpriteFont>("Fonts/MenuFont_2");
            }
        }
    }
}
