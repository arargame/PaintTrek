using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class BasicEnemyBullet : EnemyBullet
    {
        public BasicEnemyBullet(Sprite owner)
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Basic Enemy Bullet", 10, 10, 10);
        }

        public override void Load()
        {
            SetTexture(GlobalTexture.basicEnemyBulletTexture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
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
                velocity = (-1) * direction * Globals.Random.Next(3, 5);
                if (Globals.Graphics.IsFullScreen)
                {
                    velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
                }
            }
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

        public override void Reset(Sprite owner)
        {
            base.Reset(owner); // Handles System Re-Add, Owner Assign, Alive=true, InvisibleTime=0
            SetCharacterInfo("Basic Enemy Bullet", 10, 10, 10); // Reset Health


            this.visible = true; // Override base visibility (false -> true)

            // Re-find target (Player)
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
            
            SetVelocity();
        }
    }
}
