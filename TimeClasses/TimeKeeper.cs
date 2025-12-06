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
            Load();
        }

        public void Load()
        {
            oldTime = new TimeSpan(0, 0, 0);
            
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var item = localFolder.TryGetItemAsync(filePath).AsTask().Result;
                
                if (item != null)
                {
                    Windows.Storage.StorageFile file = (Windows.Storage.StorageFile)item;
                    string content = Windows.Storage.FileIO.ReadTextAsync(file).AsTask().Result;
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        // Remove any potential newline characters
                        content = content.Trim();
                        oldTime = TimeSpan.Parse(content);
                    }
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("[TimeKeeper] Load Error: " + exc.Message);
            }
        }

        public void Save()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = localFolder.CreateFileAsync(filePath, Windows.Storage.CreationCollisionOption.OpenIfExists).AsTask().Result;
                
                string content = (newTime + oldTime).ToString();
                Windows.Storage.FileIO.WriteTextAsync(file, content).AsTask().Wait();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("[TimeKeeper] Save Error: " + exc.Message);
            }
        }

        public void Reset()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile file = localFolder.CreateFileAsync(filePath, Windows.Storage.CreationCollisionOption.OpenIfExists).AsTask().Result;
                
                string content = new TimeSpan(0, 0, 0).ToString();
                Windows.Storage.FileIO.WriteTextAsync(file, content).AsTask().Wait();
                
                oldTime = TimeSpan.Zero;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("[TimeKeeper] Reset Error: " + exc.Message);
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
