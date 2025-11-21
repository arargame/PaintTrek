using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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
            get { return IsNewKeyPress(Keys.Left); }
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

        public void Update() 
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            gamePadState = GamePad.GetState(PlayerIndex.One);

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            cursorPosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            cursorRect = new Rectangle((int)cursorPosition.X, (int)cursorPosition.Y, 10, 10);

            for (int i = 0; i < ClickableAreaSystem.clickableAreas.Count; i++)
            {
                ClickableArea CA = ClickableAreaSystem.clickableAreas[i];
                CA.Update();
                if (CA.GetRect().Intersects(cursorRect))
                {
                    CA.IsOverlapped = true;
                }
                else CA.IsOverlapped = false;

                if (CA.GetRect().Intersects(cursorRect) && IsLeftClicked())
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
            return gamePadState.Buttons.Back == ButtonState.Pressed;
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
