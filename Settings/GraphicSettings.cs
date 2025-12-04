using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class GraphicSettings
    {
        public static Rectangle leftAreaRectofScreen;
        public static Rectangle rightAreaofScreen;
        public static Rectangle topAreaofScreen;
        public static Rectangle bottomAreaofScreen;

        public GraphicSettings()
        {
            // Default to fullscreen - will be overridden by GameSettings if needed
            // This ensures we have a valid initial state
            MakeFullScreen();
        }

        public static Vector2 GetScreenSize()
        {
            return new Vector2(Globals.Graphics.PreferredBackBufferWidth, Globals.Graphics.PreferredBackBufferHeight);
        }

        public static void ExecuteScreenSize(float screenWidth, float screenHeight)
        {
            Globals.MenuFont = Globals.Content.Load<SpriteFont>("Fonts/MenuFont_1");
            Globals.GameFont = Globals.Content.Load<SpriteFont>("Fonts/GameFont_1");

            Globals.Graphics.IsFullScreen = false;
            Globals.GameSize = new Vector2(screenWidth, screenHeight);
            Globals.Graphics.PreferredBackBufferWidth = (int)Globals.GameSize.X;
            Globals.Graphics.PreferredBackBufferHeight = (int)Globals.GameSize.Y;
            Globals.Graphics.ApplyChanges();

            ExecuteAllRect();
        }

        public static void MakeFullScreen()
        {
            Globals.MenuFont = Globals.Content.Load<SpriteFont>("Fonts/MenuFont_2");
            Globals.GameFont = Globals.Content.Load<SpriteFont>("Fonts/GameFont_2");

            Globals.Graphics.HardwareModeSwitch = false; // Use Borderless Window
            Globals.Graphics.IsFullScreen = true;
            Globals.GameSize = new Vector2(1280, 800);
            Globals.Graphics.PreferredBackBufferWidth = (int)Globals.GameSize.X;
            Globals.Graphics.PreferredBackBufferHeight = (int)Globals.GameSize.Y;
            Globals.Graphics.ApplyChanges();

            ExecuteAllRect();
            
            // Recreate RenderTarget with new resolution
            if (Globals.Game != null)
            {
                ((Game1)Globals.Game).RecreateRenderTarget();
            }
        }
        
        public static void MakeWindowed()
        {
            ExecuteScreenSize(800, 600);
            
            // Recreate RenderTarget with new resolution
            if (Globals.Game != null)
            {
                ((Game1)Globals.Game).RecreateRenderTarget();
            }
        }

        public static void ExecuteAllRect()
        {
            int width = (int)Globals.GameSize.X;
            int height = (int)Globals.GameSize.Y;

            Globals.GameRect = new Rectangle(0, 0, width, height);
            leftAreaRectofScreen = new Rectangle(-width, -height, width - 100, height * 3);
            rightAreaofScreen = new Rectangle(width, -height, width, height * 3);
            topAreaofScreen = new Rectangle(0, -height, width, height);
            bottomAreaofScreen = new Rectangle(0, height, width, height);
        }
        public static void ApplyChanges()
        {
            Globals.Graphics.ApplyChanges();
        }

    }
}
