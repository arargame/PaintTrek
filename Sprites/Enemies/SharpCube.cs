using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class SharpCube : Enemy
    {
        double d;
        public SharpCube()
        {
            Initialize();

        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Sharp Cube", 30, 30, 10);
            SetVelocity();
        }

        public override void Load()
        {
            //SetTextures(GlobalTexture.sharpCubeTexture);
            SetTexture(GlobalTexture.sharpCubeTexture, 5, 1, 5, true);
        }

        public override void Update()
        {
            d++;
            float angle = (float)Math.Sin(d * MathHelper.Pi / 180);
            velocity.Y = ((float)Math.Sin(angle)) * 2;
            //velocity.Y = ((float)Math.Tan(angle))*2;

            SimpleMovement(velocity);
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Kill()
        {
            base.Kill();
        }

        public override void MakeVisible()
        {
            base.MakeVisible();
        }

        public override void SetStartingPosition()
        {
            position = new Vector2(Globals.Random.Next((int)(Globals.GameSize.X + size.X / 2), (int)(Globals.GameSize.X + size.X * 2)), Globals.Random.Next((int)(Globals.GameSize.Y *0.25 ),(int)(Globals.GameSize.Y*0.75)));
        }

        public override void SetVelocity()
        {
            velocity.X = -1 * Globals.Random.Next(2, 8);
            velocity.Y = 0;

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static SharpCube GetSharpCube()
        {
            return new SharpCube();
        }
    }
}
