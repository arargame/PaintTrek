using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class ScreenManager
    {
        internal static List<Screen> screens;
        public InputState inputState;

        Texture2D cursorTexture;

        public ScreenManager()
        {
            screens = new List<Screen>();
            inputState = new InputState();
        }

        // Load cursor texture from content
        public void LoadContent()
        {
            // Load cursor texture from content pipeline
            cursorTexture = Globals.Content.Load<Texture2D>("cursorTexture");
        }

        public void Update() 
        {
            CheckScreenStatus();
            inputState.Update(); // Ensure input state is updated

            for (int i = 0; i < screens.Count; i++)
            {
                if(screens[i].GetScreenState()==ScreenState.Active)
                screens[i].Update();
            }
        }

        public void Draw() 
        {
            for (int i = 0; i < screens.Count; i++)
            {
                screens[i].Draw();
            }

            Globals.SpriteBatch.Begin();
            if (cursorTexture != null && Globals.ShowCursor)
            {
                // Calculate scale to make cursor appear at mouse cursor size (typically 16-32 pixels)
                // Target size is 32 pixels, so scale based on texture size
                float targetSize = 32f; // Target cursor size in pixels
                float scaleX = targetSize / cursorTexture.Width;
                float scaleY = targetSize / cursorTexture.Height;
                float scale = Math.Min(scaleX, scaleY); // Use smaller scale to maintain aspect ratio
                
                Vector2 origin = new Vector2(cursorTexture.Width / 2, cursorTexture.Height / 2);
                Globals.SpriteBatch.Draw(cursorTexture, inputState.cursorPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            Globals.SpriteBatch.End();
        }

        internal static void AddScreen(Screen screen) 
        {
            // Check if a screen of the same type already exists and is active
            Type screenType = screen.GetType();
            for (int i = screens.Count - 1; i >= 0; i--)
            {
                if (screens[i].GetType() == screenType && screens[i].GetScreenState() != ScreenState.Inactive)
                {
                    // Screen of this type already exists, don't add duplicate
                    return;
                }
            }
            
            screens.Add(screen);
        }

        internal static void RemoveScreen(Screen screen) 
        {
            screens.Remove(screen);
            screen = null;
            GC.Collect();
        }

        internal static void ClearScreens() 
        {
            screens.Clear();
        }

        public List<Screen> GetScreenList() 
        {
            return screens;
        }

        private void CheckScreenStatus() 
        {
            for (int i = 0; i < screens.Count; i++)
            {
                if(screens[i].GetScreenState()==ScreenState.Inactive)
                {
                    RemoveScreen(screens[i]);
                }
            }
        }

     

    }
}
