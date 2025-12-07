using System;
using System.IO;

namespace PaintTrek
{
    public static class Logger
    {
        private static string logFileName = "game_trace.log";
        private static object lockObj = new object();

        public static void Log(string message)
        {
            // Also write to debug console for development
            System.Diagnostics.Debug.WriteLine(message);

            try
            {
                lock (lockObj)
                {
                    string logEntry = $"{DateTime.Now:HH:mm:ss.fff} - {message}{Environment.NewLine}";
                    
                    // Use standard IO with LocalApplicationData folder
                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
                    Directory.CreateDirectory(folderPath);
                    string fullPath = Path.Combine(folderPath, logFileName);
                    
                    File.AppendAllText(fullPath, logEntry);
                }
            }
            catch (Exception)
            {
                // Fail silently to avoid crashing the logger itself
            }
        }

        public static void Clear()
        {
            try
            {
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PaintTrek");
                string fullPath = Path.Combine(folderPath, logFileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch { }
        }
    }
}
