using System;

namespace PaintTrek
{
    /// <summary>
    /// Test utility for GameSettings file I/O operations
    /// Usage: Call GameSettingsTest.RunTests() from anywhere to test file operations
    /// </summary>
    public static class GameSettingsTest
    {
        public static void RunTests()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("GAMESETTINGS FILE I/O TEST");
            Console.WriteLine("========================================");
            Console.WriteLine();
            
            // Test 1: Basic File I/O
            Console.WriteLine("Test 1: Basic File I/O Test");
            bool test1 = GameSettings.Instance.TestFileIO();
            Console.WriteLine($"Result: {(test1 ? "PASSED" : "FAILED")}");
            Console.WriteLine();
            
            // Test 2: Settings Persistence
            Console.WriteLine("Test 2: Settings Persistence Test");
            bool test2 = TestSettingsPersistence();
            Console.WriteLine($"Result: {(test2 ? "PASSED" : "FAILED")}");
            Console.WriteLine();
            
            // Test 3: Display Current Settings
            Console.WriteLine("Test 3: Current Settings Info");
            Console.WriteLine(GameSettings.Instance.GetInfo());
            Console.WriteLine();
            
            // Summary
            Console.WriteLine("========================================");
            Console.WriteLine("TEST SUMMARY");
            Console.WriteLine("========================================");
            Console.WriteLine($"Basic File I/O: {(test1 ? "✓ PASSED" : "✗ FAILED")}");
            Console.WriteLine($"Settings Persistence: {(test2 ? "✓ PASSED" : "✗ FAILED")}");
            Console.WriteLine($"Overall: {(test1 && test2 ? "✓ ALL TESTS PASSED" : "✗ SOME TESTS FAILED")}");
            Console.WriteLine();
        }
        
        private static bool TestSettingsPersistence()
        {
            try
            {
                // Backup current settings
                bool backupSFX = GameSettings.Instance.SoundEffectsEnabled;
                bool backupMusic = GameSettings.Instance.MusicEnabled;
                bool backupMenu = GameSettings.Instance.MenuSoundsEnabled;
                
                // Change settings
                GameSettings.Instance.UpdateSettings(
                    soundEffects: !backupSFX,
                    music: !backupMusic,
                    menuSounds: !backupMenu
                );
                
                Console.WriteLine($"  Changed settings - SFX: {!backupSFX}, Music: {!backupMusic}, Menu: {!backupMenu}");
                
                // Force save
                GameSettings.Instance.MarkDirty();
                GameSettings.Instance.Save();
                
                // Reload by clearing instance
                var field = typeof(GameSettings).GetField("instance", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                field.SetValue(null, null);
                
                // Get new instance (will load from file)
                var reloaded = GameSettings.Instance;
                
                // Verify
                bool success = (
                    reloaded.SoundEffectsEnabled == !backupSFX &&
                    reloaded.MusicEnabled == !backupMusic &&
                    reloaded.MenuSoundsEnabled == !backupMenu
                );
                
                Console.WriteLine($"  Reloaded settings - SFX: {reloaded.SoundEffectsEnabled}, Music: {reloaded.MusicEnabled}, Menu: {reloaded.MenuSoundsEnabled}");
                
                // Restore original settings
                GameSettings.Instance.UpdateSettings(
                    soundEffects: backupSFX,
                    music: backupMusic,
                    menuSounds: backupMenu
                );
                
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
