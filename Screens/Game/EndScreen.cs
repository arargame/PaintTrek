using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class EndScreen:GameScreen
    {
        double time;
        Texture2D texture;
        Vector2 textPosition;
        Rectangle textureRect;
        string str;
        double charCounter;
        bool isKeyForStarting;
        string congratulate;
        bool hide;

        public EndScreen() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "End Screen";
            Globals.Window.Title = screenTitle;
            time = 0;
            charCounter = 0;

            texture = Globals.Content.Load<Texture2D>("Scenario/Finish/endScene");
            str = "When you reach to the end,it doesn't matter how you come.\nThe owner will bring you a new scenario,so you have to be ready for this...";

            textureRect = new Rectangle(0,0,(int) Globals.GameSize.X, texture.Height);
            textPosition = new Vector2(textureRect.Left, textureRect.Bottom);

            backButton = new BackButton("Return To Main Menu", this, true);
            isKeyForStarting = false;
            congratulate = "Congratulations,You Won.Your Score is "+Level.Score;
            hide = true;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {

            base.Update();

            time += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            charCounter += 0.2f;

            if (isKeyForStarting)
            {
                if ((int)time % 3 == 0) hide = true;
                else hide = false;
            }
           
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            if (isKeyForStarting) backButton.Draw();

            Globals.SpriteBatch.Begin();

            if (isKeyForStarting && !hide)
            {
                Globals.SpriteBatch.DrawString(Globals.GameFont,congratulate,new Vector2(Globals.GameSize.X/2-Globals.GameFont.MeasureString(congratulate).X/2,Globals.GameSize.Y/2),Color.Red);
            }

            if (Level.LevelCounter == 10)
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
                { s = str; isKeyForStarting = true; }

                Vector2 p = new Vector2(Globals.GameSize.X / 2 - Globals.GameFont.MeasureString(s).X / 2, textPosition.Y + 10);
                Globals.SpriteBatch.DrawString(Globals.GameFont, s, p, Color.White);
            }

            Globals.SpriteBatch.End();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }
    }
}
