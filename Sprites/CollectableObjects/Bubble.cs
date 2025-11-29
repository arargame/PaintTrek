using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Bubble:CollectableObject
    {
        Player owner;

        public Bubble() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Bubble", 50, 0, 10);
            SetVelocity();
        }
        public override void Load()
        {
            //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/bubble"));
            SetTexture(GlobalTexture.bubbleTexture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);
            CollisionDetectionWithEnemies();

            if (owner != null) 
            {
                position = new Vector2(owner.position.X,owner.position.Y);
            }

            if (GetHealth() < 40)
            {
                color.A = 192;
            }
            else if (GetHealth() < 30)
            {
                color.A = 128;
            }
            else if (GetHealth() < 20)
            {
                color.A = 64;
            }
        }
        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-4,0);
            if (Globals.Graphics.IsFullScreen)
            {
                velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
            }
        }

        public void AssaignOwner(Player owner) 
        {
            this.owner = owner;
        }
        public Player GetOwner() 
        {
            return owner;
        }

        internal static Bubble GetBubble()
        {
            return new Bubble();
        }

        public void CollisionDetectionWithEnemies() 
        {
            Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);

            //With Enemies
            #region collisionWithEnemies
            for (int i = EnemySystem.enemyList.Count - 1; i >= 0; i--)
            {
                if (owner == null) continue;

                Enemy enemy = EnemySystem.enemyList[i];

                if (enemy != null)
                {
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)enemy.size.X, (int)enemy.size.Y), enemy.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (this.specificTextureData == null || enemy.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, this.specificTextureData, enemy.transformMatrix, (int)enemy.size.X, (int)enemy.size.Y, enemy.specificTextureData))
                        {
                            enemy.TakeDamage(this);
                            TakeDamage(enemy);
                            SetHealth(enemy.GetDamage());
                            Level.AddScore(this.GetPoint());

                            if (enemy is Boss)
                            {
                                enemy.SetHealth(this.GetDamage());
                                Level.AddScore(enemy.GetPoint() * Level.LevelCounter);
                            }
                            else
                            {
                                enemy.SetHealth(-1000);
                                Level.AddScore(enemy.GetPoint());
                            }
                        }
                    }
                }
            }
            #endregion

        }
    }
}
