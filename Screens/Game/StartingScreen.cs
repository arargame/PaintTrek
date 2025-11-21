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
        Texture2D xnaTexture;
        double time;

        public StartingScreen() 
        {
            Initialize();
        }

        ~StartingScreen() 
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

            screenTitle = "Paint Trek";
            Globals.Window.Title = screenTitle;

            lifeSpan = 0;
            logoOpacity = 0;
            isAlive = true;

            tradeMarkSpritePosition = new Vector2((Globals.GameSize.X / 2) - gameFont.MeasureString("Arar Game Studio.2010").X / 2, (Globals.GameSize.Y / 2) + (logoTexture.Height * (1.3f) / 3));
            logoRect = new Rectangle((int)Globals.GameSize.X / 2 - logoTexture.Width / 2, (int)Globals.GameSize.Y / 2 - logoTexture.Height / 2, logoTexture.Width, logoTexture.Height);

            time = 0;
        }

        public override void Load()
        {
            base.Load();
            logoTexture = Globals.Content.Load<Texture2D>(@"Textures/smilemanLogo");
            xnaTexture = Globals.Content.Load<Texture2D>(@"Textures/xnaTexture");
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

                if (logoOpacity < 255)
                {
                    logoOpacity++;
                }

                if (lifeSpan < 9 && logoOpacity < 255)
                {
                    lifeSpan += Globals.GameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    if (time >= 500)
                    isAlive = false;
                }

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

            if (time < 300)
            {
                Globals.SpriteBatch.Draw(logoTexture, logoRect, new Color((byte)255, (byte)255, (byte)255, (byte)logoOpacity));

                if (logoOpacity > 100)
                {
                    Globals.SpriteBatch.DrawString(gameFont, "Arar Game Studio.2010", tradeMarkSpritePosition, Color.White);
                }
            }
            else 
            {
                Globals.SpriteBatch.Draw(xnaTexture, new Rectangle((int)Globals.GameSize.X / 2 - xnaTexture.Width / 2, (int)Globals.GameSize.Y / 2 - xnaTexture.Height / 2, xnaTexture.Width, xnaTexture.Height), Color.White);
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
