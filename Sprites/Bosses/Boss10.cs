using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss10 : Boss
    {
        Player target;
        Boss10Fire bossFire;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        enum MovementStyle
        {
            Starting,
            InverseMove
        }
        MovementStyle movementStyle;

        public Boss10()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 10", 1500, 30, 10000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            this.spriteEffect = SpriteEffects.FlipHorizontally;
            movementStyle = MovementStyle.Starting;

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;
                if (player != null)
                {
                    target = player;
                }
            }

            bossFire = new Boss10Fire(this);
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss10SpriteSheet"));
            SetTexture(GlobalTexture.boss10Texture, 4, 2, 8, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (movementStyle == MovementStyle.Starting && position.X <= Globals.GameSize.X - size.X / 2)
            {
                velocity = Vector2.Zero;
                movementStyle = MovementStyle.InverseMove;
            }

            if (movementStyle == MovementStyle.InverseMove)
            {
                if (target.movementStyle == Player.MovementStyle.movingToUp)
                {
                    velocity.Y = 6;
                }
                else if (target.movementStyle == Player.MovementStyle.movingToDown)
                {
                    velocity.Y = -6;
                }

                if (position.Y + size.Y / 2 <= 0)
                {
                    position.Y = Globals.GameSize.Y - size.Y / 2;
                }
                else if (position.Y >= Globals.GameSize.Y)
                {
                    position.Y = size.Y / 2;
                }
            }


            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            if (fireTime > Globals.Random.Next(1, 3))
            {
                fireTime = 0;
                bossFire = new Boss10Fire(this);
                if (canFire)
                {
                    bossFire.Fire();
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
            velocity = new Vector2(-2, 0);
        }

        internal static Boss10 GetBoss10()
        {
            return new Boss10();
        }
    }
}
