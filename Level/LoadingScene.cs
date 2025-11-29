using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class LoadingScene
    {
        #region LevelTransitionFields

        Texture2D transitionTexture;
        Texture2D loadBar;
        Texture2D barStrike;
        Rectangle transTextureRect;
        Rectangle loadBarRect;
        double transitionTimer;
        bool openingKeyForLevel;

        Rectangle enterRect;

        ClickableArea clickableArea;

        internal ClickableArea ClickableArea
        {
            get { return clickableArea; }
            set { clickableArea = value; }
        }

        #endregion

        public LoadingScene()
        {
            loadBar = Globals.Content.Load<Texture2D>("Textures/LoadBar");
            barStrike = Globals.Content.Load<Texture2D>("Textures/BarStrike");

            openingKeyForLevel = false;
            transitionTimer = 0;

            //loadBarRect = new Rectangle((int)(Globals.GameRect.Center.X - loadBar.Width * 2 / 3), (int)(Globals.GameRect.Center.Y + Globals.GameSize.Y / 4), loadBar.Width, loadBar.Height);
            loadBarRect = new Rectangle((int)(Globals.GameSize.X/2 - loadBar.Width/2), (int)(Globals.GameRect.Center.Y + Globals.GameSize.Y / 4), loadBar.Width, loadBar.Height);

            LoadTransitionTexture();

            transTextureRect = new Rectangle(0, Globals.GameRect.Center.Y - (transitionTexture.Height), (int)Globals.GameSize.X, (int)Globals.GameSize.Y / 3);
            clickableArea = new ClickableArea(new Rectangle((int)(Globals.GameRect.Center.X - Globals.GameSize.X / 7), (int)(Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), 100, 30));

            GlobalTexture.LoadTextures();
        }

        ~LoadingScene() 
        {
            UnloadContent();
        }

        public void UnloadContent() 
        {
            
        }

        public void Update()
        {
            if (transitionTimer > 37)
            {
                // timer = 0;
                openingKeyForLevel = true;
                Globals.ShowCursor = true;
            }
            else
            {
                Globals.ShowCursor = false;
                transitionTimer += Globals.GameTime.ElapsedGameTime.TotalSeconds * 16;
            }

            //InputState inputState = new InputState();

        }
        public void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(transitionTexture, transTextureRect, Color.White);
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Level :" + Level.LevelCounter, new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString("Level :").X / 2, Globals.GameRect.Center.Y + Globals.GameSize.Y / 6), Color.White);

            Globals.SpriteBatch.Draw(loadBar, loadBarRect, Color.White);


            //Globals.SpriteBatch.Draw(barStrike,new Rectangle(405+5*i,405,5,10),Color.White);
            for (int i = 0; i < transitionTimer; i++)
            {
                Globals.SpriteBatch.Draw(barStrike, new Rectangle((loadBarRect.X + 5) + 5 * i, loadBarRect.Y, 5, loadBar.Height), Color.White);
            }

            if (openingKeyForLevel)
            {
                //Globals.SpriteBatch.DrawString(Globals.GameFont, "Enter to Continue", new Vector2(Globals.GameRect.Center.X - Globals.GameSize.X / 7, Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), Color.White);
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Enter to Continue", new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString("Enter to Continue").X/2, Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), Color.White);
                enterRect = new Rectangle((int)(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString("Enter to Continue").X / 2),(int)( Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), (int)Globals.GameFont.MeasureString("Enter to Continue").X, (int)Globals.GameFont.MeasureString("Enter to Continue").Y);
                clickableArea.SetRect(enterRect);
            }
            Globals.SpriteBatch.End();
        }

        public void LoadTransitionTexture()
        {
            transitionTexture = Globals.Content.Load<Texture2D>("LoadingScene/transitionTexture" + Level.LevelCounter);
        }

        public bool GetKeyForStarting()
        {
            return openingKeyForLevel;
        }
    }
}
