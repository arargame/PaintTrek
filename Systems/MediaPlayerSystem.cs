using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace PaintTrek
{
    class MediaPlayerSystem
    {
        List <Song> songList;
        string artist;
        string title;
        int activeSongIndex;
        int songCount;
        List <string[]> info;

        public int ActiveSongIndex 
        {
            set { activeSongIndex = value; }
            get { return activeSongIndex; }
        }

        public MediaPlayerSystem() 
        {
            info = new List<string[]>();
            songList = new List<Song>();
            activeSongIndex = 0;
        }
        public void AddSong(string songName, bool IsLooped, string artist, string title) 
        {
            try
            {
                songList.Add(Globals.Content.Load<Song>(songName));
                Loop(IsLooped);
                this.artist = artist;
                this.title = title;
                songCount = songList.Count;
                info.Add(new string[]{artist,title});
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MediaPlayerSystem] Failed to load song '{songName}': {ex.Message}");
            }
        }

        public void Play()
        {
            // MediaPlayerSystem is for music, check MusicsEnabled
            if (Globals.MusicsEnabled && MediaPlayer.GameHasControl)
            {
                try
                {
                    MediaPlayer.Volume = 0.85f; // User Request: Lower music volume by 15% (to 0.85) on Desktop

                    for (int i = 0; i < songList.Count; i++)
                    {
                        if (i == activeSongIndex && songList[i] != null)
                            MediaPlayer.Play(songList[i]);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MediaPlayerSystem] Failed to play song: {ex.Message}");
                }
            }
        }

        public int GetSongCount() 
        {
            return songCount;
        }
        public MediaState GetState() 
        {
           return MediaPlayer.State;
        }

        public void Loop(bool IsLooped)
        {
            MediaPlayer.IsRepeating = IsLooped;
        }

        public void Stop()
        {
            MediaPlayer.Stop();
        }
        public void Pause()
        {
            MediaPlayer.Pause();
        }
        public void Resume()
        {
            MediaPlayer.Resume();
        }

        public string[] GetInfo(int counter)
        {
            for (int i = 0; i < songList.Count; i++)
            {
                if (counter == i)
                    return info[i];
            }

            return new string[] { "Artist", "Title" };
        }

        public void UnloadContent()
        {
            if (this.GetState().ToString() == "Playing")
                Stop();
        }
    }
}
