using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class ScenarioScene
    {
        double time;
        bool isKeyForStarting;
        Texture2D texture;
        Vector2 textPosition;
        Rectangle textureRect;
        string str;
        double charCounter;
        ClickableArea clickableArea;

        internal ClickableArea ClickableArea
        {
            get { return clickableArea; }
            set { clickableArea = value; }
        }

        public ScenarioScene()
        {
            isKeyForStarting = false;
            Globals.ShowCursor = false;
            time = 0;
            charCounter = 0;

            if (Level.LevelCounter == 1)
            {
                texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc1");
                str = "Life is a game painted by the owner.\nYou just need to play and enjoy it...";
            }
            int size = 250;
            textureRect = new Rectangle((int)(Globals.GameSize.X / 2 - size / 2), (int)(Globals.GameSize.Y / 2 - size / 2), size, size);
            textPosition = new Vector2(textureRect.Left, textureRect.Bottom);

            clickableArea = new ClickableArea();

            
        }

        ~ScenarioScene() 
        {
            
        }

        public void Update()
        {
            time += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            charCounter += 0.2f;

            if (Level.LevelCounter == 1)
            {
                if ((int)time == 1)
                {
                    texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc2");
                }
                else if ((int)time == 2)
                {
                    texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc3");
                }
                else if ((int)time == 3)
                {
                    texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc4");
                }
                if ((int)time == 4)
                {
                    texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc5");
                }
                else if ((int)time == 5)
                {
                    texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc6");
                }
            }
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            if (Level.LevelCounter == 1)
            {
                Globals.SpriteBatch.Draw(texture, textureRect, Color.White);
                Char[] array = str.ToCharArray();
                string s = "";

                if ((int)charCounter <= array.Length)
                {
                    for (int i = 0; i < (int)charCounter; i++)
                    {
                        s += array[i];
                    }
                }
                else
                {
                    s = str; 
                    isKeyForStarting = true;
                    Globals.ShowCursor = true;
                }

                Vector2 p = new Vector2(Globals.GameSize.X/2-Globals.GameFont.MeasureString(s).X/2, textPosition.Y + 10);
                Globals.SpriteBatch.DrawString(Globals.GameFont, s, p, Color.White);
            }

            if (isKeyForStarting)
            {
                // Position Skip button like BackButton - bottom right with margin
                float skipWidth = Globals.GameFont.MeasureString("Skip>>").X;
                float skipHeight = Globals.GameFont.MeasureString("Skip>>").Y;
                Vector2 skipPosition = new Vector2(Globals.GameSize.X - (skipWidth + 20), Globals.GameSize.Y - (skipHeight * 2));
                
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Skip>>", skipPosition, Color.White);
                Rectangle enterRect = new Rectangle((int)skipPosition.X, (int)skipPosition.Y, (int)skipWidth, (int)skipHeight);
                clickableArea.SetRect(enterRect);
            }

            Globals.SpriteBatch.End();
        }

        public bool GetKeyForStarting()
        {
            return isKeyForStarting;
        }

        private void ChangeTexture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
