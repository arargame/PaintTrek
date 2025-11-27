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
        List<string> keyLabels = new List<string>();

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
            
            backButton = new BackButton("Back", this, true);
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

            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            Globals.SpriteBatch.Begin();
            
            // Draw "Programming and Graphics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Programming and Graphics :", position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Koray Arar" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Koray Arar", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing * 2;
            
            // Draw "Musics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Musics :", position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Sahin Meric" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Sahin Meric", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            
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
