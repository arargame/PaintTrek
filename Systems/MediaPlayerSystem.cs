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
            songList.Add(Globals.Content.Load<Song>(songName));
            Loop(IsLooped);
            this.artist = artist;
            this.title = title;
            songCount = songList.Count;
            info.Add(new string[]{artist,title});
        }

        public void Play()
        {
            if (Globals.GameSoundsActivated && MediaPlayer.GameHasControl)
                for (int i = 0; i < songList.Count; i++)
                {
                    if(i==activeSongIndex)
                    MediaPlayer.Play(songList[i]);   
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
