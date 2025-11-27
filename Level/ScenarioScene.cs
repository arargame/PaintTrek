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
            Globals.Game.IsMouseVisible = false;
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
                    Globals.Game.IsMouseVisible = true;
                }

                Vector2 p = new Vector2(Globals.GameSize.X/2-Globals.GameFont.MeasureString(s).X/2, textPosition.Y + 10);
                Globals.SpriteBatch.DrawString(Globals.GameFont, s, p, Color.White);
            }

            if (isKeyForStarting)
            {
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Skip>>", new Vector2(Globals.GameSize.X - Globals.GameFont.MeasureString("Skip>>").X - 10, Globals.GameSize.Y - Globals.GameFont.MeasureString("Skip>>").Y), Color.White);
                Rectangle enterRect = new Rectangle((int)(Globals.GameSize.X - Globals.GameFont.MeasureString("Skip>>").X - 10), (int)(Globals.GameSize.Y - Globals.GameFont.MeasureString("Skip>>").Y), (int)Globals.GameFont.MeasureString("Skip>>").X, (int)Globals.GameFont.MeasureString("Skip>>").Y);
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
