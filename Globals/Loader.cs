using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;

namespace PaintTrek
{
    class Loader
    {
        string filePath;
        IsolatedStorageFileStream isolatedStorageFileStream;

        public Loader()
        {
            filePath = "settings.info";


            isolatedStorageFileStream = null;

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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }
            FileSettingsLoad();
        }


        private void FileSettingsLoad()
        {
            List<string> str = new List<string>();
            string s = null;
            StreamReader streamReader = null;


            isolatedStorageFileStream = null;

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

                streamReader = new StreamReader(isolatedStorageFileStream);

                while ((s = streamReader.ReadLine()) != null)
                {
                    str.Add(s);
                }

            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();

                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();

                if (str.Count >= 3)
                {
                    Globals.AutoAttack = Convert.ToBoolean(str[0]);
                    Globals.GameSoundsActivated = Convert.ToBoolean(str[1]);
                    Globals.Graphics.IsFullScreen = Convert.ToBoolean(str[2]);
                }
                else
                {
                    Globals.AutoAttack = false;
                    Globals.GameSoundsActivated = true;
                    Globals.Graphics.IsFullScreen = true;
                }
            }
        }

        public bool FileSettingsSave()
        {
            StreamWriter streamWriter = null;

            isolatedStorageFileStream = null;

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
            return true;
        }

        public static void Reset()
        {
            //Game.Save 
            FileSystem FS = new FileSystem();
            FS.Reset();

            //Settings.info
            StreamWriter streamWriter = null;
            IsolatedStorageFileStream isolatedStorageFileStream = null;
            try
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                if (!isolatedStorage.FileExists("settings.info"))
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("settings.info", FileMode.CreateNew, isolatedStorage);
                }
                else
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("settings.info", FileMode.Open, isolatedStorage);
                }

                streamWriter = new StreamWriter(isolatedStorageFileStream);
                bool b = false;
                streamWriter.WriteLine(b);//Auto-Attack false
                streamWriter.WriteLine(!b);//Sound true
                streamWriter.WriteLine(!b); //Full-screen true
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
            //time.info

            TimeKeeper TK = new TimeKeeper();
            TK.Reset();

            //isFistTime FALSE

            StreamWriter streamWriter2 = null;
            try
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                if (!isolatedStorage.FileExists("lock.info"))
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("lock.info", FileMode.CreateNew, isolatedStorage);
                }
                else
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("lock.info", FileMode.Open, isolatedStorage);
                }

                streamWriter2 = new StreamWriter(isolatedStorageFileStream);
                streamWriter2.Write(1); // is not first time
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                streamWriter2.Close();

                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
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
