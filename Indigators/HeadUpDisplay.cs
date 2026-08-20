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
        LevelBuilder levelBuilder;


        public HeadUpDisplay(Player player, LevelBuilder levelBuilder)
        {
            this.player = player;
            this.levelBuilder = levelBuilder;
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
            DrawStageProgress();

            Globals.SpriteBatch.End();

            #if WINDOWS_PHONE
            mobileController.Draw();
            #endif

            mobileController.Draw();
        }

        private void DrawStageProgress()
        {
            int bossSize = 34;
            int barWidth = Math.Min(420, Math.Max(180, (int)(Globals.GameSize.X * 0.42f)));
            int barHeight = 10;
            int contentWidth = barWidth + bossSize + 14;
            int barX = ((int)Globals.GameSize.X - contentWidth) / 2;
            int barY = (int)Globals.GameSize.Y - 44;
            Rectangle outer = new Rectangle(barX, barY, barWidth, barHeight);
            Rectangle inner = new Rectangle(barX + 1, barY + 1, barWidth - 2, barHeight - 2);
            int fillWidth = (int)(inner.Width * levelBuilder.StageProgress);

            Globals.SpriteBatch.Draw(pixel, outer, Color.White * 0.85f);
            Globals.SpriteBatch.Draw(pixel, inner, Color.Black * 0.55f);
            if (fillWidth > 0)
                Globals.SpriteBatch.Draw(pixel, new Rectangle(inner.X, inner.Y, fillWidth, inner.Height), Color.White);

            int percentage = (int)(levelBuilder.StageProgress * 100f);
            string percentageText = percentage + "%";
            Vector2 textSize = Globals.GameFont.MeasureString(percentageText);
            Globals.SpriteBatch.DrawString(Globals.GameFont, percentageText,
                new Vector2(barX + (barWidth - textSize.X) / 2, barY - textSize.Y - 4), Color.White);

            DrawBossPreview(new Rectangle(outer.Right + 14, barY - (bossSize - barHeight) / 2, bossSize, bossSize));
        }

        private void DrawBossPreview(Rectangle destination)
        {
            Texture2D texture = GetBossPreviewTexture(out int columns, out int rows);
            if (texture == null)
                return;

            int frameCount = columns * rows;
            int frame = (int)(Globals.GameTime.TotalGameTime.TotalSeconds * 5d) % frameCount;
            int frameWidth = texture.Width / columns;
            int frameHeight = texture.Height / rows;
            Rectangle source = new Rectangle((frame % columns) * frameWidth, (frame / columns) * frameHeight, frameWidth, frameHeight);
            Globals.SpriteBatch.Draw(texture, destination, source, Color.White);
        }

        private Texture2D GetBossPreviewTexture(out int columns, out int rows)
        {
            columns = 1;
            rows = 1;

            switch (levelBuilder.UpcomingBossKey)
            {
                case "Boss1": columns = 3; return GlobalTexture.boss1Texture;
                case "Boss2": columns = 5; return GlobalTexture.boss2Texture;
                case "Boss3": columns = 6; return GlobalTexture.boss3Texture;
                case "Boss4": rows = 3; return GlobalTexture.snakeStoneLeftTexture;
                case "Boss5": columns = 4; return GlobalTexture.boss5Texture;
                case "Boss6": columns = 3; return GlobalTexture.boss6Texture;
                case "Boss7": rows = 5; return GlobalTexture.boss7Texture;
                case "Boss8": columns = 3; return GlobalTexture.boss8Texture;
                case "Boss9": columns = 3; rows = 2; return GlobalTexture.boss9Texture;
                case "Boss10": columns = 4; rows = 2; return GlobalTexture.boss10Texture;
                default: return null;
            }
        }

  
    }
}
