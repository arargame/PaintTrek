using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Boss1 : Boss
    {
        BossFire1 bossFire;

        public Boss1()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 1", 1000, 30, 1000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            bossFire = new BossFire1(this);
        }

        public override void Load()
        {
            SetTexture(GlobalTexture.boss1Texture, 3, 1, 10, true);
        }

        public override void Update()
        {
            base.Update();

            Rectangle rect = Globals.GameRect;
            rect.X = (int)(rect.X - this.size.X + 10);

            if (!rect.Intersects(this.destinationRectangle))
            {
                this.velocity = new Vector2(-1, 0);
            }
            else
                this.velocity = Vector2.Zero;

            SimpleMovement(velocity);

            if (!bossFire.alive)
                bossFire = new BossFire1(this);

            if (visible)
                bossFire.Fire();

        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetStartingPosition()
        {
            float width = Globals.GameSize.X;
            float height = Globals.GameSize.Y / 2 - this.size.Y / 2;
            position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), height);
            position.X = MathHelper.Clamp(position.X, width, width + width / 3);
        }

        public override void SetVelocity()
        {
            base.SetVelocity();
        }

        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
        }

        internal static Boss1 GetBoss1()
        {
            return new Boss1();
        }
    }
}
