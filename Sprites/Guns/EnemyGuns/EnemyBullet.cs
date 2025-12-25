using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    abstract class EnemyBullet : Bullet
    {
        //SoundSystem hitSound;
        public Vector2 targetPosition;

        public override void Initialize()
        {
            base.Initialize();

          //  hitSound = new SoundSystem("Sounds/SoundEffects/hit", 1f, 0f, 0f, false, "", "");
            SoundManager.Load("hit", "Sounds/SoundEffects/hit");

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                Player player = SpriteSystem.spriteList[i] as Player;

                if (player == null) continue;

                targetPosition = player.position;
                break;
            }
           
        }

        public override void Update()
        {
            base.Update();
            CollisionDetectionWithPlayer();
            CollisionDetectionWithBubble();
            CollisionDetectionWithPlayerBullet();
        }

        public override void Draw()
        {
            base.Draw();
        }
        private void CollisionDetectionWithPlayer()
        {

            for (int i = 0; i < SpriteSystem.spriteList.Count; i++)
            {
                if (!visible) continue;

                Player player = SpriteSystem.spriteList[i] as Player;

                if (player != null && !player.isInvinsible)
                {
                    Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)player.size.X, (int)player.size.Y), player.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (specificTextureData == null || player.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, specificTextureData, player.transformMatrix, (int)player.size.X, (int)player.size.Y, player.specificTextureData))
                        {
                            TakeDamage(player);
                            player.TakeDamage(this);
                            alive = false;
                            player.Respawn(2f);
                        }
                    }
                }
            }
        }

        private void CollisionDetectionWithBubble()
        {
            // OPTIMIZATION: Use cached activeBubbles list instead of full object list
            // Reduces complexity from O(AllObjects) to O(ActiveBubbles)
            
            for (int i = 0; i < CollectableObjectSystem.activeBubbles.Count; i++)
            {
                if (!visible) continue;
                Bubble bubble = CollectableObjectSystem.activeBubbles[i];

                if (bubble != null && bubble.GetOwner() != null && bubble.IsActive) // Ensure active
                {
                    Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)bubble.size.X, (int)bubble.size.Y), bubble.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (specificTextureData == null || bubble.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, specificTextureData, bubble.transformMatrix, (int)bubble.size.X, (int)bubble.size.Y, bubble.specificTextureData))
                        {
                            TakeDamage(bubble);
                            bubble.TakeDamage(this);
                            alive = false;
                        }
                    }
                }
            }
        }

        private void CollisionDetectionWithPlayerBullet()
        {
            // OPTIMIZATION: Use SpatialGrid for spatial partitioning collision check
            // Reduces O(N*M) to O(N*k) where k is generally small
            
            var nearbyBullets = SpatialGrid.Instance.GetNearby<PlayerBullet>(this);
            
            foreach (var bullet in nearbyBullets)
            {
                if (!visible) continue;
                
                PlayerBullet playerBullet = bullet as PlayerBullet; // Already filtered by GetNearby generic, but cast confirms

                if (playerBullet != null && playerBullet.visible && playerBullet.alive) // Alive check important
                {
                    Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)playerBullet.size.X, (int)playerBullet.size.Y), playerBullet.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (specificTextureData == null || playerBullet.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, specificTextureData, playerBullet.transformMatrix, (int)playerBullet.size.X, (int)playerBullet.size.Y, playerBullet.specificTextureData))
                        {
                            TakeDamage(playerBullet);
                            playerBullet.TakeDamage(this);

                            SoundManager.Play("hit");

                            if (GetHealth() <= 0)
                                alive = false;

                            playerBullet.alive = false;
                        }
                    }
                }
            }
        }
    }
}
