using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{

    class ChildTrilobit : Enemy
    {
        public enum MovementStyle
        {
            spawned,
            chasing
        }
        MovementStyle movementStyle;
        double timer;
        Vector2 targetPosition;

        public ChildTrilobit()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Egg", 10, 10, 10);
            SetVelocity();
            movementStyle = MovementStyle.spawned;
            timer = 0;
        }

        public override void Load()
        {
            //   SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/trilobitChildSpriteSheet"));
            //  animation = new Animation(texture, 1, 4, 4, true);
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/childTrilobitEggSpriteSheet"));
            SetTexture(GlobalTexture.childTrilobitEggTexture, 3, 1, 3, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            timer += Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (movementStyle == MovementStyle.spawned && (int)timer >= 5)
            {
                movementStyle = MovementStyle.chasing;
                //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/trilobitChildSpriteSheet"));
                SetTexture(GlobalTexture.childTrilobitTexture, 1, 4, 4, true);
                SetCharacterInfo("Child Trilobit", 30, 10, 10);
            }

            if (movementStyle == MovementStyle.chasing)
            {

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
                    velocity = (-1) * direction * Globals.Random.Next(2, 4);
                    double angle = Math.Atan2(-velocity.Y, -velocity.X);
                    rotation = (float)angle;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = Vector2.Zero;
        }

        public override void SetStartingPosition()
        {
            position = new Vector2((float)Globals.Random.Next(0, (int)(Globals.GameSize.X - size.X / 2)), (float)Globals.Random.Next(0, (int)(Globals.GameSize.Y - size.Y / 2)));
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
        }
        public MovementStyle GetMovementStyle()
        {
            return movementStyle;
        }

        internal static ChildTrilobit GetChildTrilobit()
        {
            return new ChildTrilobit();
        }

    }
}
