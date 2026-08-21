using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class BouncingFireCollection:Supply
    {
        //double time;

        public BouncingFireCollection() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Collectable Bouncing Ball Object", 50, 0, 10);
            SetVelocity();
          //  time = Globals.Random.Next(30, 60);
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/bouncingBallSupply"));
            SetTexture(GlobalTexture.bouncingFireCollectionTexture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            // Bouncing logic - check boundaries and reverse velocity
            // Left boundary
            if (position.X - size.X / 2 <= 0)
            {
                ReverseNaturalVelocityX();
                position.X = size.X / 2; // Prevent sticking to edge
            }

            // Right boundary
            if (position.X + size.X / 2 >= Globals.GameSize.X)
            {
                ReverseNaturalVelocityX();
                position.X = Globals.GameSize.X - size.X / 2;
            }

            // Top boundary
            if (position.Y - size.Y / 2 <= 0)
            {
                ReverseNaturalVelocityY();
                position.Y = size.Y / 2;
            }

            // Bottom boundary - THIS WAS THE BUG!
            if (position.Y + size.Y / 2 >= Globals.GameSize.Y)
            {
                ReverseNaturalVelocityY();
                position.Y = Globals.GameSize.Y - size.Y / 2; // Prevent sticking to bottom
            }

        }

        public override void Draw()
        {
            base.Draw();
        }
        public override void SetVelocity()
        {
            // Supply.Update integrates this velocity with delta time and magnet pull.  The old
            // -1/-2 per-frame assignment overwrote that vector and left the ball almost static.
            SetNaturalVelocity(new Vector2(-Globals.Random.Next(135, 191), Globals.Random.Next(-85, 86)));
        }
       /* public int GiveTime()
        {
          //  return (int)time;
        }*/

        public override void SetStartingPosition()
        {
            position = new Vector2(Globals.Random.Next((int)Globals.GameSize.X, (int)(Globals.GameSize.X * 1.5f)), Globals.Random.Next((int)size.X,(int)(Globals.GameSize.Y-size.Y)));
        }
        internal static BouncingFireCollection GetCollactableBouncingBall()
        {
            return new BouncingFireCollection();
        }
    }
}
