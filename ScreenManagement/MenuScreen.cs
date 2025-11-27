using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PaintTrek
{
    abstract class MenuScreen:Screen
    {
        protected SpriteFont menuFont;

        List<MenuEntry> menuEntries=new List<MenuEntry>();

        public List<MenuEntry> MenuEntries 
        {
            get { return menuEntries; }
        }

        int selectedEntry = 0;

        List<Vector2> position = new List<Vector2>();

        public int SelectedEntry
        {
            get { return selectedEntry; }
        }

        SoundSystem menuSelectSound;
        SoundSystem menuUporDownSound;

        DroppingLightSystem droppingLightSystem;

        public override void Initialize()
        {
            Load();

            screenTitle = "Menu Screen";
            Globals.Window.Title = screenTitle;
            Globals.Game.IsMouseVisible = true;

            screenState = ScreenState.Active;

            inputState = new InputState();

            droppingLightSystem = new DroppingLightSystem();

        }

        public override void Load()
        {
            contentManager = Globals.Content;
            menuFont = Globals.MenuFont;

            menuSelectSound= new SoundSystem("Sounds/SoundEffects/menu-back", 1f, 0f, 0f, false, "Unknown", "Unknown");
            menuUporDownSound = new SoundSystem("Sounds/SoundEffects/menu-click", 1f, 0f, 0f, false, "Unknown", "Unknown");
        }

        public override void UnloadContent()
        {
            foreach (var entry in menuEntries)
            {
                entry.Dispose();
            }
        }

        public override void Update()
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                if (i == 0)
                {
                    position[i] = new Vector2(100,200);
                }
                else 
                {
                    position[i] = new Vector2(100,200+(i*30));
                }
            }

            if(inputActivated)
            HandleInput();

            timeToActivateInput += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if (timeToActivateInput > 0.5)
                inputActivated = true;

            droppingLightSystem.Update();

           // if (!Globals.GameSoundsActivated)
             //   menuSong.Stop();
        }

        public override void Draw()
        {
            droppingLightSystem.Draw();
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = (i == selectedEntry);
                menuEntries[i].Draw(new Vector2(position[i].X,position[i].Y),isSelected);
            }

        }

        public override ScreenState GetScreenState()
        {
            return screenState;
        }

        public override void HandleInput()
        {

            if (screenState != ScreenState.Active || inputState==null)
             return;

            inputState.Update();



            if(inputState.Cancel)
            {
                MenuCancel(SelectedEntry);
            }

            if (inputState.MenuSelect)
            {
                MenuSelect(SelectedEntry);
            }

            if (inputState.MenuLeft) 
            {
                MenuLeft(SelectedEntry);
            }

            if(inputState.MenuRight)
            {
                MenuRight(SelectedEntry);
            }

            if(inputState.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
                menuUporDownSound.Play();
            }

            if(inputState.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
                menuUporDownSound.Play();
            }


            for (int i = 0; i < menuEntries.Count; i++)
            {
                if(menuEntries[i].clickableArea!=null)
                {
                    if(menuEntries[i].clickableArea.IsClicked)
                    {
                        MenuSelect(menuEntries[i].entryNumber);
                    }

                    if (menuEntries[i].clickableArea.IsOverlapped)
                    {
                        if (selectedEntry != menuEntries[i].entryNumber)
                            menuUporDownSound.Play();

                        selectedEntry = menuEntries[i].entryNumber;
                    }
                }
            }
        }

        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            GC.ReRegisterForFinalize(this);
        }

        public void AddEntry(MenuEntry menuEntry) 
        {
            menuEntries.Add(menuEntry);
            position.Add(new Vector2());
        }

        public virtual void MenuSelect(int selectedEntry) {}

        public virtual void MenuCancel(int selectedEntry) { menuSelectSound.Play(); }

        public virtual void MenuLeft(int selectedEntry) { }

        public virtual void MenuRight(int selectedEntry) { }
    }
}
