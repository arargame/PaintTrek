using System;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;

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
                    
                    // Use standard IO since we have runFullTrust and it's synchronous
                    // But to be 100% safe with UWP/Store paths, let's use the LocalFolder path
                    string folderPath = ApplicationData.Current.LocalFolder.Path;
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
                string folderPath = ApplicationData.Current.LocalFolder.Path;
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
