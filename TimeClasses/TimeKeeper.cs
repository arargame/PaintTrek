using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

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
                string localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
                string fullPath = Path.Combine(localFolder, filePath);
                
                if (File.Exists(fullPath))
                {
                    string content = File.ReadAllText(fullPath);
                    
                    if (!string.IsNullOrEmpty(content))
                    {
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
                string localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
                Directory.CreateDirectory(localFolder);
                string fullPath = Path.Combine(localFolder, filePath);
                
                string content = (newTime + oldTime).ToString();
                File.WriteAllText(fullPath, content);
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
                string localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
                Directory.CreateDirectory(localFolder);
                string fullPath = Path.Combine(localFolder, filePath);
                
                string content = new TimeSpan(0, 0, 0).ToString();
                File.WriteAllText(fullPath, content);
                
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
