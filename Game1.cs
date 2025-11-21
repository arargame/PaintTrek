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


        public Game1()
        {
            Globals.Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            Reset();
            //TargetElapsedTime = TimeSpan.FromTicks(333333);
            IsMouseVisible = true;
            loader = new Loader();

            Globals.Window = Window;
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
            graphicSettings = new GraphicSettings();
            Loader.Load();
            timeKeeper = new TimeKeeper();

            clickableAreaSystem = new ClickableAreaSystem();
            screenManager = new ScreenManager();
            ScreenManager.AddScreen(new StartingScreen());
            GlobalTexture.LoadTextures();
        }


        protected override void UnloadContent()
        {
            
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
                    GraphicSettings.ExecuteScreenSize(800, 600);
                }
                System.Threading.Thread.Sleep(200); // Prevent rapid toggling
            }

            screenManager.Update();
            timeKeeper.Update();
            clickableAreaSystem.Update();
            
            // Screenshot functionality - Press F12 or PrintScreen to take a screenshot
            ScreenshotHelper.Update(GraphicsDevice);

            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            screenManager.Draw();

           //clickableAreaSystem.Draw();

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
