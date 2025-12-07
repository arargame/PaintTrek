using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Statistics;

namespace PaintTrek
{
    abstract class Enemy : Sprite
    {
        Texture2D damageTexture; 

        public override void Initialize()
        {
            base.Initialize();
            EnemySystem.Add(this);
            SetName("Enemy");
            visible = true;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();
            MakeVisible();
            Kill();

            if (!alive)
            {
                EnemySystem.Remove(this);
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public virtual void MakeVisible()
        {
            if (this.destinationRectangle.Intersects(Globals.GameRect))
            {
                visible = true;
            }
        }


        public virtual void Kill()
        {
            if (this.destinationRectangle.Intersects(new Rectangle(GraphicSettings.leftAreaRectofScreen.Left, GraphicSettings.leftAreaRectofScreen.Top, GraphicSettings.leftAreaRectofScreen.Width - 50, GraphicSettings.leftAreaRectofScreen.Height)))
            {
                // Enemy escaped off screen - no kill credit
                alive = false;
            }
        }

        public override void SetTexture(Texture2D texture, int tilesX, int tilesY, int frameCount, bool looping)
        {
            base.SetTexture(texture, tilesX, tilesY, frameCount, looping);
            damageTexture = MakeDamageTexture(normalTexture);
        }

        public override void TakeDamage(Sprite another)
        {
            texture = damageTexture;
            
            // Store health before damage
            double healthBefore = GetHealth();
            
            base.TakeDamage(another);
            
            // Check if this damage killed the enemy
            if (healthBefore > 0 && GetHealth() <= 0)
            {
                // Record kill statistics with weapon info
                string weaponUsed = "Unknown";
                if (another is PlayerBullet)
                {
                    weaponUsed = another.GetType().Name; // "Laser", "Rocket", etc.
                }
                else if (another is Player)
                {
                    weaponUsed = "PlayerCollision";
                }
                
                StatisticsManager.Instance.RecordEnemyKill(
                    enemyType: this.GetType().Name,
                    weaponUsed: weaponUsed
                );
            }
        }

        public override void SetStartingPosition()
        {
            if (EnemySystem.enemyList.Count == 0)
            {
                float width = Globals.GameSize.X;
                float height = Globals.GameSize.Y - animation.Height;
                position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), Globals.Random.Next(0, (int)height));
                position.X = MathHelper.Clamp(position.X, width, width + width / 3);
                position.Y = MathHelper.Clamp(position.Y, 0, height);
            }
            else
            {

                for (int i = 0; i < EnemySystem.enemyList.Count; i++)
                {
                    int counter = 0;
                    do
                    {
                        float width = Globals.GameSize.X;
                        float height = Globals.GameSize.Y - size.Y;
                        position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), Globals.Random.Next(0, (int)height));
                        position.X = MathHelper.Clamp(position.X, width, width + width / 3);
                        position.Y = MathHelper.Clamp(position.Y, 0, height);

                        counter++;
                        if (counter == 5)
                            break;

                    } while (Sprite.SimpleCollisionDetection(this, EnemySystem.enemyList[i]));
                }
            }
        }
    }
}
