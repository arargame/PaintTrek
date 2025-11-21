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
            int maxLevel = 0;
            int maxScore = 0;

            level = level + 1;
            MathHelper.Clamp(level, 1, 10);


            FileSystem FS = new FileSystem();
            maxLevel = FS.LoadFile()[2];
            maxScore = FS.LoadFile()[3];
            FS.Dispose();

            IsolatedStorageFileStream isolatedStorageFileStream = null;

            BinaryWriter writer = null;
            try
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                if (!isolatedStorage.FileExists("game.save"))
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.CreateNew, isolatedStorage);
                }
                else
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.Open, isolatedStorage);
                }
                writer = new BinaryWriter(isolatedStorageFileStream);
            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
                return;
            }


            if (level > maxLevel)
                maxLevel = level;

            if (score > maxScore)
                maxScore = score;

            try
            {
                writer.Write(score);
                writer.Write(level);
                writer.Write(maxLevel);
                writer.Write(maxScore);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();

                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }
        }

        public int[] LoadFile()
        {
            int[] array = new int[4];

            IsolatedStorageFileStream isolatedStorageFileStream = null;
            BinaryReader reader = null;
            try
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                if (!isolatedStorage.FileExists("game.save"))
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.CreateNew, isolatedStorage);
                }
                else
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.Open, isolatedStorage);
                }

                reader = new BinaryReader(isolatedStorageFileStream);

                array[0] = reader.ReadInt32();
                array[1] = reader.ReadInt32();
                array[2] = reader.ReadInt32();
                array[3] = reader.ReadInt32();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }

            return array;
        }

        public void Reset()
        {
            IsolatedStorageFileStream isolatedStorageFileStream = null;

            try
            {
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForDomain();

                if (!isolatedStorage.FileExists("game.save"))
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.CreateNew, isolatedStorage);
                }
                else
                {
                    isolatedStorageFileStream = new IsolatedStorageFileStream("game.save", FileMode.Open, isolatedStorage);
                }

                BinaryWriter br = new BinaryWriter(isolatedStorageFileStream as Stream);

                for (int i = 0; i < 4; i++)
                {
                    br.Write(1);
                }

                //StreamWriter sw=new StreamWriter(isolatedStorageFileStream);

                /*for (int i = 0; i < 4; i++)
                {
                    sw.WriteLine(1);
                }

                sw.Close();*/
                br.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }
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
