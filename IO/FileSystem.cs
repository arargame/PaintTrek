using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;

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
            IsolatedStorageFileStream isolatedStorageFileStream = null;

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

            IsolatedStorageFileStream isolatedStorageFileStream = null;


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

                StreamReader st = new StreamReader(isolatedStorageFileStream);
                number = Convert.ToInt32(st.ReadLine());

                st.Close();
            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }

            return number == 0;
        }
    }
}
