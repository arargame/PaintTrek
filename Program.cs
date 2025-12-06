using System;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;

namespace PaintTrek
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            // Clear previous session logs to keep file size manageable
            Logger.Clear();

            // Global exception handlers for non-UI threads and tasks
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                LogCrash((Exception)args.ExceptionObject, "AppDomain.UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                LogCrash(args.Exception, "TaskScheduler.UnobservedTaskException");
                args.SetObserved();
            };

            try
            {
                using var game = new PaintTrek.Game1();
                game.Run();
            }
            catch (Exception ex)
            {
                LogCrash(ex, "Main Loop Exception");
                throw;
            }
        }

        private static void LogCrash(Exception ex, string source)
        {
            try
            {
                string crashInfo = $"--------------------------------------------------\n" +
                                   $"Crash Date: {DateTime.Now}\n" +
                                   $"Source: {source}\n" +
                                   $"Exception: {ex.Message}\n" +
                                   $"Stack Trace: {ex.StackTrace}\n";

                if (ex.InnerException != null)
                {
                    crashInfo += $"Inner Exception: {ex.InnerException.Message}\n" +
                                 $"Inner Stack Trace: {ex.InnerException.StackTrace}\n";
                }
                crashInfo += "--------------------------------------------------\n";

                // Try 1: MyDocuments
                try
                {
                    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string logPath = Path.Combine(docPath, "PaintTrek_CrashLog.txt");
                    File.AppendAllText(logPath, crashInfo);
                }
                catch { }

                // Try 2: LocalFolder (Store Safe)
                try
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = localFolder.CreateFileAsync("crash_store.log", CreationCollisionOption.OpenIfExists).AsTask().Result;
                    File.AppendAllText(file.Path, crashInfo);
                }
                catch { }
            }
            catch
            {
                // Last resort: fails silently
            }
        }
    }
}
