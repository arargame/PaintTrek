using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace PaintTrek
{
    class LevelSoundtrack
    {
        //SoundSystem levelSong;
        MediaPlayerSystem levelSong;

        public LevelSoundtrack()
        {
            LoadSounds();
        }
        ~LevelSoundtrack() 
        {
            UnloadContent();
        }

        public void LoadSounds()
        {
           //levelSong = new SoundSystem("Sounds/Musics/music" + Level.LevelCounter, 1f, 0f, 0f, true, "Sahin Meric", "Song" + Level.LevelCounter);
            levelSong = new MediaPlayerSystem();
            levelSong.AddSong("Sounds/Musics/music" + (Level.LevelCounter+10), true, "Sahin Meric", "Song" + Level.LevelCounter);
        }

        public void UnloadContent()
        {
            levelSong.UnloadContent();
        }

        public void Update()
        {
            if (levelSong != null)
            {
                // Check if music is enabled
                if (!Globals.MusicsEnabled)
                {
                    // Music disabled - pause if playing
                    if (levelSong.GetState().ToString() == "Playing")
                        levelSong.Pause();
                }
                else
                {
                    // Music enabled - play or resume
                    if (levelSong.GetState().ToString() == "Stopped")
                        levelSong.Play();
                    else if (levelSong.GetState().ToString() == "Paused")
                        levelSong.Resume();
                }
            }
        }

        public void Pause()
        {
            levelSong.Pause();
        }

        public void Dispose()
        {
            if (levelSong != null)
            {
                if (levelSong.GetState().ToString()=="Playing" ||levelSong.GetState().ToString()=="Paused")
                {
                    levelSong.Stop();
                }
            }
        }
    }
}
