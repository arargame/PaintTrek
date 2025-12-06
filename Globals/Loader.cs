using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;
using Windows.Storage;
using System.Threading.Tasks;

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
            StreamReader streamReader = null;
            Stream stream = null;

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                
                var item = localFolder.TryGetItemAsync(filePath).AsTask().Result;
                if (item == null)
                {
                    // File doesn't exist, create it? Or just use defaults.
                    // Original code created it if not exists.
                    StorageFile file = localFolder.CreateFileAsync(filePath, CreationCollisionOption.OpenIfExists).AsTask().Result;
                    stream = file.OpenStreamForReadAsync().Result;
                }
                else
                {
                    StorageFile file = (StorageFile)item;
                    stream = file.OpenStreamForReadAsync().Result;
                }

                streamReader = new StreamReader(stream);

                while ((s = streamReader.ReadLine()) != null)
                {
                    str.Add(s);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                
                if (stream != null)
                    stream.Close();

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
            StreamWriter streamWriter = null;
            Stream stream = null;

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = localFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
                
                stream = file.OpenStreamForWriteAsync().Result;
                streamWriter = new StreamWriter(stream);

                streamWriter.WriteLine(Globals.AutoAttack);
                streamWriter.WriteLine(Globals.GameSoundsActivated);
                streamWriter.WriteLine(Globals.Graphics.IsFullScreen);
            }
            catch (Exception exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();
                
                if (stream != null)
                    stream.Close();
            }
            return true;
        }

        public static void Reset()
        {
            //Game.Save 
            // Use GameSettings.Instance.Reset() instead of FileSystem
            GameSettings.Instance.Reset();

            //Settings.info
            StreamWriter streamWriter = null;
            Stream stream = null;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = localFolder.CreateFileAsync("settings.info", CreationCollisionOption.ReplaceExisting).AsTask().Result;
                
                stream = file.OpenStreamForWriteAsync().Result;
                streamWriter = new StreamWriter(stream);
                
                bool b = false;
                streamWriter.WriteLine(b);//Auto-Attack false
                streamWriter.WriteLine(!b);//Sound true
                streamWriter.WriteLine(!b); //Full-screen true
            }
            catch (Exception exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();
                
                if (stream != null)
                    stream.Close();
            }
            //time.info

            TimeKeeper TK = new TimeKeeper();
            TK.Reset();

            //isFistTime FALSE

            StreamWriter streamWriter2 = null;
            Stream stream2 = null;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = localFolder.CreateFileAsync("lock.info", CreationCollisionOption.ReplaceExisting).AsTask().Result;
                
                stream2 = file.OpenStreamForWriteAsync().Result;
                streamWriter2 = new StreamWriter(stream2);
                streamWriter2.Write(1); // is not first time
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (streamWriter2 != null)
                    streamWriter2.Close();
                
                if (stream2 != null)
                    stream2.Close();
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
