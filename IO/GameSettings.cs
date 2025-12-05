using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace PaintTrek
{
    public class GameSettings
    {
        private static GameSettings instance;
        private static readonly object lockObject = new object();
        
        public int CurrentScore { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public int MaxScore { get; set; }
        public int[] LevelScores { get; set; }
        
        public bool SoundEffectsEnabled { get; set; }
        public bool MusicEnabled { get; set; }
        public bool MenuSoundsEnabled { get; set; }
        public bool AutoAttack { get; set; }
        public bool IsFullScreen { get; set; }
        // DeveloperMode is runtime-only, not saved to file
        
        private bool isDirty = false;
        
        private GameSettings()
        {
            CurrentScore = 0;
            CurrentLevel = 1;
            MaxLevel = 1;
            MaxScore = 0;
            LevelScores = new int[10];
            
            SoundEffectsEnabled = true;
            MusicEnabled = true;
            MenuSoundsEnabled = true;
            AutoAttack = false;
            IsFullScreen = true;
            // DeveloperMode is not persisted
        }
        
        public static GameSettings Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new GameSettings();
                        instance.Load();
                    }
                    return instance;
                }
            }
        }
        
        public void Load()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            IsolatedStorageFileStream stream = null;
            BinaryReader reader = null;
            
            try
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
                
                if (!storage.FileExists("game.save"))
                {
                    System.Diagnostics.Debug.WriteLine("[GameSettings] Save file not found, using defaults");
                    isDirty = false;
                    return;
                }
                
                stream = new IsolatedStorageFileStream("game.save", FileMode.Open, storage);
                reader = new BinaryReader(stream);
                
                CurrentScore = reader.ReadInt32();
                CurrentLevel = reader.ReadInt32();
                MaxLevel = reader.ReadInt32();
                MaxScore = reader.ReadInt32();
                
                for (int i = 0; i < 10; i++)
                {
                    try { LevelScores[i] = reader.ReadInt32(); }
                    catch { LevelScores[i] = 0; }
                }
                
                try
                {
                    SoundEffectsEnabled = reader.ReadBoolean();
                    MusicEnabled = reader.ReadBoolean();
                    MenuSoundsEnabled = reader.ReadBoolean();
                    AutoAttack = reader.ReadBoolean();
                    IsFullScreen = reader.ReadBoolean();
                    // DeveloperMode is not loaded from file
                }
                catch { }
                
                isDirty = false;
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Loaded in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Load error: {ex.Message}");
            }
            finally
            {
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
            }
        }
        
        public void Save()
        {
            if (!isDirty) return;
            
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            IsolatedStorageFileStream stream = null;
            BinaryWriter writer = null;
            
            try
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
                stream = new IsolatedStorageFileStream("game.save", FileMode.Create, storage);
                writer = new BinaryWriter(stream);
                
                writer.Write(CurrentScore);
                writer.Write(CurrentLevel);
                writer.Write(MaxLevel);
                writer.Write(MaxScore);
                
                for (int i = 0; i < 10; i++)
                    writer.Write(LevelScores[i]);
                
                writer.Write(SoundEffectsEnabled);
                writer.Write(MusicEnabled);
                writer.Write(MenuSoundsEnabled);
                writer.Write(AutoAttack);
                writer.Write(IsFullScreen);
                // DeveloperMode is not saved to file
                
                isDirty = false;
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Saved in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Save error: {ex.Message}");
            }
            finally
            {
                if (writer != null) writer.Close();
                if (stream != null) stream.Close();
            }
        }
        
        public void SaveLevelProgress(int level, int score)
        {
            CurrentScore = score;
            CurrentLevel = level + 1;
            
            if (CurrentLevel > MaxLevel)
                MaxLevel = CurrentLevel;
            
            if (score > MaxScore)
                MaxScore = score;
            
            if (level >= 1 && level <= 10)
                LevelScores[level - 1] = score;
            
            isDirty = true;
            Save();
        }
        
        public void MarkDirty()
        {
            isDirty = true;
        }
        
        public int GetLevelScore(int level)
        {
            if (level < 1 || level > 10) return 0;
            return LevelScores[level - 1];
        }
        
        public void Reset()
        {
            CurrentScore = 0;
            CurrentLevel = 1;
            MaxLevel = 1;
            MaxScore = 0;
            
            for (int i = 0; i < 10; i++)
                LevelScores[i] = 0;
            
            isDirty = true;
            Save();
            
            System.Diagnostics.Debug.WriteLine("[GameSettings] Reset completed");
        }
        
        // Ayarları güncelle (UI'dan çağrılacak)
        public void UpdateSettings(bool? soundEffects = null, bool? music = null, bool? menuSounds = null, 
                                   bool? autoAttack = null, bool? fullScreen = null)
        {
            if (soundEffects.HasValue && SoundEffectsEnabled != soundEffects.Value)
            {
                SoundEffectsEnabled = soundEffects.Value;
                isDirty = true;
            }
            
            if (music.HasValue && MusicEnabled != music.Value)
            {
                MusicEnabled = music.Value;
                isDirty = true;
            }
            
            if (menuSounds.HasValue && MenuSoundsEnabled != menuSounds.Value)
            {
                MenuSoundsEnabled = menuSounds.Value;
                isDirty = true;
            }
            
            if (autoAttack.HasValue && AutoAttack != autoAttack.Value)
            {
                AutoAttack = autoAttack.Value;
                isDirty = true;
            }
            
            if (fullScreen.HasValue && IsFullScreen != fullScreen.Value)
            {
                IsFullScreen = fullScreen.Value;
                isDirty = true;
            }
            
            // DeveloperMode is not persisted
            
            if (isDirty)
            {
                Save();
                System.Diagnostics.Debug.WriteLine("[GameSettings] Settings updated");
            }
        }
        
        // Globals ile senkronize et
        public void SyncToGlobals()
        {
            Globals.SoundEffectsEnabled = SoundEffectsEnabled;
            Globals.MusicsEnabled = MusicEnabled;
            Globals.MenuSoundsEnabled = MenuSoundsEnabled;
            Globals.AutoAttack = AutoAttack;
            // DeveloperMode is runtime-only, not synced from file
            
            // Eski GameSoundsActivated için backward compatibility
            Globals.GameSoundsActivated = SoundEffectsEnabled || MusicEnabled;
        }
        
        // Globals'den ayarları al
        public void SyncFromGlobals()
        {
            bool changed = false;
            
            if (SoundEffectsEnabled != Globals.SoundEffectsEnabled)
            {
                SoundEffectsEnabled = Globals.SoundEffectsEnabled;
                changed = true;
            }
            
            if (MusicEnabled != Globals.MusicsEnabled)
            {
                MusicEnabled = Globals.MusicsEnabled;
                changed = true;
            }
            
            if (MenuSoundsEnabled != Globals.MenuSoundsEnabled)
            {
                MenuSoundsEnabled = Globals.MenuSoundsEnabled;
                changed = true;
            }
            
            if (AutoAttack != Globals.AutoAttack)
            {
                AutoAttack = Globals.AutoAttack;
                changed = true;
            }
            
            // DeveloperMode is runtime-only, not synced to file
            
            if (changed)
            {
                isDirty = true;
            }
        }
        
        /// <summary>
        /// Get formatted info string for developer mode display
        /// </summary>
        public string GetInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.AppendLine("=== GAME SETTINGS INFO ===");
            sb.AppendLine($"Current Score: {CurrentScore}");
            sb.AppendLine($"Current Level: {CurrentLevel}");
            sb.AppendLine($"Max Level: {MaxLevel}");
            sb.AppendLine($"Max Score: {MaxScore}");
            sb.AppendLine();
            sb.AppendLine("Level Scores:");
            for (int i = 0; i < 10; i++)
            {
                if (LevelScores[i] > 0)
                    sb.AppendLine($"  Level {i + 1}: {LevelScores[i]}");
            }
            sb.AppendLine();
            sb.AppendLine("Settings:");
            sb.AppendLine($"  Sound Effects: {(SoundEffectsEnabled ? "ON" : "OFF")}");
            sb.AppendLine($"  Music: {(MusicEnabled ? "ON" : "OFF")}");
            sb.AppendLine($"  Menu Sounds: {(MenuSoundsEnabled ? "ON" : "OFF")}");
            sb.AppendLine($"  Auto Attack: {(AutoAttack ? "ON" : "OFF")}");
            sb.AppendLine($"  Full Screen: {(IsFullScreen ? "ON" : "OFF")}");
            sb.AppendLine($"  Developer Mode (Runtime): {(Globals.DeveloperMode ? "ON" : "OFF")}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Test file I/O operations
        /// </summary>
        public bool TestFileIO()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[GameSettings] Starting File I/O Test...");
                
                // Backup current values
                int backupScore = CurrentScore;
                int backupLevel = CurrentLevel;
                
                // Write test values
                CurrentScore = 99999;
                CurrentLevel = 5;
                isDirty = true;
                Save();
                
                System.Diagnostics.Debug.WriteLine("[GameSettings] Test values written");
                
                // Clear instance to force reload
                instance = null;
                
                // Reload
                var reloaded = GameSettings.Instance;
                
                // Verify
                bool success = (reloaded.CurrentScore == 99999 && reloaded.CurrentLevel == 5);
                
                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("[GameSettings] File I/O Test PASSED");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[GameSettings] File I/O Test FAILED");
                }
                
                // Restore original values
                CurrentScore = backupScore;
                CurrentLevel = backupLevel;
                isDirty = true;
                Save();
                
                return success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] File I/O Test ERROR: {ex.Message}");
                return false;
            }
        }
        public void FactoryReset()
        {
            try
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();

                if (storage.FileExists("game.save"))
                {
                    storage.DeleteFile("game.save");
                }

                if (storage.FileExists("lock.info"))
                {
                    storage.DeleteFile("lock.info");
                }

                // Reset in-memory values
                Reset();

                System.Diagnostics.Debug.WriteLine("[GameSettings] FACTORY RESET COMPLETED - Files deleted.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Factory Reset Error: {ex.Message}");
            }
        }
    }
}
