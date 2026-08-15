using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    class HeadUpDisplay
    {
        Player player;
        Texture2D healthBarTexture;
        Texture2D healthBarLine;
        Texture2D pixel;
        Rectangle healthRect;
        double maxHealt;
        Vector2 scorePosition;
        MobileController mobileController;


        public HeadUpDisplay(Player player)
        {
            this.player = player;
            healthBarTexture = Globals.Content.Load<Texture2D>("Textures/HealthBar");
            healthBarLine = Globals.Content.Load<Texture2D>("Textures/LoadBar");
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");
            healthRect = new Rectangle((int)(Globals.GameSize.X * 0.08), (int)(Globals.GameSize.Y * 0.08), (int)player.GetHealth(), (int)healthBarTexture.Height / 2);
            string scoreText = string.Format(Loc.T(LocKeys.Gameplay.Score), Level.Score);
            scorePosition = new Vector2(Globals.GameSize.X - Globals.GameFont.MeasureString(scoreText).X * 2, healthRect.Top);
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
            string scoreText = string.Format(Loc.T(LocKeys.Gameplay.Score), Level.Score);
            scorePosition = new Vector2(Globals.GameSize.X - Globals.GameFont.MeasureString(scoreText).X - Globals.GameFont.MeasureString(scoreText).X / 2, healthRect.Top);

            #if WINDOWS_PHONE
            mobileController.Update(this.player);
            #endif

            mobileController.Update(this.player);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            //HP Bar Drawing
            Globals.SpriteBatch.DrawString(Globals.GameFont, Loc.T(LocKeys.Gameplay.HP) + " ", new Vector2((int)(Globals.GameSize.X * 0.01), (int)(Globals.GameSize.Y * 0.01)), Color.White);
            
            // Düz Arka Plan Çerçevesi (Koyu Kırmızımsı Gri)
            Globals.SpriteBatch.Draw(pixel, new Rectangle(healthRect.X, healthRect.Y, (int)maxHealt * 2, healthRect.Height), Color.DarkRed * 0.3f);
            
            // Düz Can Dolgusu (Parlak Kırmızı)
            Globals.SpriteBatch.Draw(pixel, healthRect, Color.Red);
            
            // İnce Düz Kenarlık (1px Gri Kenarlık)
            // Üst
            Globals.SpriteBatch.Draw(pixel, new Rectangle(healthRect.X - 1, healthRect.Y - 1, (int)maxHealt * 2 + 2, 1), Color.Gray);
            // Alt
            Globals.SpriteBatch.Draw(pixel, new Rectangle(healthRect.X - 1, healthRect.Y + healthRect.Height, (int)maxHealt * 2 + 2, 1), Color.Gray);
            // Sol
            Globals.SpriteBatch.Draw(pixel, new Rectangle(healthRect.X - 1, healthRect.Y - 1, 1, healthRect.Height + 2), Color.Gray);
            // Sağ
            Globals.SpriteBatch.Draw(pixel, new Rectangle(healthRect.X + (int)maxHealt * 2, healthRect.Y - 1, 1, healthRect.Height + 2), Color.Gray);

            //Score Drawing
            Globals.SpriteBatch.DrawString(Globals.GameFont, string.Format(Loc.T(LocKeys.Gameplay.Score), Level.Score), scorePosition, Color.Yellow);

            Globals.SpriteBatch.End();

            #if WINDOWS_PHONE
            mobileController.Draw();
            #endif

            mobileController.Draw();
        }

  
    }
}
