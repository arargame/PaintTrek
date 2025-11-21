using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Asteroid : Enemy
    {
        float r;
        public Asteroid()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            SetVelocity();
            SetCharacterInfo("Asteroid", 50, 30, 10);
            r = (float)Globals.Random.NextDouble();
            if (r < 0.5f) r = 0.5f;
        }
        public override void Load()
        {
           //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/asteroidSpriteSheet"));
            SetTextures(GlobalTexture.astreoidTexture);
            animation = new Animation(texture, 1, 1, 1, false);
        }
        public override void Update()
        {
            SimpleMovement(velocity);
            base.Update();
            Rotate(r);
        }
        public override void Draw()
        {
            base.Draw();
        }

        public override void MakeVisible()
        {
            base.MakeVisible();
        }

        public override void Kill()
        {
            base.Kill();
        }
        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }
        public override void SetVelocity()
        {
            this.velocity = new Vector2(-2, 0);

            if (Globals.Graphics.IsFullScreen)
                velocity.X = (velocity.X * 1280) / 800;
        }
        internal static Asteroid GetAstreoid()
        {
            return new Asteroid();
        }

    }
}
