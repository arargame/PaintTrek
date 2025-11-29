using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Comet : Enemy
    {
        Vector2 targetPosition;
        public Comet()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Comet", 20, 20, 10);
            SetVelocity();

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;

                if (player == null) continue;

                targetPosition = player.position;
                break;
            }

            if (targetPosition != Vector2.Zero)
            {
                Vector2 direction = Vector2.Normalize(this.position - targetPosition);
                velocity = (-1) * direction * Globals.Random.Next(5, 7);
                double angle = Math.Atan2(-velocity.Y, -velocity.X);
                rotation = (float)angle;
            }
        }

        public override void Load()
        {

            SetTexture(GlobalTexture.cometTexture, 4, 2, 16, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;

                if (player == null) continue;

                targetPosition = player.position;
                break;
            }

            if (targetPosition != Vector2.Zero)
            {
                Vector2 direction = Vector2.Normalize(this.position - targetPosition);
                velocity = (-1) * direction * Globals.Random.Next(3, 5);
                double angle = Math.Atan2(-velocity.Y, -velocity.X);
                rotation = (float)angle;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-1, -1);
        }

        internal static Comet GetComet()
        {
            return new Comet();
        }
    }
}
