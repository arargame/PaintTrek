using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;
using PaintTrek.Shared.Statistics;

namespace PaintTrek
{
    abstract class Enemy : Sprite
    {
        public bool isPoisoned = false;
        private Texture2D poisonTexture; // Just for visual reference if needed, or use GlobalTexture
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
             if (isPoisoned && alive && visible)
             {
                 // Draw generic green glow behind
                 // Reusing GlobalTexture.diamondGreenTexture if it exists, or just a tinted sprite
                 // Let's assume GlobalTexture has it, or just draw the enemy itself with Green color and offsets
                 // Ideally use a particle texture. Wrench uses GlobalTexture.wrenchTexture.
                 // Let's use `GlobalTexture.diamondGreenTexture` assuming it exists (common for "green diamond").
                 // If not, I'll fallback to a simple circle or just tinting.
                 // PROPOSED: Pulsating "Frog Swimming" Effect
                 // Use Sine wave for smooth breathing/pulsing
                 float time = (float)Globals.GameTime.TotalGameTime.TotalSeconds;
                 float pulseSpeed = 10f; // How fast it pulsates
                 float pulseAmount = 0.1f; // How much it expands
                 float baseScale = 1.2f;
                 
                 float currentPulse = (float)Math.Sin(time * pulseSpeed); // -1 to 1
                 float finalScaleMultiplier = baseScale + (currentPulse * pulseAmount); 
                 
                 // Also oscillate Alpha for extra "shine"
                 float alpha = 0.7f + (currentPulse * 0.2f); // 0.5 to 0.9

                 Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                 Globals.SpriteBatch.Draw(texture, position, sourceRectangle, Color.Lime * alpha, rotation, origin, scale * finalScaleMultiplier, spriteEffect, 1f);
                 Globals.SpriteBatch.End();
             }
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
                if (Globals.IsWaveMode && !(this is Boss))
                    EndlessManager.Instance.NotifyEnemyKilled();

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

                PlayerBullet playerBullet = another as PlayerBullet;
                Player playerOwner = playerBullet?.owner as Player;
                if (playerOwner != null)
                    playerOwner.RewardEnemyDefeat(position);
            }
        }

        public override void SetCharacterInfo(string name, double health, double damage, int point)
        {
            base.SetCharacterInfo(name, health, damage, point);
            if (Globals.CurrentMode == GameMode.AgainstAllBosses && this is Boss)
                SetHealth(GetHealth() * EndlessManager.Instance.BossRushHealthMultiplier - GetHealth());
            else if (Globals.CurrentMode == GameMode.Endless || Globals.CurrentMode == GameMode.UfoInvasion)
            {
                float scale = EndlessManager.Instance.DifficultyScaling;
                SetHealth(GetHealth() * scale - GetHealth());
                SetDamage(damage * scale);
            }
        }

        public override void SetStartingPosition()
        {
            if (EnemySystem.enemyList.Count == 0)
            {
                float width = Globals.GameSize.X;
                float height = Globals.GameSize.Y - animation.Height;
                int maxY = (int)height;
                if (maxY <= 0) maxY = 1;
                position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), Globals.Random.Next(0, maxY));
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
                        int maxY = (int)height;
                        if (maxY <= 0) maxY = 1;
                        
                        position = new Vector2(Globals.Random.Next((int)width, (int)(width + width / 3)), Globals.Random.Next(0, maxY));
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
