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
            SetTextures(GlobalTexture.bouncingFireCollectionTexture);
            animation = new Animation(texture, 1, 1, 1, true);
        }

        public override void Update()
        {

            SimpleMovement(velocity);

           // time -= Globals.GameTime.ElapsedGameTime.TotalSeconds;
           // if (time <= 0) alive = false;

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
           // Vector2 measure = Globals.GameFont.MeasureString(((int)time).ToString());
            Globals.SpriteBatch.Begin();
           // Globals.SpriteBatch.DrawString(Globals.GameFont, ((int)time).ToString(), new Vector2(position.X, position.Y), Color.Black);
            Globals.SpriteBatch.End();
        }
        public override void SetVelocity()
        {
            velocity = new Vector2(-Globals.Random.Next(1,3),0);
            if (Globals.Graphics.IsFullScreen)
            {
                velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
            }
        }
       /* public int GiveTime()
        {
          //  return (int)time;
        }*/

        public override void SetStartingPosition()
        {
            position = new Vector2(Globals.Random.Next((int)(Globals.GameSize.X + size.X / 4), (int)(Globals.GameSize.X * 2)), Globals.Random.Next((int)size.X,(int)(Globals.GameSize.Y-size.Y)));
        }
        internal static BouncingFireCollection GetCollactableBouncingBall()
        {
            return new BouncingFireCollection();
        }
    }
}
