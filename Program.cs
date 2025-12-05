using System;

try
            {
                using var game = new PaintTrek.Game1();
                game.Run();
            }
            catch (Exception ex)
            {
                try
                {
                    using (var storage = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain())
                    {
                        using (var stream = new System.IO.IsolatedStorage.IsolatedStorageFileStream("crash.log", System.IO.FileMode.Create, storage))
                        using (var writer = new System.IO.StreamWriter(stream))
                        {
                            writer.WriteLine($"Crash Date: {DateTime.Now}");
                            writer.WriteLine($"Exception: {ex.Message}");
                            writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                            if (ex.InnerException != null)
                            {
                                writer.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                                writer.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                            }
                        }
                    }
                }
                catch
                {
                    // If logging fails, there's not much we can do.
                }
                throw; // Re-throw to let the OS handle the crash reporting if needed
            }
