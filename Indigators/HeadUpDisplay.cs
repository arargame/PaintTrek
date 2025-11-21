using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class HeadUpDisplay
    {
        Player player;
        Texture2D healthBarTexture;
        Texture2D healthBarLine;
        Rectangle healthRect;
        double maxHealt;
        Vector2 scorePosition;
        MobileController mobileController;


        public HeadUpDisplay(Player player)
        {
            this.player = player;
            healthBarTexture = Globals.Content.Load<Texture2D>("Textures/HealthBar");
            healthBarLine = Globals.Content.Load<Texture2D>("Textures/LoadBar");
            healthRect = new Rectangle((int)(Globals.GameSize.X * 0.08), (int)(Globals.GameSize.Y * 0.08), (int)player.GetHealth(), (int)healthBarTexture.Height / 2);
            scorePosition = new Vector2(Globals.GameSize.X - Globals.GameFont.MeasureString("Score : " + Level.Score).X * 2, healthRect.Top);
            maxHealt = player.FetchStartingHealth();

            #if WINDOWS_PHONE
                mobileController = new MobileController();
            #endif

            mobileController = new MobileController();

        }
        ~HeadUpDisplay() 
        {
        }

        public void Update()
        {
            healthRect = new Rectangle((int)(Globals.GameSize.X * 0.05), (int)(Globals.GameSize.Y * 0.02), (int)player.GetHealth() * 2, (int)healthBarTexture.Height / 2);
            scorePosition = new Vector2(Globals.GameSize.X - Globals.GameFont.MeasureString("Score : " + Level.Score).X - Globals.GameFont.MeasureString("Score : " + Level.Score).X / 2, healthRect.Top);

            #if WINDOWS_PHONE
            mobileController.Update(this.player);
            #endif

            mobileController.Update(this.player);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            //HP Bar Drawing
            Globals.SpriteBatch.DrawString(Globals.GameFont, "HP ", new Vector2((int)(Globals.GameSize.X * 0.01), (int)(Globals.GameSize.Y * 0.01)), Color.White);
            Globals.SpriteBatch.Draw(healthBarTexture, healthRect, Color.White);
            Globals.SpriteBatch.Draw(healthBarLine, new Rectangle(healthRect.X, healthRect.Y, (int)maxHealt * 2, healthBarTexture.Height / 2), Color.White);

            //Score Drawing
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Score : " + Level.Score, scorePosition, Color.Yellow);

            Globals.SpriteBatch.End();

            #if WINDOWS_PHONE
            mobileController.Draw();
            #endif

            mobileController.Draw();
        }

  
    }
}
