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
            
            // Create cursor texture programmatically (Arrow shape)
            int width = 12;
            int height = 20;
            cursorTexture = new Texture2D(Globals.Graphics.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            
            // Simple arrow shape logic
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Draw outline (Black) and fill (White)
                    // This is a simplified arrow shape
                    if (x == 0 || x == y || x == width - 1 && y > width/2) 
                        data[y * width + x] = Color.Black; // Outline (rough)
                    else if (x < y)
                        data[y * width + x] = Color.White; // Fill
                    else
                        data[y * width + x] = Color.Transparent;
                }
            }
            // Better manual pixel art for arrow
            // Clear all to transparent first
            for(int i=0; i<data.Length; i++) data[i] = Color.Transparent;
            
            // Draw arrow pixels (White with Black border)
            // Tip is at 0,0
            // Left edge
            for(int y=0; y<16; y++) { data[y*width] = Color.Black; data[y*width+1] = Color.White; }
            // Diagonal edge
            for(int i=0; i<12; i++) { data[i*width+i] = Color.Black; if(i>0) data[i*width+(i-1)] = Color.White; }
            // Bottom
            for(int x=0; x<8; x++) { data[11*width+x] = Color.Black; }
            
            cursorTexture.SetData(data);
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
            if (cursorTexture != null && Globals.Graphics.IsFullScreen)
            {
                Globals.SpriteBatch.Draw(cursorTexture, inputState.cursorPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            Globals.SpriteBatch.End();
        }

        internal static void AddScreen(Screen screen) 
        {
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
