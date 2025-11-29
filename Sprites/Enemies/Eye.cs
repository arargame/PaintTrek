using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Eye : Enemy
    {
        public Eye()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Eye", 10, 10, 10);
            SetVelocity();


        }

        public override void Load()
        {
            SetTexture(GlobalTexture.eyeTexture, 4, 1, 5, true);
        }

        public override void Update()
        {
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
            base.SetStartingPosition();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(Globals.Random.Next(-10, -2), 0);

            if (Globals.Graphics.IsFullScreen)
                velocity.X = (velocity.X * 1280) / 800;
        }

        public override void SimpleMovement(Vector2 amount)
        {
            Vector2 playerPosition = Vector2.Zero;

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;

                if (player != null)
                    playerPosition = player.position;
            }

            if (playerPosition == Vector2.Zero)
            {
                amount.X = -2;
            }
            else
            {

                if (position.Y > playerPosition.Y)
                    position.Y--;

                if (position.Y < playerPosition.Y)
                    position.Y++;
            }

            position.X += amount.X;

        }

        internal static Eye GetEyes()
        {
            return new Eye();
        }
    }
}
