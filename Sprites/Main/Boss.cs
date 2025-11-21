using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    abstract class Boss : Enemy
    {
        Texture2D hpTexture;
        Texture2D hpTextureBg;
        Vector2 hpIndicatorPosition;
        double startingHealth;
        Texture2D damageTexture; 

        public override void Load()
        {
            base.Load();

        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss", 100, 50, 100);
            BossSystem.Add(this);
            hpTexture = Globals.Content.Load<Texture2D>("Textures/bossHpBar");
            hpTextureBg = Globals.Content.Load<Texture2D>("Textures/bossHealthBarBackground");
            hpIndicatorPosition = new Vector2(Globals.GameSize.X * 0.3f, Globals.GameSize.Y * 0.9f);

            if (Globals.Graphics.IsFullScreen)
            {
                scale = 1.6f;
            }
        }
        public override void Update()
        {
            base.Update();

            if (!alive)
            {
                BossSystem.Remove(this);
            }

        }
        public override void Draw()
        {
            base.Draw();

            Globals.SpriteBatch.Begin();
            Vector2 p = new Vector2(hpIndicatorPosition.X, hpIndicatorPosition.Y);
            //(int)hpIndicatorPosition.X+i*2, (int)hpIndicatorPosition.Y
            //         Globals.SpriteBatch.Draw(hpTextureBg, new Rectangle((int)(p.X - Globals.GameFont.MeasureString("Boss :").X - 10), (int)(p.Y - Globals.GameFont.MeasureString("Boss :").Y / 3),(int) ((HpExecute()*5)+ (Globals.GameFont.MeasureString("Boss :").X + 10)),30), Color.White);
            for (int i = 0; i < HpExecute(); i++)
            {
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Boss :", new Vector2(p.X - Globals.GameFont.MeasureString("Boss :").X - 10, p.Y - Globals.GameFont.MeasureString("Boss :").Y / 3), Color.White);
                Globals.SpriteBatch.Draw(hpTexture, new Rectangle((int)p.X + i * 5, (int)p.Y, 1, 10), Color.White);
            }
            Globals.SpriteBatch.End();
        }

        private int HpExecute()
        {
            int hp = (int)GetHealth();
            if (hp <= 0) hp = 0;

            int indicatorBarCount = 0;

            if (startingHealth > 0)
                indicatorBarCount = Convert.ToInt32((100 * hp) / startingHealth);

            return indicatorBarCount;
        }

        public void FetchStartingHealth(double health)
        {
            this.startingHealth = health;
        }

        public override int GetPoint()
        {
            return base.GetPoint() * Level.LevelCounter;
        }

        public override void SetTextures(Texture2D newTexture)
        {
            base.SetTextures(newTexture);
            damageTexture = MakeDamageTexture(normalTexture);
        }

        public override void TakeDamage(Sprite another)
        {
            texture = damageTexture;
            isTakingDamage = true;
            SetHealth(-10.0);
        }
    }
}
