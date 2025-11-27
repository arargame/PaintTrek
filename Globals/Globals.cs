using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Globals
    {
        public static ContentManager Content;
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static GameTime GameTime;
        public static Vector2 GameSize;
        public static Random Random;
        public static TimeSpan EnemySpawnTime;
        public static TimeSpan PreviousSpawnTime;
        public static Rectangle GameRect;
        public static bool GameSoundsActivated;
        public static GameWindow Window;
        public static Game Game;
        public static bool exitGame;
        public static bool firstTimeYouPlay;

        public static SpriteFont GameFont;
        public static SpriteFont MenuFont;

        public static bool AutoAttack;

        public static bool IsActive;

        public static double XX;
        
        public static bool DebugMode = true; // Enable debug visualization
    }
}
