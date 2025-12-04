using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
// using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PaintTrek
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        ScreenManager screenManager;
        GraphicSettings graphicSettings;
        ClickableAreaSystem clickableAreaSystem;
        TimeKeeper timeKeeper;
        Loader loader;
        RenderTarget2D renderTarget;
        Rectangle renderRect;
        
        // Debug overlay - automatically synced with Globals.DeveloperMode
        private bool showDebugInfo => Globals.DeveloperMode;
        KeyboardState lastKeyboardState;

        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Force fixed 60 FPS update rate so gameplay speed is consistent
            //IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
            //Globals.Graphics.SynchronizeWithVerticalRetrace = true;
        }


        protected override void Initialize()
        {
            Reset();
            //TargetElapsedTime = TimeSpan.FromTicks(333333);
            IsMouseVisible = false;
            loader = new Loader();

            Globals.Window = Window;
            Globals.Game = this;
            // Globals.Window.AllowUserResizing = true;
            Globals.Random = new Random();
            Globals.PreviousSpawnTime = TimeSpan.Zero;
            Globals.EnemySpawnTime = TimeSpan.FromSeconds(1.0f);

            base.Initialize();
        }




        protected override void LoadContent()
        {

            Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.Content = Content;
            Globals.Window.Title = "Paint Trek";
            
            // Load GameSettings FIRST
            GameSettings.Instance.Load();
            GameSettings.Instance.SyncToGlobals();
            System.Diagnostics.Debug.WriteLine("[Game1] GameSettings loaded and synced");
            
            // Initialize graphics (defaults to fullscreen)
            graphicSettings = new GraphicSettings();
            
            // Override with saved resolution from GameSettings if different
            if (!GameSettings.Instance.IsFullScreen && Globals.Graphics.IsFullScreen)
            {
                GraphicSettings.MakeWindowed();
                System.Diagnostics.Debug.WriteLine("[Game1] Applied windowed resolution from saved settings");
            }
            else if (GameSettings.Instance.IsFullScreen && !Globals.Graphics.IsFullScreen)
            {
                GraphicSettings.MakeFullScreen();
                System.Diagnostics.Debug.WriteLine("[Game1] Applied fullscreen resolution from saved settings");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Game1] Using default resolution (IsFullScreen: {Globals.Graphics.IsFullScreen})");
            }
            
            // Initialize RenderTarget with GameSize
            renderTarget = new RenderTarget2D(GraphicsDevice, (int)Globals.GameSize.X, (int)Globals.GameSize.Y);
            
            Loader.Load();
            timeKeeper = new TimeKeeper();

            clickableAreaSystem = new ClickableAreaSystem();
            screenManager = new ScreenManager();
            screenManager.LoadContent(); // Load cursor texture
            ScreenManager.AddScreen(new StartingScreen());
            GlobalTexture.LoadTextures();
        }


        protected override void UnloadContent()
        {
            
        }
        
        /// <summary>
        /// Recreate RenderTarget when resolution changes
        /// </summary>
        public void RecreateRenderTarget()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
            }
            
            renderTarget = new RenderTarget2D(GraphicsDevice, (int)Globals.GameSize.X, (int)Globals.GameSize.Y);
            System.Diagnostics.Debug.WriteLine($"[Game1] RenderTarget recreated: {Globals.GameSize.X}x{Globals.GameSize.Y}");
        }


        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Globals.exitGame)
            {
                bool b = false;
                do
                {
                    b = loader.FileSettingsSave();
                } while (b == false);

                this.Exit();
            }

            Globals.GameTime = gameTime;
            Globals.GameRect = new Rectangle(0, 0, (int)Globals.GameSize.X, (int)Globals.GameSize.Y);
            Globals.IsActive = IsActive;

            // Manage mouse visibility: Always hide system cursor (we draw a custom one)
            IsMouseVisible = false;

            // Alt+Enter to toggle fullscreen (for debugging)
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.LeftAlt) && keyState.IsKeyDown(Keys.Enter))
            {
                if (!Globals.Graphics.IsFullScreen)
                {
                    GraphicSettings.MakeFullScreen();
                }
                else
                {
                    // Resolution change disabled - always use fullscreen (1280x800)
                    GraphicSettings.MakeFullScreen();
                }
                System.Threading.Thread.Sleep(200); // Prevent rapid toggling
            }
            
            // F1 to toggle DeveloperMode (showDebugInfo automatically follows via property)
            if (keyState.IsKeyDown(Keys.F1) && !lastKeyboardState.IsKeyDown(Keys.F1))
            {
                Globals.DeveloperMode = !Globals.DeveloperMode;
                System.Diagnostics.Debug.WriteLine($"[Game1] Developer Mode: {(Globals.DeveloperMode ? "ON" : "OFF")}");
            }
            lastKeyboardState = keyState;

            screenManager.Update();
            timeKeeper.Update();
            clickableAreaSystem.Update();
            
            // Screenshot functionality - Press F12 or PrintScreen to take a screenshot
            ScreenshotHelper.Update(GraphicsDevice);

            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {
            if (Globals.Graphics.IsFullScreen)
            {
                // FULLSCREEN MODE: Draw to RenderTarget and Scale Up
                
                // 1. Draw game to RenderTarget (1280x800)
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                screenManager.Draw();
                //clickableAreaSystem.Draw();

                // 2. Switch back to BackBuffer (Screen)
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                // 3. Calculate destination rectangle (Stretch to fit screen)
                var viewport = GraphicsDevice.Viewport;
                
                // Stretch scaling (fills screen, ignores aspect ratio)
                int width = viewport.Width;
                int height = viewport.Height;
                renderRect = new Rectangle(0, 0, width, height);

                // 4. Draw RenderTarget to Screen
                // Use LinearClamp for smoother upscaling (reduces pixelation)
                Globals.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp);
                Globals.SpriteBatch.Draw(renderTarget, renderRect, Color.White);
                
                // Draw debug info on top
                if (showDebugInfo)
                {
                    DrawDebugInfo();
                }
                
                Globals.SpriteBatch.End();
            }
            else
            {
                // WINDOWED MODE: Draw directly to BackBuffer (No Scaling)
                GraphicsDevice.Clear(Color.CornflowerBlue);
                screenManager.Draw();
                //clickableAreaSystem.Draw();
                
                // Draw debug info on top
                if (showDebugInfo)
                {
                    Globals.SpriteBatch.Begin();
                    DrawDebugInfo();
                    Globals.SpriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }

        private void Reset()
        {
            FileSystem FS = new FileSystem();
            if(FS.isFirstTime())
            {
                Loader.Reset();
            }

        }
        
        private void DrawDebugInfo()
        {
            if (Globals.GameFont == null) return;
            
            // Position debug info at top-right corner with margin
            float lineHeight = 20f;
            float rightMargin = 10f;
            
            // Approximate max width (will be calculated dynamically)
            float estimatedMaxWidth = 400f;
            
            // Start position at top-right
            Vector2 position = new Vector2(Globals.GameSize.X - estimatedMaxWidth - rightMargin, 10);
            
            // Color scheme
            Color headerColor = Color.CornflowerBlue;
            Color keyColor = Color.Beige;
            Color valueColor = Color.White;
            
            float currentY = position.Y;
            
            // Header
            Globals.SpriteBatch.DrawString(Globals.GameFont, "=== DEBUG INFO (F1 to toggle) ===", 
                new Vector2(position.X, currentY), headerColor);
            currentY += lineHeight;
            
            // FPS
            string fpsKey = "FPS: ";
            string fpsValue = $"{1.0 / Globals.GameTime.ElapsedGameTime.TotalSeconds:F1}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, fpsKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, fpsValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(fpsKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Elapsed
            string elapsedKey = "Elapsed: ";
            string elapsedValue = $"{Globals.GameTime.ElapsedGameTime.TotalSeconds:F4}s";
            Globals.SpriteBatch.DrawString(Globals.GameFont, elapsedKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, elapsedValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(elapsedKey).X, currentY), valueColor);
            currentY += lineHeight * 2;
            
            // Screen Manager Header
            Globals.SpriteBatch.DrawString(Globals.GameFont, "--- SCREEN MANAGER ---", 
                new Vector2(position.X, currentY), headerColor);
            currentY += lineHeight;
            
            // Total Screens
            string totalKey = "Total Screens: ";
            string totalValue = $"{ScreenManager.screens.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, totalKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, totalValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(totalKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            int activeCount = 0;
            int frozenCount = 0;
            int inactiveCount = 0;
            int gameBoardCount = 0;
            
            for (int i = 0; i < ScreenManager.screens.Count; i++)
            {
                var screen = ScreenManager.screens[i];
                var state = screen.GetScreenState();
                string screenName = screen.GetType().Name;
                
                if (state == ScreenState.Active) activeCount++;
                else if (state == ScreenState.Frozen) frozenCount++;
                else if (state == ScreenState.Inactive) inactiveCount++;
                
                if (screenName == "GameBoard") gameBoardCount++;
                
                string screenKey = $"[{i}] {screenName}: ";
                string screenValue = $"{state}";
                Globals.SpriteBatch.DrawString(Globals.GameFont, screenKey, 
                    new Vector2(position.X, currentY), keyColor);
                Globals.SpriteBatch.DrawString(Globals.GameFont, screenValue, 
                    new Vector2(position.X + Globals.GameFont.MeasureString(screenKey).X, currentY), valueColor);
                currentY += lineHeight;
            }
            
            currentY += lineHeight * 0.5f;
            string countsKey = "Active: ";
            string countsValue = $"{activeCount}, Frozen: {frozenCount}, Inactive: {inactiveCount}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, countsKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, countsValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(countsKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            string gbKey = "GameBoard instances: ";
            string gbValue = $"{gameBoardCount}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, gbKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, gbValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(gbKey).X, currentY), valueColor);
            currentY += lineHeight * 2;
            
            // Sprite System Header
            Globals.SpriteBatch.DrawString(Globals.GameFont, "--- SPRITE SYSTEM ---", 
                new Vector2(position.X, currentY), headerColor);
            currentY += lineHeight;
            
            // Total Sprites
            string spritesKey = "Total Sprites: ";
            string spritesValue = $"{SpriteSystem.spriteList.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, spritesKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, spritesValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(spritesKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Enemies
            string enemiesKey = "Enemies: ";
            string enemiesValue = $"{EnemySystem.enemyList.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, enemiesKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, enemiesValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(enemiesKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Bullets
            string bulletsKey = "Bullets: ";
            string bulletsValue = $"{GunSystem.bulletList.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, bulletsKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, bulletsValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(bulletsKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Collectables
            string collectKey = "Collectables: ";
            string collectValue = $"{CollectableObjectSystem.collactableObjectList.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, collectKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, collectValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(collectKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Bosses
            string bossesKey = "Bosses: ";
            string bossesValue = $"{BossSystem.bossList.Count}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, bossesKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, bossesValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(bossesKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            // Count Player instances
            int playerCount = 0;
            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                if (SpriteSystem.spriteList[i] is Player)
                    playerCount++;
            }
            string playersKey = "Players: ";
            string playersValue = $"{playerCount}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, playersKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, playersValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(playersKey).X, currentY), valueColor);
            currentY += lineHeight * 2;
            
            // GameSettings Info Header
            Globals.SpriteBatch.DrawString(Globals.GameFont, "--- GAME SETTINGS ---", 
                new Vector2(position.X, currentY), headerColor);
            currentY += lineHeight;
            
            // Settings info
            string settingsKey = "Sound Effects: ";
            string settingsValue = $"{(Globals.SoundEffectsEnabled ? "ON" : "OFF")}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, settingsKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, settingsValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(settingsKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            string musicKey = "Music: ";
            string musicValue = $"{(Globals.MusicsEnabled ? "ON" : "OFF")}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, musicKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, musicValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(musicKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            string menuSoundKey = "Menu Sounds: ";
            string menuSoundValue = $"{(Globals.MenuSoundsEnabled ? "ON" : "OFF")}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, menuSoundKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, menuSoundValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(menuSoundKey).X, currentY), valueColor);
            currentY += lineHeight;
            
            string devModeKey = "Developer Mode: ";
            string devModeValue = $"{(Globals.DeveloperMode ? "ON" : "OFF")}";
            Globals.SpriteBatch.DrawString(Globals.GameFont, devModeKey, 
                new Vector2(position.X, currentY), keyColor);
            Globals.SpriteBatch.DrawString(Globals.GameFont, devModeValue, 
                new Vector2(position.X + Globals.GameFont.MeasureString(devModeKey).X, currentY), valueColor);
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            bool b = false;
            do
            {
                b = loader.FileSettingsSave();
            } while (b == false);

            base.OnExiting(sender, args);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup resources
                if (screenManager != null)
                {
                    screenManager = null;
                }
                
                // Force exit
                Environment.Exit(0);
            }
            
            base.Dispose(disposing);
        }


    }
}
