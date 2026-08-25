using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class StartingScreen:GameScreen
    {
        Byte logoOpacity;
        Rectangle logoRect;
        Vector2 tradeMarkSpritePosition;
        Texture2D logoTexture;
        bool isAlive;
        double lifeSpan;
        Texture2D monogameTexture;
        double time;
        double splashElapsedSeconds;
        bool launchCountRecorded;

        private double PhaseDuration
        {
            get
            {
#if DEBUG
                return 1.0;
#else
                int launchCount = Math.Max(1, GameSettings.Instance.InitSplashLaunchCount);
                return Math.Max(1.0, 5.0 - (launchCount - 1));
#endif
            }
        }

        private double TotalDuration => PhaseDuration * 2.0;

        public StartingScreen() 
        {
            Globals.Game.IsMouseVisible = false;
            Initialize();
        }

        ~StartingScreen() 
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            Globals.ShowCursor = false;

            screenTitle = "Paint Trek";
            Globals.Window.Title = screenTitle;

            lifeSpan = 0;
            logoOpacity = 0;
            isAlive = true;

            tradeMarkSpritePosition = new Vector2((Globals.GameSize.X / 2) - gameFont.MeasureString("Arar Games Studio.2010").X / 2, (Globals.GameSize.Y / 2) + (logoTexture.Height * (1.3f) / 3));
            logoRect = new Rectangle((int)Globals.GameSize.X / 2 - logoTexture.Width / 2, (int)Globals.GameSize.Y / 2 - logoTexture.Height / 2, logoTexture.Width, logoTexture.Height);

            time = 0;
            splashElapsedSeconds = 0;
            launchCountRecorded = false;
        }

        public override void Load()
        {
            base.Load();
            logoTexture = Globals.Content.Load<Texture2D>(@"Textures/smilemanLogo");
            monogameTexture = Globals.Content.Load<Texture2D>("monogameTexture");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            logoTexture.Dispose();
        }

        public override void Update()
        {

            if (screenState == ScreenState.Active) 
            {

                time++;

                if (!launchCountRecorded)
                {
                    launchCountRecorded = true;
                    GameSettings.Instance.InitSplashLaunchCount++;
                    GameSettings.Instance.MarkDirty();
                    GameSettings.Instance.Save();
                }

                splashElapsedSeconds += Globals.GameTime.ElapsedGameTime.TotalSeconds;

                if (logoOpacity < 255)
                {
                    logoOpacity++;
                }

                if (lifeSpan < 9 && logoOpacity < 255)
                {
                    lifeSpan += Globals.GameTime.ElapsedGameTime.TotalSeconds;
                }

                if (splashElapsedSeconds >= TotalDuration)
                    isAlive = false;

                if (isAlive == false)
                {
                    ExitScreen();
                }

                base.Update();
            }
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied);

            if (splashElapsedSeconds < PhaseDuration)
            {
                Globals.SpriteBatch.Draw(logoTexture, logoRect, new Color((byte)255, (byte)255, (byte)255, (byte)logoOpacity));

                if (logoOpacity > 100)
                {
                    Globals.SpriteBatch.DrawString(gameFont, "Arar Games Studio.2010", tradeMarkSpritePosition, Color.White);
                }
            }
            else 
            {
                Globals.SpriteBatch.Draw(monogameTexture, new Rectangle((int)Globals.GameSize.X / 2 - monogameTexture.Width / 2, (int)Globals.GameSize.Y / 2 - monogameTexture.Height / 2, monogameTexture.Width, monogameTexture.Height), Color.White);
            }

            Globals.SpriteBatch.End();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void ExitScreen()
        {
            screenState = ScreenState.Inactive;
            ScreenManager.AddScreen(new MainMenuScreen());
            GC.ReRegisterForFinalize(this);
        }
    }
}
