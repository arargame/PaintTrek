using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class FileSystem
    {
        string filePath;
        List<int> amount;
        List<string> type;
        List<string> str;

        public List<int> Amount
        {
            get { return amount; }
        }
        public List<string> Type
        {
            get { return type; }
        }


        public FileSystem(string path)
        {
            amount = new List<int>();
            type = new List<string>();
            str = new List<string>();
            filePath = path;
            
            // Legacy file check removed. GameSettings handles file I/O.
        }
        public FileSystem()
        {
            amount = new List<int>();
            type = new List<string>();
            str = new List<string>();
        }

        ~FileSystem()
        {
            Dispose();
        }

        private void Dispose()
        {

        }

        public void SaveFile(int score, int level)
        {
            // Yeni sistem: GameSettings kullan (singleton, otomatik cache)
            GameSettings.Instance.SaveLevelProgress(level, score);
        }

        public int[] LoadFile()
        {
            // Yeni sistem: GameSettings kullan (singleton, otomatik cache)
            GameSettings settings = GameSettings.Instance;
            
            int[] array = new int[14];
            array[0] = settings.CurrentScore;
            array[1] = settings.CurrentLevel;
            array[2] = settings.MaxLevel;
            array[3] = settings.MaxScore;
            
            // Level scores
            for (int i = 0; i < 10; i++)
            {
                array[4 + i] = settings.LevelScores[i];
            }
            
            return array;
        }
        
        // Belirli bir level'ın skorunu getir
        public int GetLevelScore(int level)
        {
            return GameSettings.Instance.GetLevelScore(level);
        }

        public void Reset()
        {
            // Yeni sistem: GameSettings kullan
            GameSettings.Instance.Reset();
        }

        public void ReadLevel()
        {
            string s = null;

            Stream fileStream = null;
            StreamReader reader = null;

            try
            {


                fileStream = TitleContainer.OpenStream("Content/Levels/level" + Level.LevelCounter + ".txt");
                reader = new StreamReader(fileStream);


                while ((s = reader.ReadLine()) != null)
                {
                    str.Add(s);
                }

                reader.Close();
            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }


            if (str.Count % 2 != 0)
                return;

            for (int i = 0, j = 1; i < str.Count + 1 && j < str.Count; i += 2, j += 2)
            {
                amount.Add(Convert.ToInt32(str[i]));
                type.Add(str[j]);
            }
        }

        public bool isFirstTime()
        {
            int number = 0;
            Stream stream = null;
            StreamReader st = null;

            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                
                var item = localFolder.TryGetItemAsync("lock.info").AsTask().Result;
                Windows.Storage.StorageFile file;

                if (item == null)
                {
                    file = localFolder.CreateFileAsync("lock.info", Windows.Storage.CreationCollisionOption.OpenIfExists).AsTask().Result;
                }
                else
                {
                    file = (Windows.Storage.StorageFile)item;
                }

                stream = file.OpenStreamForReadAsync().Result;
                st = new StreamReader(stream);
                
                string line = st.ReadLine();
                if (line != null)
                {
                    number = Convert.ToInt32(line);
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("[FileSystem] isFirstTime Error: " + exc.Message);
            }
            finally
            {
                if (st != null) st.Close();
                if (stream != null) stream.Close();
            }

            return number == 0;
        }
    }
}
