using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    abstract class GameScreen:Screen
    {
        protected SpriteFont gameFont;
        protected BackButton backButton;
        // SoundSystem menuSelectSound;
        

        public override void Initialize()
        {
            Load();

            screenTitle = "Game Screen";
            Globals.Window.Title = screenTitle;

            screenState = ScreenState.Active;
            inputState = new InputState();
            backButton = new BackButton("Back",this,false);
            
            // Register back button's clickable area
            if (backButton != null && backButton.clickableArea != null)
            {
                RegisterClickableArea(backButton.clickableArea);
            }
        }

        public override void Load()
        {
            contentManager = Globals.Content;
            gameFont = Globals.GameFont;
            // menuSelectSound = new SoundSystem("Sounds/SoundEffects/menu-back", 1f, 0f, 0f, false, "Unknown", "Unknown");
            SoundManager.Load("menu-back", "Sounds/SoundEffects/menu-back");
           
        }

        public override void UnloadContent()
        {
           
        }

        public override void Update()
        {
            if (inputActivated)
                HandleInput();

            timeToActivateInput += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if (timeToActivateInput > 0.5)
                inputActivated = true;

            backButton.Update();
        }

        public override void Draw()
        {
            backButton.Draw();
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

            if (this.inputState.Cancel)
            {
                ExitScreen();
            }
        }

        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            
            // Clean up this screen's clickable areas
            CleanupClickableAreas();
            
            GC.ReRegisterForFinalize(this);
            SoundManager.Play("menu-back");
        }



    }
}
