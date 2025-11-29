using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class HeartBreaker : Enemy
    {
        Vector2 targetPosition;
        Enemy owner;
        public HeartBreaker(Enemy owner)
        {
            this.owner = owner;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("HeartBreaker", 30, 30, 30);

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;

                if (player == null) continue;

                targetPosition = player.position;
                break;
            }

            SetVelocity();
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/heartBreakerSpriteSheet"));
            SetTexture(GlobalTexture.hearthBreakerTexture, 6, 1, 6, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            if (owner != null && targetPosition != Vector2.Zero)
            {
                Vector2 direction = Vector2.Normalize(owner.position - targetPosition);
                velocity = (-1) * direction * Globals.Random.Next(5, 7);
                double angle = Math.Atan2(-velocity.Y, -velocity.X);
                rotation = (float)angle;
            }
            else velocity = new Vector2(-2, 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        public override void SetStartingPosition()
        {
            if (owner != null)
            {
                if (owner.alive)
                    position = owner.position;
            }
            else
            {
                base.SetStartingPosition();
            }
        }
    }
}
