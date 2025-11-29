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
    class Abilitiy
    {

        TimeSpan timeKeeper;
        double remainingTime;
        Skills skill;
        Player player;

        Vector2 linePosition;
        Texture2D redLine;
        Texture2D blueLine;
        Texture2D greenLine;
        Texture2D blackLine;

        public Abilitiy(Player player)
        {
            skill = Skills.None;
            this.player = player;
            Load();

        }

        public Abilitiy(Diamond diamond, Player player)
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

            timeKeeper = Time.TotalGameTime();

            redLine = Globals.Content.Load<Texture2D>("Textures/redLine");
            blueLine = Globals.Content.Load<Texture2D>("Textures/blueLine");
            blackLine = Globals.Content.Load<Texture2D>("Textures/blackLine");
            greenLine = Globals.Content.Load<Texture2D>("Textures/greenLine");
            //linePosition = new Vector2((float)(Globals.GameSize.X*0.01),(float)(Globals.GameSize.Y*0.95));
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
            Texture2D t = Globals.Content.Load<Texture2D>("Textures/blackLine");
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

            if (skill != Skills.None)
                for (int i = 0; i < remainingTime; i++)
                {
                    Globals.SpriteBatch.Draw(t, new Rectangle((int)linePosition.X + (i * 5), (int)linePosition.Y, 10, 10), Color.White);
                }

            Globals.SpriteBatch.End();
        }

        public Skills GetSkill()
        {
            return skill;
        }

        /*       void time_Elapsed(object sender, ElapsedEventArgs e)
               {
                   remainingTime--;

                   if (remainingTime <= 0)
                   {
                       //time.Stop();
                       remainingTime = 0;
                   }
               }*/

        private void SpeedyAttack()
        {
            Bullet b = player.GetGun();
         //   b.color = Color.Blue;
            b.SetTexture(Globals.Content.Load<Texture2D>("Guns/PlayerBullet/speedyBullet"), 1, 1, 1, true);
            (b as Laser).SetVelocity(new Vector2(25, 0));
            b.SetDamage(Globals.Random.Next(10, 25));
        }

        private void PowerAttack()
        {
            Bullet b = player.GetGun();
           // b.color = Color.Red;
            b.SetTexture(Globals.Content.Load<Texture2D>("Guns/PlayerBullet/powerBullet"), 1, 1, 1, true);
            b.SetDamage(Globals.Random.Next(25, 40));
        }

        private void PoisonAttack()
        {
            Bullet b = player.GetGun();
           // b.color = Color.Green;
            b.SetTexture(Globals.Content.Load<Texture2D>("Guns/PlayerBullet/poisonBullet"), 1, 1, 1, true);
        }

        private void CriticalAttack()
        {
            Bullet b = player.GetGun();
            //b.color = Color.Black;
            b.SetTexture(Globals.Content.Load<Texture2D>("Guns/PlayerBullet/criticalBullet"), 1, 1, 1, true);
            (b as Laser).SetVelocity(new Vector2(20, 0));
            b.SetDamage(Globals.Random.Next(50, 100));
        }

        private void Normalize()
        {
            Bullet b = player.GetGun();
            b.color = Color.White;
            b.SetTexture(Globals.Content.Load<Texture2D>("Guns/laser"), 1, 1, 1, true);
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
