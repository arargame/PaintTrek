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

        // Dialog system
        private int currentDialogIndex = 0;
        private List<string> dialogLines;
        private bool dialogComplete = false;

        internal ClickableArea ClickableArea
        {
            get { return clickableArea; }
            set { clickableArea = value; }
        }

        public ScenarioScene()
        {
            isKeyForStarting = true;
            Globals.ShowCursor = true;
            time = 0;
            charCounter = 0;

            if (Level.LevelCounter == 1)
            {
                // Start with story image and dialog
                texture = Globals.Content.Load<Texture2D>("Scenario/Starting/story");
                
                // Dialog lines
                dialogLines = new List<string>
                {
                    "Child: Mom, look what I drew! Do you like it?   ",
                    "Mom: Very impressive! I love it!   ",
                    "Child: I wish I could play with them...   ",
                    "Mom: Everything you can imagine may become real\nin this world, sweetie. Never stop dreaming.   "
                };
                
                str = dialogLines[0]; // Start with first dialog
            }

            int size = (int)(Globals.GameSize.Y * 0.55f);

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

            if (Level.LevelCounter == 1)
            {
                if (!dialogComplete)
                {
                    // Dialog phase - advance char counter
                    charCounter += 0.15f;
                    
                    // Check if current dialog is complete
                    if ((int)charCounter >= str.Length)
                    {
                        // Wait 2 seconds before next dialog
                        if ((int)time % 3 == 2)
                        {
                            currentDialogIndex++;
                            
                            if (currentDialogIndex < dialogLines.Count)
                            {
                                // Next dialog
                                str = dialogLines[currentDialogIndex];
                                charCounter = 0;
                            }
                            else
                            {
                                // Dialog complete, switch to main scenario
                                dialogComplete = true;
                                str = "Life is a game painted by the owner.\nYou just need to play and enjoy it...";
                                charCounter = 0;
                                time = 0; // Reset time for sc1-6 sequence
                            }
                        }
                    }
                }
                else
                {
                    // Main scenario phase - advance char counter
                    charCounter += 0.2f;
                    
                    // Story image shows for 3 seconds, then sc1-6
                    if ((int)time == 1)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc1");
                    }
                    else if ((int)time == 2)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc2");
                    }
                    else if ((int)time == 3)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc3");
                    }
                    else if ((int)time == 4)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc4");
                    }
                    else if ((int)time == 5)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc5");
                    }
                    else if ((int)time == 6)
                    {
                        texture = Globals.Content.Load<Texture2D>("Scenario/Starting/sc6");
                    }
                }
            }
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            string s = "";
            
            if (Level.LevelCounter == 1)
            {
                Globals.SpriteBatch.Draw(texture, textureRect, Color.White);
                Char[] array = str.ToCharArray();

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
                    
                    // Only enable skip button after main scenario text is complete
                    if (dialogComplete)
                    {
                        isKeyForStarting = true;
                        Globals.ShowCursor = true;
                    }
                }

                Vector2 p = new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString(s).X / 2, textPosition.Y + 10);
                Globals.SpriteBatch.DrawString(Globals.GameFont, s, p, Color.White);
            }

            if (isKeyForStarting)
            {
                // Position Skip button at bottom-right corner
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
