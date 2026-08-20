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

        protected int selectedEntry = 0;

        List<Vector2> position = new List<Vector2>();

        public int SelectedEntry
        {
            get { return selectedEntry; }
        }

        // menuSelectSound ve menuUporDownSound artık SoundManager ile yönetiliyor

        DroppingLightSystem droppingLightSystem;

        protected virtual bool UseInvertedMenuButtons => false;

        public override void Initialize()
        {
            Load();

            screenTitle = "Menu Screen";
            Globals.Window.Title = screenTitle;
            Globals.ShowCursor = true;
            Globals.Game.IsMouseVisible = true;

            screenState = ScreenState.Active;

            inputState = new InputState();

            droppingLightSystem = new DroppingLightSystem();

        }

        public override void Load()
        {
            contentManager = Globals.Content;
            menuFont = Globals.MenuFont;

            // Pre-load menu sounds
            SoundManager.Load("menu-back", "Sounds/SoundEffects/menu-back");
            SoundManager.Load("menu-click", "Sounds/SoundEffects/menu-click");
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
                    position[i] = new Vector2(150, 80);
                }
                else 
                {
                    position[i] = new Vector2(150, 80 + (i * 82.5f));
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
                menuEntries[i].Draw(new Vector2(position[i].X, position[i].Y), isSelected, UseInvertedMenuButtons);
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

            if (!ScreenManager.IsTopActiveMenu(this))
                return;

            inputState.Update();



            if(inputState.Cancel)
            {
                MenuCancel(menuEntries.Count > 0 ? menuEntries[SelectedEntry].entryNumber : -1);
            }

            if (menuEntries.Count > 0)
            {
                if (inputState.MenuSelect)
                {
                    SoundManager.Play("menu-back");
                    MenuSelect(menuEntries[SelectedEntry].entryNumber);
                }

                if (inputState.MenuLeft) 
                {
                    MenuLeft(menuEntries[SelectedEntry].entryNumber);
                }

                if(inputState.MenuRight)
                {
                    MenuRight(menuEntries[SelectedEntry].entryNumber);
                }

                if(inputState.MenuUp)
                {
                    selectedEntry--;

                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                    SoundManager.Play("menu-click");
                }

                if(inputState.MenuDown)
                {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                    SoundManager.Play("menu-click");
                }
            }


            for (int i = 0; i < menuEntries.Count; i++)
            {
                if(menuEntries[i].clickableArea!=null)
                {
                    if(menuEntries[i].clickableArea.IsClicked)
                    {
                        SoundManager.Play("menu-back");
                        MenuSelect(menuEntries[i].entryNumber);
                    }

                    if (menuEntries[i].clickableArea.IsOverlapped)
                    {
                        if (selectedEntry != i)
                            SoundManager.Play("menu-click");

                        selectedEntry = i;
                    }
                }
            }
        }

        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            
            // Clean up this screen's clickable areas
            CleanupClickableAreas();
            
            GC.ReRegisterForFinalize(this);
        }

        public void AddEntry(MenuEntry menuEntry) 
        {
            menuEntries.Add(menuEntry);
            position.Add(new Vector2());
            
            // Register clickable area with this screen
            if (menuEntry.clickableArea != null)
            {
                menuEntry.clickableArea.OwnerScreen = this;
                RegisterClickableArea(menuEntry.clickableArea);
            }
        }

        public virtual void MenuSelect(int selectedEntry) {}

        public virtual void MenuCancel(int selectedEntry) { SoundManager.Play("menu-back"); }

        public virtual void MenuLeft(int selectedEntry) { }

        public virtual void MenuRight(int selectedEntry) { }
    }
}
