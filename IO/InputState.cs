using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class InputState
    {

        public GamePadState gamePadState;

        public KeyboardState currentKeyboardState;
        public KeyboardState lastKeyboardState;

        public MouseState currentMouseState;
        public MouseState lastMouseState;
        public Vector2 cursorPosition;
        public Rectangle cursorRect;

        public bool MenuUp
        {
            get { return IsNewKeyPress(Keys.Up); }
        }
        public bool MenuDown
        {
            get { return IsNewKeyPress(Keys.Down); }
        }
        public bool MenuLeft
        {
            get { return IsNewKeyPress(Keys.Left) ; }
        }
        public bool MenuRight
        {
            get { return IsNewKeyPress(Keys.Right); }
        }

        public bool MenuSelect
        {
            get { return IsNewKeyPress(Keys.Enter); }
        }
        public bool Cancel
        {
            get { return IsNewKeyPress(Keys.Escape) || IsGamePadPress(Keys.Back); }
        }
        public bool PauseGame
        {
            get { return IsNewKeyPress(Keys.Escape); }
        }
        public bool MoveUp
        {
            get { return IsKeyPress(Keys.Up) || IsKeyPress(Keys.W); }
        }
        public bool MoveDown
        {
            get { return IsKeyPress(Keys.Down) || IsKeyPress(Keys.S); }
        }
        public bool MoveLeft
        {
            get { return IsKeyPress(Keys.Left) || IsKeyPress(Keys.A); }
        }
        public bool MoveRight
        {
            get { return IsKeyPress(Keys.Right) || IsKeyPress(Keys.D); }
        }
        public bool Fire
        {
            get { return IsKeyPress(Keys.Space) || IsKeyPress(Keys.K) || IsMouseLeftPressed(); }
        }

        public GamePadState lastGamePadState;

        public void Update() 
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            lastGamePadState = gamePadState;
            gamePadState = GamePad.GetState(PlayerIndex.One);

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            // Mouse is reported in back-buffer pixels. Convert it to the fixed game canvas so
            // hit-testing remains correct in fullscreen, windowed mode and after a live resize.
            ResolutionHelper.EnsureCurrent(Globals.Graphics.GraphicsDevice);
            cursorPosition = ResolutionHelper.ToVirtual(new Vector2(currentMouseState.X, currentMouseState.Y));
            cursorRect = new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, 10, 10);

            for (int i = 0; i < ClickableAreaSystem.clickableAreas.Count; i++)
            {
                ClickableArea CA = ClickableAreaSystem.clickableAreas[i];
                CA.Update();
                
                // Only process clickable areas that belong to active screens or have no owner
                bool shouldProcess = CA.OwnerScreen == null || CA.OwnerScreen.GetScreenState() == ScreenState.Active;
                
                if (shouldProcess && CA.GetRect().Intersects(cursorRect))
                {
                    CA.IsOverlapped = true;
                }
                else CA.IsOverlapped = false;

                if (shouldProcess && CA.GetRect().Intersects(cursorRect) && IsLeftClicked())
                {
                    CA.IsClicked = true;
                }
                else CA.IsClicked = false;
            }
        }

        public bool IsNewKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }
        public bool IsKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsGamePadPress(Keys key) 
        {
            // Legacy support
            return gamePadState.Buttons.Back == ButtonState.Pressed;
        }

        public bool IsNewButtonPress(Buttons button)
        {
            return gamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button);
        }

        public bool IsLeftClicked()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsMouseLeftPressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }
    }
}
