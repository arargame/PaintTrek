using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.IO.IsolatedStorage;

namespace PaintTrek
{
    class TimeKeeper
    {
        TimeSpan oldTime;
        TimeSpan newTime;
        IsolatedStorageFileStream isolatedStorageFileStream;
        string filePath;
        string str;
        public static string time;
        public static TimeSpan timeSpan;
        public TimeKeeper()
        {
            Initialize();
        }

        ~TimeKeeper()
        {
            Save();
        }

        public void Initialize()
        {

            filePath = "time.info";

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

            Load();
        }

        public void Load()
        {
            oldTime = new TimeSpan(0, 0, 0);
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
                str = streamReader.ReadLine();
            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error\n: " + exc.Message);
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();

                if (str != null)
                    oldTime = TimeSpan.Parse(str);

                if (isolatedStorageFileStream != null)
                    isolatedStorageFileStream.Close();
            }
        }

        public void Save()
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
                streamWriter.WriteLine(newTime + oldTime);
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
        }
        public void Reset()
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
                streamWriter.WriteLine(new TimeSpan(0, 0, 0));
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
        }
        public void Update()
        {
            if (Globals.GameTime != null)
                newTime = Globals.GameTime.TotalGameTime;

            TimeSpan ts = oldTime + newTime;
            timeSpan = ts;
            time = (int)ts.TotalHours + ":" + (int)ts.Minutes + ":" + (int)ts.Seconds;
        }

        public TimeSpan GetOldTime()
        {
            return oldTime + newTime;
        }

        public string GetOldTime2()
        {
            TimeSpan ts = oldTime + newTime;
            string str = (int)ts.TotalHours + ":" + (int)ts.Minutes + ":" + (int)ts.Seconds;
            return str;
        }
    }
}
