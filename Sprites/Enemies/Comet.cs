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
        float speed;

        public Comet()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Comet", 20, 20, 10);
            
            // Random speed once
            speed = Globals.Random.Next(4, 7);
            
            // Initial target search
            FindTarget();
            
            if (targetPosition != Vector2.Zero)
            {
                CalculateVelocity();
            }
            else
            {
                velocity = new Vector2(-speed, 0); // Default left
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

            FindTarget();
            
            if (targetPosition != Vector2.Zero)
            {
                CalculateVelocity();
                
                double angle = Math.Atan2(-velocity.Y, -velocity.X); // Angle follows velocity
                rotation = (float)angle;
            }
        }
        
        private void FindTarget()
        {
            targetPosition = Vector2.Zero;
            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;
                if (player != null && player.alive)
                {
                    targetPosition = player.position;
                    break;
                }
            }
        }
        
        private void CalculateVelocity()
        {
             Vector2 direction = Vector2.Normalize(targetPosition - this.position);
             float currentSpeed = speed;
             if (isPoisoned) currentSpeed *= 0.5f;
             velocity = direction * currentSpeed;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
             // Handled in Initialize/Update
        }

        internal static Comet GetComet()
        {
            return new Comet();
        }
    }
}
