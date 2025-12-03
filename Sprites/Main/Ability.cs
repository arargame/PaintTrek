using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    enum Skills
    {
        PowerAttack,//Red Diamond
        SpeedyAttack,//Blue Diamond
        CriticalAttack,//Green Diamond
        PoisonAttack,//Black Diamond
        PixelatedAttack,
        None
    }
    class Ability
    {
        // PERFORMANS: Static texture cache - bir kez yükle, hep kullan
        private static Texture2D redLine;
        private static Texture2D blueLine;
        private static Texture2D greenLine;
        private static Texture2D blackLine;
        
        // Bullet texture cache
        private static Texture2D speedyBulletTexture;
        private static Texture2D powerBulletTexture;
        private static Texture2D poisonBulletTexture;
        private static Texture2D criticalBulletTexture;
        private static Texture2D normalLaserTexture;
        
        private static bool texturesLoaded = false;

        TimeSpan timeKeeper;
        double remainingTime;
        Skills skill;
        Player player;

        Vector2 linePosition;

        // Static constructor - texture'ları bir kez yükle
        static Ability()
        {
            LoadStaticTextures();
        }

        private static void LoadStaticTextures()
        {
            if (texturesLoaded) return;
            
            try
            {
                // Line textures
                redLine = Globals.Content.Load<Texture2D>("Textures/redLine");
                blueLine = Globals.Content.Load<Texture2D>("Textures/blueLine");
                blackLine = Globals.Content.Load<Texture2D>("Textures/blackLine");
                greenLine = Globals.Content.Load<Texture2D>("Textures/greenLine");
                
                // Bullet textures
                speedyBulletTexture = Globals.Content.Load<Texture2D>("Guns/PlayerBullet/speedyBullet");
                powerBulletTexture = Globals.Content.Load<Texture2D>("Guns/PlayerBullet/powerBullet");
                poisonBulletTexture = Globals.Content.Load<Texture2D>("Guns/PlayerBullet/poisonBullet");
                criticalBulletTexture = Globals.Content.Load<Texture2D>("Guns/PlayerBullet/criticalBullet");
                normalLaserTexture = Globals.Content.Load<Texture2D>("Guns/laser");
                
                texturesLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ability: Texture loading error: {ex.Message}");
            }
        }

        public Ability(Player player)
        {
            skill = Skills.None;
            this.player = player;
            Load();
        }

        public Ability(Diamond diamond, Player player)
        {
            skill = Skills.None;
            this.player = player;

            if (diamond != null)
            {
                Style style = diamond.ShowStyle();
                SelectSkill(style);
            }

            Load();
        }

        public void Load()
        {
            // Texture'lar static olarak yüklendi, sadece instance değişkenleri ayarla
            timeKeeper = Time.TotalGameTime();
            linePosition = new Vector2(15, 40);
            remainingTime = Globals.Random.Next(5, 15);
        }

        public void Update()
        {

            switch (skill)
            {
                case Skills.PowerAttack:
                    PowerAttack();
                    break;
                case Skills.SpeedyAttack:
                    SpeedyAttack();
                    break;
                case Skills.CriticalAttack:
                    CriticalAttack();
                    break;
                case Skills.PoisonAttack:
                    PoisonAttack();
                    break;
                case Skills.None:
                    Normalize();
                    break;
            }

            if (remainingTime == 0)
            {
                skill = Skills.None;
            }

            if (Time.TotalGameTime() - timeKeeper > TimeSpan.FromSeconds(1.0f))
            {
                timeKeeper = Time.TotalGameTime();

                remainingTime--;

                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                }
            }

        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            // PERFORMANS: Cache'lenmiş texture kullan, her frame yükleme yok
            Texture2D t = blackLine;
            switch (skill)
            {
                case Skills.PowerAttack:
                    t = redLine;
                    break;
                case Skills.SpeedyAttack:
                    t = blueLine;
                    break;
                case Skills.CriticalAttack:
                    t = blackLine;
                    break;
                case Skills.PoisonAttack:
                    t = greenLine;
                    break;
                case Skills.None:
                    t = blackLine;
                    break;
            }

            if (skill != Skills.None && t != null)
            {
                for (int i = 0; i < remainingTime; i++)
                {
                    Globals.SpriteBatch.Draw(t, new Rectangle((int)linePosition.X + (i * 5), (int)linePosition.Y, 10, 10), Color.White);
                }
            }

            Globals.SpriteBatch.End();
        }

        public Skills GetSkill()
        {
            return skill;
        }

        private void SpeedyAttack()
        {
            Bullet b = player.GetGun();
            // PERFORMANS: Cache'lenmiş texture kullan
            b.SetTexture(speedyBulletTexture, 1, 1, 1, true);
            (b as Laser).SetVelocity(new Vector2(25, 0));
            b.SetDamage(Globals.Random.Next(10, 25));
        }

        private void PowerAttack()
        {
            Bullet b = player.GetGun();
            // PERFORMANS: Cache'lenmiş texture kullan
            b.SetTexture(powerBulletTexture, 1, 1, 1, true);
            b.SetDamage(Globals.Random.Next(25, 40));
        }

        private void PoisonAttack()
        {
            Bullet b = player.GetGun();
            // PERFORMANS: Cache'lenmiş texture kullan
            b.SetTexture(poisonBulletTexture, 1, 1, 1, true);
        }

        private void CriticalAttack()
        {
            Bullet b = player.GetGun();
            // PERFORMANS: Cache'lenmiş texture kullan
            b.SetTexture(criticalBulletTexture, 1, 1, 1, true);
            (b as Laser).SetVelocity(new Vector2(20, 0));
            b.SetDamage(Globals.Random.Next(50, 100));
        }

        private void Normalize()
        {
            Bullet b = player.GetGun();
            b.color = Color.White;
            // PERFORMANS: Cache'lenmiş texture kullan
            b.SetTexture(normalLaserTexture, 1, 1, 1, true);
        }

        public void SelectSkill(Style style)
        {
            if (style == Style.Blue)
            {
                skill = Skills.SpeedyAttack;
            }
            else if (style == Style.Red)
            {
                skill = Skills.PowerAttack;
            }
            else if (style == Style.Green)
            {
                skill = Skills.PoisonAttack;
            }
            else if (style == Style.Black)
            {
                skill = Skills.CriticalAttack;
            }

        }
    }
}
