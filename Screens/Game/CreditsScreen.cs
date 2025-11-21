using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class CreditsScreen:GameScreen
    {
        List<string> credits = new List<string>();

        public CreditsScreen() 
        {
            Initialize();
        }

        ~CreditsScreen() 
        {
        
        }

        public override void Initialize()
        {
            base.Initialize();

            screenTitle = "Credits Screen";
            Globals.Window.Title = screenTitle;

            string str = "Programming and Graphics :\n";
            str += "\n    Koray Arar\n";
            str += "\n";
            str += "Musics :\n";
            str += "\n    Sahin Meric\n";

            credits.Add(str);
            backButton = new BackButton("Back", this, true);
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Vector2 position = new Vector2(100, 200);
            Vector2 origin = new Vector2(0, Globals.GameFont.LineSpacing / 2);
            Color buttonColor = Color.White, descriptionColor = Color.White;

            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            Globals.SpriteBatch.Begin();
            for (int i = 0; i < credits.Count; i++)
            {
                Globals.SpriteBatch.DrawString(Globals.GameFont, credits[i], position, buttonColor, 0, origin, 1f, SpriteEffects.None, 0);

                position.Y += Globals.GameFont.LineSpacing;
            }
            Globals.SpriteBatch.End();

            base.Draw();
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
