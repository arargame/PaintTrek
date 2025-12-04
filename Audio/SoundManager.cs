using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace PaintTrek
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        public static void Load(string name, string path)
        {
            if (!soundEffects.ContainsKey(name))
            {
                try
                {
                    soundEffects[name] = Globals.Content.Load<SoundEffect>(path);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load sound: " + path + " Error: " + e.Message);
                }
            }
        }

        public static void Play(string name, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            // Check if this is a menu sound
            bool isMenuSound = (name == "menu-back" || name == "menu-click");
            
            if (isMenuSound)
            {
                // Menu sounds require MenuSoundsEnabled
                if (Globals.MenuSoundsEnabled && soundEffects.ContainsKey(name))
                {
                    soundEffects[name].Play(volume, pitch, pan);
                }
            }
            else
            {
                // Game sound effects require SoundEffectsEnabled
                if (Globals.SoundEffectsEnabled && soundEffects.ContainsKey(name))
                {
                    soundEffects[name].Play(volume, pitch, pan);
                }
            }
        }

        public static void Unload()
        {
            soundEffects.Clear();
        }
        
        /// <summary>
        /// Apply sound settings immediately (called when settings change)
        /// </summary>
        public static void ApplySoundSettings()
        {
            // Müzik kontrolü - MediaPlayer üzerinden
            try
            {
                var mediaState = Microsoft.Xna.Framework.Media.MediaPlayer.State;
                
                if (Globals.MusicsEnabled)
                {
                    // Müzik açıldı - eğer duraklıysa devam et
                    if (mediaState == Microsoft.Xna.Framework.Media.MediaState.Paused)
                    {
                        Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
                        System.Diagnostics.Debug.WriteLine("[SoundManager] Music resumed");
                    }
                    else if (mediaState == Microsoft.Xna.Framework.Media.MediaState.Stopped)
                    {
                        // Müzik durmuşsa, kullanıcı tekrar başlatmalı veya GameBoard otomatik başlatır
                        System.Diagnostics.Debug.WriteLine("[SoundManager] Music is stopped, needs manual restart");
                    }
                }
                else
                {
                    // Müzik kapatıldı - çalıyorsa veya duraklıysa durdur
                    if (mediaState == Microsoft.Xna.Framework.Media.MediaState.Playing)
                    {
                        Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
                        System.Diagnostics.Debug.WriteLine("[SoundManager] Music paused");
                    }
                    else if (mediaState == Microsoft.Xna.Framework.Media.MediaState.Paused)
                    {
                        // Zaten duraklı, hiçbir şey yapma
                        System.Diagnostics.Debug.WriteLine("[SoundManager] Music already paused");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SoundManager] Error applying music settings: {ex.Message}");
            }
            
            System.Diagnostics.Debug.WriteLine($"[SoundManager] Settings applied - SFX: {Globals.SoundEffectsEnabled}, Music: {Globals.MusicsEnabled}, Menu: {Globals.MenuSoundsEnabled}");
        }
    }
}
