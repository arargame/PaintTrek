using System;
using System.IO;

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
                        System.Diagnostics.Debug.WriteLine("[GameSettings] 🆕 Creating new instance (defaults only, call Load() explicitly)");
                        instance = new GameSettings();
                        // DON'T call Load() here - ApplicationData.Current not ready yet!
                        // Load() must be called explicitly from LoadContent()
                    }
                    return instance;
                }
            }
        }
        
        public void Load()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            Stream stream = null;
            BinaryReader reader = null;
            
            try
            {
                // Use standard file system instead of ApplicationData.Current (timing issues)
                string localFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PaintTrek");
                
                // Ensure directory exists
                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);
                
                System.Diagnostics.Debug.WriteLine($"[GameSettings] LocalFolder: {localFolder}");
                
                // Check if file exists
                string filePath = Path.Combine(localFolder, "game.save");
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine("[GameSettings] ⚠️ Save file not found, using defaults");
                    System.Diagnostics.Debug.WriteLine($"[GameSettings] Expected location: {filePath}");
                    isDirty = false;
                    return;
                }
                
                stream = File.OpenRead(filePath);
                reader = new BinaryReader(stream);
                
                System.Diagnostics.Debug.WriteLine($"[GameSettings] File size: {stream.Length} bytes");
                
                CurrentScore = reader.ReadInt32();
                CurrentLevel = reader.ReadInt32();
                MaxLevel = reader.ReadInt32();
                MaxScore = reader.ReadInt32();
                
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Progress: Score={CurrentScore}, Level={CurrentLevel}, MaxLevel={MaxLevel}");
                
                for (int i = 0; i < 10; i++)
                {
                    try { LevelScores[i] = reader.ReadInt32(); }
                    catch (Exception ex) 
                    { 
                        LevelScores[i] = 0;
                        System.Diagnostics.Debug.WriteLine($"[GameSettings] ⚠️ Error reading LevelScores[{i}]: {ex.Message}");
                    }
                }
                
                try
                {
                    SoundEffectsEnabled = reader.ReadBoolean();
                    MusicEnabled = reader.ReadBoolean();
                    MenuSoundsEnabled = reader.ReadBoolean();
                    AutoAttack = reader.ReadBoolean();
                    IsFullScreen = reader.ReadBoolean();
                    
                    System.Diagnostics.Debug.WriteLine($"[GameSettings] Settings: Sound={SoundEffectsEnabled}, Music={MusicEnabled}, Menu={MenuSoundsEnabled}, AutoAttack={AutoAttack}, FullScreen={IsFullScreen}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GameSettings] ⚠️ Error reading settings (using defaults): {ex.Message}");
                }
                
                isDirty = false;
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[GameSettings] ✅ Loaded successfully in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] ❌ Load error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Stack trace: {ex.StackTrace}");
            }
            finally
            {
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
            }
        }
        
        public void Save()
        {
            // Always save when explicitly called (removed isDirty check for debugging)
            if (!isDirty)
            {
                System.Diagnostics.Debug.WriteLine("[GameSettings] ⚠️ Save called but not dirty - saving anyway for safety");
            }
            
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            Stream stream = null;
            BinaryWriter writer = null;
            
            try
            {
                // Use standard file system
                string localFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PaintTrek");
                
                // Ensure directory exists
                if (!Directory.Exists(localFolder))
                    Directory.CreateDirectory(localFolder);
                
                string filePath = Path.Combine(localFolder, "game.save");
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Saving to: {filePath}");
                
                stream = File.Create(filePath);
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
                
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Data: Score={CurrentScore}, Level={CurrentLevel}, Sound={SoundEffectsEnabled}, Music={MusicEnabled}, AutoAttack={AutoAttack}, FullScreen={IsFullScreen}");
                
                isDirty = false;
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[GameSettings] ✅ Saved successfully in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] ❌ Save error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Stack trace: {ex.StackTrace}");
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
            System.Diagnostics.Debug.WriteLine($"[GameSettings] 🔧 UpdateSettings called: Sound={soundEffects}, Music={music}, Menu={menuSounds}, Auto={autoAttack}, Full={fullScreen}");
            
            bool wasChanged = false;
            
            if (soundEffects.HasValue && SoundEffectsEnabled != soundEffects.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] SoundEffects: {SoundEffectsEnabled} → {soundEffects.Value}");
                SoundEffectsEnabled = soundEffects.Value;
                isDirty = true;
                wasChanged = true;
            }
            
            if (music.HasValue && MusicEnabled != music.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] Music: {MusicEnabled} → {music.Value}");
                MusicEnabled = music.Value;
                isDirty = true;
                wasChanged = true;
            }
            
            if (menuSounds.HasValue && MenuSoundsEnabled != menuSounds.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] MenuSounds: {MenuSoundsEnabled} → {menuSounds.Value}");
                MenuSoundsEnabled = menuSounds.Value;
                isDirty = true;
                wasChanged = true;
            }
            
            if (autoAttack.HasValue && AutoAttack != autoAttack.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] AutoAttack: {AutoAttack} → {autoAttack.Value}");
                AutoAttack = autoAttack.Value;
                isDirty = true;
                wasChanged = true;
            }
            
            if (fullScreen.HasValue && IsFullScreen != fullScreen.Value)
            {
                System.Diagnostics.Debug.WriteLine($"[GameSettings] FullScreen: {IsFullScreen} → {fullScreen.Value}");
                IsFullScreen = fullScreen.Value;
                isDirty = true;
                wasChanged = true;
            }
            
            if (wasChanged)
            {
                System.Diagnostics.Debug.WriteLine("[GameSettings] 💾 Calling Save()...");
                Save();
                System.Diagnostics.Debug.WriteLine("[GameSettings] ✅ Settings updated and saved");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[GameSettings] ⏭️ No changes detected");
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
                string localFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "PaintTrek");
                
                string gameSavePath = Path.Combine(localFolder, "game.save");
                if (File.Exists(gameSavePath))
                {
                    File.Delete(gameSavePath);
                }

                string lockInfoPath = Path.Combine(localFolder, "lock.info");
                if (File.Exists(lockInfoPath))
                {
                    File.Delete(lockInfoPath);
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
