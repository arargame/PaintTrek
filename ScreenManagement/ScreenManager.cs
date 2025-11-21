using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class ScreenManager
    {
        internal static List<Screen> screens;
        public InputState inputState;

        public ScreenManager() 
        {
            screens = new List<Screen>();
            inputState = new InputState();
        }

        public void Update() 
        {
            CheckScreenStatus();

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
           // Globals.SpriteBatch.DrawString(Globals.MenuFont, "Screens :" + ScreenManager.screens.Count, new Vector2(300, 300), Color.Red);
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
