using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Bristle : Enemy
    {
        DiffusedLightSystem dls;
        double d;
        float degree;

        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;
        int fireSpeed;

        public Bristle()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Bristle", 50, 20, 10);
            SetVelocity();
            fireSpeed = 5;
            dls = new DiffusedLightSystem(this, 8, "StarFire", fireSpeed);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
            degree = (float)Globals.Random.NextDouble();
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/bristleSpriteSheet"));
            SetTextures(GlobalTexture.bristleTexture);
            animation = new Animation(texture, 3, 1, 6, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            d++;
            float angle = (float)Math.Sin(d * MathHelper.Pi / 180);
            velocity.Y = ((float)Math.Sin(angle)) * 2;

            FireSettings();
            Rotate(degree);
        }

        public void FireSettings()
        {
           // dls.Reload();

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(2, 6))
            {
                fireTime = 0;
                dls = new DiffusedLightSystem(this, 8, "StarFire", fireSpeed);
                if (canFire)
                {
                    dls.Fire();
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity.X = -1 * Globals.Random.Next(1, 3);
            velocity.Y = 0;

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        public override void SetStartingPosition()
        {
            //position = new Vector2(Globals.Random.Next((int)(Globals.GameSize.X + size.X / 2),(int)(Globals.GameSize.X + size.X * 2)),
                //Globals.Random.Next((int)animation.Height/2,(int)(Globals.GameSize.Y - animation.Height /2)));
            position = new Vector2(Globals.Random.Next((int)(Globals.GameSize.X + size.X / 2), (int)(Globals.GameSize.X + size.X * 2)),
             Globals.Random.Next(0, (int)Globals.GameSize.Y / 2));
            //base.SetStartingPosition();
        }
        internal static Bristle GetBristle()
        {
            return new Bristle();
        }
    }
}
