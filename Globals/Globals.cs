using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Platform;

namespace PaintTrek
{
    public enum GameMode
    {
        Normal,
        Endless,
        UfoInvasion,
        AgainstAllBosses
    }

    class Globals
    {
        public static ContentManager Content;
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static GameTime GameTime;
        // GameSize is the fixed virtual canvas. ActualScreenSize is the resizable back buffer.
        public static Vector2 GameSize;
        public static Vector2 ActualScreenSize;
        public static Random Random;
        public static TimeSpan EnemySpawnTime;
        public static TimeSpan PreviousSpawnTime;
        public static Rectangle GameRect;
        public static bool GameSoundsActivated;
        public static GameWindow Window;
        public static Game Game;
        public static IGamePlatformServices PlatformServices;
        public static GameMode CurrentMode = GameMode.Normal;
        public static bool IsWaveMode => CurrentMode != GameMode.Normal;
        public static bool exitGame;
        public static bool firstTimeYouPlay;

        public static SpriteFont GameFont;
        public static SpriteFont MenuFont;

        public static bool AutoAttack;

        public static bool IsActive;

        public static double XX;
        
        public static bool DebugMode = false; // Enable debug visualization
        public static bool ShowCursor = true;
        
        // Yeni ses ayarları (GameSettings ile senkronize)
        public static bool SoundEffectsEnabled = true;
        public static bool MusicsEnabled = true;
        public static bool MenuSoundsEnabled = true;
        public static bool DeveloperMode = false;
    }
}
