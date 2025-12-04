using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace PaintTrek
{
    class SoundSystem
    {
        SoundEffect soundEffect;
        public SoundEffectInstance soundEffectInstance;

        string artist;
        string title;

        float volume = 1.0f;
        float pitch = 0.0f;
        float pan = 0.0f;

        public SoundSystem(string songName, float volume, float pitch, float pan, bool IsLooped, string artist, string title)
        {
            LoadSound(songName);
            this.volume = volume;
            this.pitch = pitch;
            this.pan = pan;
            soundEffectInstance = soundEffect.CreateInstance();
            Loop(IsLooped);
            this.artist = artist;
            this.title = title;
        }
        public SoundSystem(string songName,bool IsLooped) 
        {
            LoadSound(songName);
            soundEffectInstance = soundEffect.CreateInstance();
            Loop(IsLooped);
            this.artist = "Unknown";
            this.title = "Unknown";
        }

        ~SoundSystem() 
        {
           // UnloadContent();
        }

        public void LoadSound(string songName)
        {
            soundEffect = Globals.Content.Load<SoundEffect>(songName);
        }

        public void UnloadContent() 
        {
            if(soundEffectInstance.IsDisposed)return;

            if(soundEffectInstance.State==SoundState.Playing)
            {
                soundEffectInstance.Pause();
                soundEffectInstance.Dispose();
                soundEffect.Dispose();
            }
        }

        public void Play()
        {
            // SoundSystem is used for game sound effects, not music
            if (Globals.SoundEffectsEnabled)
                soundEffectInstance.Play();
        }

        public void Loop(bool IsLooped)
        {
            soundEffectInstance.IsLooped = IsLooped;
        }

        public void Stop()
        {
            soundEffectInstance.Stop();
        }
        public void Pause()
        {
            soundEffectInstance.Pause();
        }
        public void Resume()
        {
            soundEffectInstance.Resume();
        }

        public string[] GetInfo()
        {
            return new string[] { artist, title };
        }
    }
}
