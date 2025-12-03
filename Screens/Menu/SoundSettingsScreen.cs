using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PaintTrek
{
    /// <summary>
    /// Sound Settings Screen - allows fine-grained control over all sound types
    /// </summary>
    class SoundSettingsScreen : MenuScreen
    {
        private Texture2D pixel;
        private Type callerType;
        private GameBoard gameBoard;

        public SoundSettingsScreen(Type callerType = null, GameBoard gameBoard = null)
        {
            this.callerType = callerType;
            this.gameBoard = gameBoard;
            Initialize();
            LoadMenuEntries();
        }

        private void LoadMenuEntries()
        {
            if (MenuEntries.Count > 0)
                MenuEntries.Clear();

            // Sound Effects
            if (Globals.SoundEffectsEnabled)
                AddEntry(new MenuEntry("Sound Effects : On", true, 0));
            else 
                AddEntry(new MenuEntry("Sound Effects : Off", true, 0));

            // Musics
            if (Globals.MusicsEnabled)
                AddEntry(new MenuEntry("Musics : On", true, 1));
            else 
                AddEntry(new MenuEntry("Musics : Off", true, 1));

            // Menu Sounds
            if (Globals.MenuSoundsEnabled)
                AddEntry(new MenuEntry("Menu Sounds : On", true, 2));
            else 
                AddEntry(new MenuEntry("Menu Sounds : Off", true, 2));

            AddEntry(new MenuEntry("Back", true, 3));
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Sound Settings";
            Globals.Window.Title = screenTitle;
        }

        public override void Load()
        {
            base.Load();
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            if (callerType == typeof(PauseScreen) && pixel != null)
            {
                // Fade effect for pause screen
                Fade();
            }
            else
            {
                Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            }
            base.Draw();
        }

        public void Fade()
        {
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(pixel, new Rectangle(0, 0, Globals.Graphics.GraphicsDevice.Viewport.Width, Globals.Graphics.GraphicsDevice.Viewport.Height),
                new Color((byte)0, (byte)0, (byte)0, (byte)(0.5 * 255)));
            Globals.SpriteBatch.End();
        }

        public override void MenuSelect(int selectedEntry)
        {
            switch (SelectedEntry)
            {
                case 0:
                    // Sound Effects toggle
                    Globals.SoundEffectsEnabled = !Globals.SoundEffectsEnabled;
                    GameSettings.Instance.UpdateSettings(soundEffects: Globals.SoundEffectsEnabled);
                    ApplySoundSettings();
                    LoadMenuEntries();
                    break;

                case 1:
                    // Musics toggle
                    Globals.MusicsEnabled = !Globals.MusicsEnabled;
                    GameSettings.Instance.UpdateSettings(music: Globals.MusicsEnabled);
                    ApplySoundSettings();
                    LoadMenuEntries();
                    break;

                case 2:
                    // Menu Sounds toggle
                    Globals.MenuSoundsEnabled = !Globals.MenuSoundsEnabled;
                    GameSettings.Instance.UpdateSettings(menuSounds: Globals.MenuSoundsEnabled);
                    ApplySoundSettings();
                    LoadMenuEntries();
                    break;

                case 3:
                    MenuCancel(SelectedEntry);
                    break;

                default:
                    break;
            }

            base.MenuSelect(selectedEntry);
        }

        private void ApplySoundSettings()
        {
            // Anında ses ayarlarını uygula
            SoundManager.ApplySoundSettings();
        }

        public override void MenuLeft(int selectedEntry)
        {
            if (selectedEntry == 0)
            {
                Globals.SoundEffectsEnabled = !Globals.SoundEffectsEnabled;
                GameSettings.Instance.UpdateSettings(soundEffects: Globals.SoundEffectsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
            else if (selectedEntry == 1)
            {
                Globals.MusicsEnabled = !Globals.MusicsEnabled;
                GameSettings.Instance.UpdateSettings(music: Globals.MusicsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
            else if (selectedEntry == 2)
            {
                Globals.MenuSoundsEnabled = !Globals.MenuSoundsEnabled;
                GameSettings.Instance.UpdateSettings(menuSounds: Globals.MenuSoundsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
        }

        public override void MenuRight(int selectedEntry)
        {
            if (selectedEntry == 0)
            {
                Globals.SoundEffectsEnabled = !Globals.SoundEffectsEnabled;
                GameSettings.Instance.UpdateSettings(soundEffects: Globals.SoundEffectsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
            else if (selectedEntry == 1)
            {
                Globals.MusicsEnabled = !Globals.MusicsEnabled;
                GameSettings.Instance.UpdateSettings(music: Globals.MusicsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
            else if (selectedEntry == 2)
            {
                Globals.MenuSoundsEnabled = !Globals.MenuSoundsEnabled;
                GameSettings.Instance.UpdateSettings(menuSounds: Globals.MenuSoundsEnabled);
                ApplySoundSettings();
                LoadMenuEntries();
            }
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            
            // Return to the screen that opened this one
            if (callerType == typeof(PauseScreen) && gameBoard != null)
            {
                ScreenManager.AddScreen(new PauseScreen(gameBoard));
            }
            else
            {
                ScreenManager.AddScreen(new OptionsScreen());
            }
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
        }
    }
}
