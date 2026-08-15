using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PaintTrek.Shared.Localization;

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

        PaintTrek.Shared.Tips.GameTip currentTip;


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

            clickableArea = new ClickableArea(new Rectangle((int)(Globals.GameRect.Center.X - Globals.GameSize.X / 7), (int)(Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), 100, 30));

            GlobalTexture.LoadTextures();

            // Load a random tip
            currentTip = PaintTrek.Shared.Tips.LoadingTipProvider.GetRandom(PaintTrek.Shared.Tips.Platform.Desktop);
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
            
            string levelCounterLabel = string.Format(Loc.T(LocKeys.Gameplay.LevelCounter), Level.LevelCounter);
            Globals.SpriteBatch.DrawString(Globals.GameFont, levelCounterLabel, new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString(levelCounterLabel).X / 2, Globals.GameRect.Center.Y + Globals.GameSize.Y / 6), Color.White);

            Globals.SpriteBatch.Draw(loadBar, loadBarRect, Color.White);

            // Draw Tip
            if (currentTip != null)
            {
                string text = "'" + Loc.T(currentTip.LocKey) + "'";
                float tipScale = 0.7f;
                // Adjust wrap width for scale
                string wrappedText = WrapText(Globals.GameFont, text, (Globals.GameSize.X * 0.8f) / tipScale);
                Vector2 textSize = Globals.GameFont.MeasureString(wrappedText) * tipScale;
                
                Vector2 pos = new Vector2(Globals.GameSize.X / 2 - textSize.X / 2, loadBarRect.Bottom + 20);
                
                Globals.SpriteBatch.DrawString(Globals.GameFont, wrappedText, pos, Color.LightGray, 0f, Vector2.Zero, tipScale, SpriteEffects.None, 0f);
            }


            //Globals.SpriteBatch.Draw(barStrike,new Rectangle(405+5*i,405,5,10),Color.White);
            for (int i = 0; i < transitionTimer; i++)
            {
                Globals.SpriteBatch.Draw(barStrike, new Rectangle((loadBarRect.X + 5) + 5 * i, loadBarRect.Y, 5, loadBar.Height), Color.White);
            }

            if (openingKeyForLevel)
            {
                string enterToContinueLabel = Loc.T(LocKeys.Gameplay.EnterToContinue);
                Globals.SpriteBatch.DrawString(Globals.GameFont, enterToContinueLabel, new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString(enterToContinueLabel).X/2, Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), Color.White);
                enterRect = new Rectangle((int)(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString(enterToContinueLabel).X / 2),(int)( Globals.GameRect.Center.Y + Globals.GameSize.Y / 5), (int)Globals.GameFont.MeasureString(enterToContinueLabel).X, (int)Globals.GameFont.MeasureString(enterToContinueLabel).Y);
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

        private string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
