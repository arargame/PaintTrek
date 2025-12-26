using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class StarFire:EnemyBullet
    {
        public StarFire(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
            SetVelocity();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Star Fire", 10, 10, 10);
        }

        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("Guns/starFireSpriteSheet"));
            SetTexture(GlobalTexture.starFireTexture, 4, 2, 16, true);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

        public override void Reset(Sprite owner)
        {
            base.Reset(owner);
            SetCharacterInfo("Star Fire", 10, 10, 10); // Reset Health
            this.visible = true;
            SetVelocity();
        }

        public override void SetVelocity()
        {
             // Target player logic similar to BasicEnemyBullet
             Vector2 target = Vector2.Zero;
             if (cachedPlayer != null && cachedPlayer.alive) target = cachedPlayer.position;
             else 
             {
                 // Find player
                 for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
                 {
                     Player player = SpriteSystem.spriteList[i] as Player;
                     if (player != null && player.alive)
                     {
                         target = player.position;
                         cachedPlayer = player;
                         break;
                     }
                 }
             }

             if (target != Vector2.Zero)
             {
                 Vector2 direction = Vector2.Normalize(this.position - target); // Towards target? Wait.
                 // Normalize(Pos - Target) points AWAY from target.
                 // Normalize(Target - Pos) points TO target.
                 // Usually velocity = Direction * Speed.
                 // Old code usually used: (-1) * Normalize(...) to flip?
                 // BasicEnemyBullet used: (-1) * Normalize(owner.pos - target.pos).
                 // That means (-1) * (Owner - Target) = (Target - Owner). Correct.
                 
                 Vector2 dir = Vector2.Normalize(this.position - target);
                 velocity = (-1) * dir * Globals.Random.Next(4, 7);
             }
             else
             {
                 velocity = new Vector2(-5, 0);
             }
        }
        
        private static Player cachedPlayer;
    }
}
