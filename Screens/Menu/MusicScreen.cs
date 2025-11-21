using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PaintTrek
{
    class MusicScreen:MenuScreen
    {
        BackButton backButton;

       // List<SoundSystem> musics;

        MediaPlayerSystem mpSystem;

        int counter;
        bool repeat;
        Rectangle pictureRect;
        Vector2 floatingWordsPosition;
        Vector2 titlePosition;
        Vector2 buttonPosition;
        string title;
        string floatingWord;
        int activeMusicNumber;
        List<Texture2D> buttons;
        Color[] buttonColors;
        bool next, previous;
        ClickableArea[] clickableAreas;
        MediaState state;
        Texture2D transitionTexture;

        public MusicScreen() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            mpSystem = new MediaPlayerSystem();
            base.Initialize();

            backButton = new BackButton("Back", this, true);

            screenTitle = "Music Screen";
            Globals.Window.Title = screenTitle;

            counter = 0;
            repeat = false;
            pictureRect = new Rectangle(0, Globals.GameRect.Center.Y / 2, (int)Globals.GameSize.X, (int)Globals.GameSize.Y / 3);
            floatingWordsPosition = new Vector2(Globals.GameSize.X / 2, pictureRect.Bottom + 20);
            floatingWord = "";
            activeMusicNumber = 0;
            SetTitle();
            SetButtons();
            next = false; previous = false;
            titlePosition = new Vector2(Globals.GameFont.MeasureString(title).X / 2, pictureRect.Top - Globals.GameFont.MeasureString(title).Y - 20);
            buttonColors = new Color[4];
            buttonPosition = new Vector2(Globals.GameSize.X / 2 + Globals.GameSize.X / 4, titlePosition.Y);

            for (int i = 0; i < buttonColors.Length; i++)
            {
                buttonColors[i] = new Color();
                buttonColors[i] = Color.White;
            }
            clickableAreas = new ClickableArea[4];
            for (int i = 0; i < clickableAreas.Length; i++)
            {
                clickableAreas[i] = new ClickableArea();
            }
        }

        public override void Load()
        {
            base.Load();
            LoadTexture();
            LoadMusic();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
            backButton.Update();

            floatingWordsPosition.X++;

            if (floatingWordsPosition.X > Globals.GameSize.X)
                floatingWordsPosition.X = -Globals.GameFont.MeasureString(floatingWord).X;

            SetTitle();

            //if(musics.Count>0)
            //state = musics[activeMusicNumber].soundEffectInstance.State;
            state = mpSystem.GetState();

            if (state.ToString() =="Paused" )
                floatingWord = "Paused";
            else if (state.ToString() == "Playing")
                floatingWord = "Playing";
            else floatingWord = "Stopped";

            ControlColors();

        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            backButton.Draw();
            base.Draw();



            Vector2[] buttonPositions = new Vector2[4];
            for (int i = 0; i < buttonPositions.Length; i++)
            {
                if (i == 0) buttonPositions[i] = buttonPosition;
                else
                    buttonPositions[i] = new Vector2(buttonPosition.X + (30 * i), buttonPosition.Y);
            }
            for (int i = 0; i < clickableAreas.Length; i++)
            {
                clickableAreas[i].SetRect(new Rectangle((int)buttonPositions[i].X, (int)buttonPositions[i].Y, buttons[i].Width, buttons[i].Height));
            }
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.DrawString(Globals.GameFont, title, titlePosition, Color.White);
            for (int i = 0; i < buttonPositions.Length; i++)
            {
                Globals.SpriteBatch.Draw(buttons[i], buttonPositions[i], buttonColors[i]);
            }
            Globals.SpriteBatch.Draw(transitionTexture, pictureRect, Color.White);
            Globals.SpriteBatch.DrawString(Globals.GameFont, floatingWord, floatingWordsPosition, Color.White);
            Globals.SpriteBatch.End();

        }

        public override void HandleInput()
        {
            base.HandleInput();

            InputState input = inputState;
            if (input.currentKeyboardState.IsKeyDown(Keys.Right))
            {
                next = true;
            }
            else next = false;

            if (input.currentKeyboardState.IsKeyDown(Keys.Left))
            {
                previous = true;
            }
            else previous = false;


            for (int i = 0; i < clickableAreas.Length; i++)
            {
                if (clickableAreas[i].IsClicked && clickableAreas[i].IsOverlapped)
                {

                    if (i == 0)
                    {
                        MenuLeft(SelectedEntry);
                    }
                    else if (i == 1)
                    {
                        MenuSelect(SelectedEntry);
                    }
                    else if (i == 2)
                    {
                        MenuSelect(SelectedEntry);
                    }
                    else if (i == 3)
                    {
                        MenuRight(SelectedEntry);
                    }
                }

                if (clickableAreas[i].IsOverlapped)
                {

                    if (i == 0) previous = true;
                    else previous = false;

                    if (i == 3) next = true;
                    else next = false;
                }
            }
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
            ScreenManager.AddScreen(new ExtraScreen());
        }
        public override void MenuCancel(int selectedEntry)
        {
            Stop();
            base.MenuCancel(selectedEntry);
            ExitScreen();
        }

        public override void MenuLeft(int selectedEntry)
        {
            counter--;
            if (counter < 0)
                counter = mpSystem.GetSongCount() - 1;
        }

        public override void MenuRight(int selectedEntry)
        {
            counter++;
            if (counter > mpSystem.GetSongCount() - 1)
                counter = 0;
        }

        public override void MenuSelect(int selectedEntry)
        {
            if (activeMusicNumber != counter)
            {
                if (state.ToString() == "Playing" || state.ToString() == "Paused")
                    Pause();
                mpSystem.ActiveSongIndex=activeMusicNumber = counter;
                Play();
            }
            else
            {
                if (state.ToString() == "Stopped")
                    Play();
                else if (state.ToString() == "Playing")
                    Pause();
                else Resume();
            }
            LoadTexture();
        }

        private void LoadMusic()
        {
           /* musics = new List<SoundSystem>();
            musics.Add(new SoundSystem("Sounds/Musics/music1", 1f, 0f, 0f, repeat, "Sahin Meric", "Las Calles"));
            musics.Add(new SoundSystem("Sounds/Musics/music2", 1f, 0f, 0f, repeat, "Sahin Meric", "UFO"));
            musics.Add(new SoundSystem("Sounds/Musics/music3", 1f, 0f, 0f, repeat, "Sahin Meric", "Funk My Hearth "));
            musics.Add(new SoundSystem("Sounds/Musics/music4", 1f, 0f, 0f, repeat, "Sahin Meric", "Clubber"));
            musics.Add(new SoundSystem("Sounds/Musics/music5", 1f, 0f, 0f, repeat, "Sahin Meric", "Optimism"));
            musics.Add(new SoundSystem("Sounds/Musics/music6", 1f, 0f, 0f, repeat, "Sahin Meric", "Kovalamaca"));
            musics.Add(new SoundSystem("Sounds/Musics/music7", 1f, 0f, 0f, repeat, "Sahin Meric", "My Summer Hits 2014"));
            musics.Add(new SoundSystem("Sounds/Musics/music8", 1f, 0f, 0f, repeat, "Sahin Meric", "Sahur Vakti"));
            musics.Add(new SoundSystem("Sounds/Musics/music9", 1f, 0f, 0f, repeat, "Sahin Meric", "Transition"));
            musics.Add(new SoundSystem("Sounds/Musics/music10", 1f, 0f, 0f, repeat, "Sahin Meric", "Just For Me"));*/

            mpSystem.AddSong("Sounds/Musics/music11",repeat, "Sahin Meric", "Las Calles");
            mpSystem.AddSong("Sounds/Musics/music12", repeat, "Sahin Meric", "UFO");
            mpSystem.AddSong("Sounds/Musics/music13", repeat, "Sahin Meric", "Funk My Hearth ");
            mpSystem.AddSong("Sounds/Musics/music14", repeat, "Sahin Meric", "Clubber");
            mpSystem.AddSong("Sounds/Musics/music15", repeat, "Sahin Meric", "Optimism");
            mpSystem.AddSong("Sounds/Musics/music16", repeat, "Sahin Meric", "Kovalamaca");
            mpSystem.AddSong("Sounds/Musics/music17", repeat, "Sahin Meric", "My Summer Hits 2014");
            mpSystem.AddSong("Sounds/Musics/music18", repeat, "Sahin Meric", "Sahur Vakti");
            mpSystem.AddSong("Sounds/Musics/music19", repeat, "Sahin Meric", "Transition");
            mpSystem.AddSong("Sounds/Musics/music20", repeat, "Sahin Meric", "Just For Me");
        }

        private void LoadTexture() 
        {
            int x=counter+1;
            if (x <= 0)
                x=1;
            if (x > 10) x = 10;
            transitionTexture = Globals.Content.Load<Texture2D>("LoadingScene/transitionTexture"+x);
        }

        private void ControlColors()
        {
            if (state.ToString() =="Paused")
                buttonColors[2] = Color.CornflowerBlue;
            else buttonColors[2] = Color.White;

            if (state.ToString() == "Playing")
                buttonColors[1] = Color.CornflowerBlue;
            else buttonColors[1] = Color.White;

            if (next)
                buttonColors[3] = Color.CornflowerBlue;
            else buttonColors[3] = Color.White;

            if (previous)
                buttonColors[0] = Color.CornflowerBlue;
            else buttonColors[0] = Color.White;
        }

        private void SetTitle()
        {
            //if(musics.Count>0)
            title = mpSystem.GetInfo(counter)[0] + "/" + mpSystem.GetInfo(counter)[1];
        }
        private void SetButtons()
        {
            buttons = new List<Texture2D>();

            buttons.Add(Globals.Content.Load<Texture2D>("Textures/previousButton"));
            buttons.Add(Globals.Content.Load<Texture2D>("Textures/playButton"));
            buttons.Add(Globals.Content.Load<Texture2D>("Textures/pauseButton"));
            buttons.Add(Globals.Content.Load<Texture2D>("Textures/nextButton"));
        }

        #region SoundSettings
        private void Play()
        {
            //musics[activeMusicNumber].Play();
            mpSystem.Play();
        }

        private void Stop()
        {
            //musics[activeMusicNumber].Stop();
            mpSystem.Stop();
        }

        private void Pause()
        {
            //musics[activeMusicNumber].Pause();
            mpSystem.Pause();
        }

        private void Resume()
        {
            //musics[activeMusicNumber].Resume();
            mpSystem.Resume();
        }
        #endregion
    }
}
