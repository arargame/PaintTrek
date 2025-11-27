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
            if (Globals.GameSoundsActivated && soundEffects.ContainsKey(name))
            {
                // Fire and forget play (uses internal pool)
                soundEffects[name].Play(volume, pitch, pan);
            }
        }

        public static void Unload()
        {
            soundEffects.Clear();
        }
    }
}
