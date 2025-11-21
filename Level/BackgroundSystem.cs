using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BackgroundSystem
    {
        ParallaxingBackground bgLayer;
        Texture2D mainBackground;

        public BackgroundSystem()
        {
            bgLayer = new ParallaxingBackground();
            LoadBackgroundTexture();
        }
        ~BackgroundSystem() 
        {
            UnloadContent();
        }

        private void LoadBackgroundTexture()
        {
            if (Level.LevelCounter > 0 && Level.LevelCounter < 11)
            {
                #if WINDOWS
                if (Globals.Graphics.IsFullScreen)
                {
                    bgLayer.Initialize("Backgrounds/bgLayer" + Level.LevelCounter + "(1280,800)", (int)Globals.GameSize.X, -2);
                }
                else
                {
                    bgLayer.Initialize("Backgrounds/bgLayer" + Level.LevelCounter + "(800,600)", (int)Globals.GameSize.X, -2);
                }
#elif WINDOWSPHONE
                #endif

                mainBackground = Globals.Content.Load<Texture2D>("Backgrounds/level" + Level.LevelCounter + "_bg");
            }
        }

        public void UnloadContent() 
        {
            bgLayer.UnloadContent();
        }

        public void Update()
        {
            if (bgLayer.Initialized)
                bgLayer.Update();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            if (mainBackground != null)
                Globals.SpriteBatch.Draw(mainBackground, new Rectangle(0, 0, (int)Globals.GameSize.X+100, (int)Globals.GameSize.Y+100), Color.White);
            Globals.SpriteBatch.End();

            if (bgLayer.Initialized)
                bgLayer.Draw();
        }
    }
}
