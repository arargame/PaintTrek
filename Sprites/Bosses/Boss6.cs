using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Boss6 : Boss
    {
        enum MovementStyle
        {
            Starting,
            SmoothTravelling
        }

        MovementStyle movementStyle;
        Vector2 pointA;
        Vector2 pointB;
        float amount;
        float moveTime;
        float speedVariation;
        float baseSpeed;

        public Boss6()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Boss 6", 2000, 15, 6000);
            FetchStartingHealth(GetHealth());
            SetVelocity();
            speedVariation = 2f;
            baseSpeed = 3f;
            movementStyle = MovementStyle.Starting;
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Boss/boss6SpriteSheet"));
            SetTextures(GlobalTexture.boss6Texture);
            animation = new Animation(texture, 3, 1, 3, true);
        }

        public override void Update()
        {
            base.Update();

            if (movementStyle == MovementStyle.Starting && position.X < Globals.GameSize.X - size.X)
            {
                movementStyle = MovementStyle.SmoothTravelling;
            }

            if (movementStyle == MovementStyle.SmoothTravelling)
            {
                if (moveTime <= 0)
                    SetRandomMove();
                //amount+=(float)Globals.GameTime.ElapsedGameTime.TotalSeconds/moveTime;
                amount += 0.02f;
                this.position = Vector2.Lerp(pointA, pointB, amount);

                if (this.amount >= 1f)
                    this.SetRandomMove();
            }
            else
            {
                SimpleMovement(velocity);
            }
            Rotate(0.5f);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-2, 0);
        }
        private void SetRandomMove()
        {
            this.pointA = this.position;
            this.pointB = GetRandomScreenPosition(50);
            this.moveTime = baseSpeed + Range(-speedVariation, speedVariation);
            this.amount = 0f;
        }
        private Vector2 GetRandomScreenPosition(int padding)
        {
            return new Vector2(Globals.Random.Next(padding, (int)(Globals.GameSize.X - padding)),
                                Globals.Random.Next(padding, (int)(Globals.GameSize.Y - padding)));
        }
        private float Range(float min, float max)
        {
            return (float)Globals.Random.NextDouble() * (max - min) + min;
        }

        internal static Boss6 GetBoss6()
        {
            return new Boss6();
        }
    }
}
