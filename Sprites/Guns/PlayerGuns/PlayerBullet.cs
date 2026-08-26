using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    abstract class PlayerBullet : Bullet
    {
        public override void Initialize()
        {
            base.Initialize();
            
            // Pre-load sounds
            SoundManager.Load("laserFire", "Sounds/SoundEffects/laserFire");
            SoundManager.Load("hit", "Sounds/SoundEffects/hit");
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            CollisionDetectionWithEnemies();
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        /// <summary>
        /// Diamond glow belongs only to the player's basic Laser projectile.
        /// Special projectiles deliberately do not call this method.
        /// </summary>
        protected void DrawAbilityGlow()
        {
            Player playerOwner = owner as Player;
            if (playerOwner == null || !alive || !visible || texture == null || texture.IsDisposed)
                return;

            Color glowColor;
            switch (playerOwner.ability.GetSkill())
            {
                case Skills.PowerAttack:
                    glowColor = new Color(255, 45, 30, 92);
                    break;
                case Skills.SpeedyAttack:
                    glowColor = new Color(35, 145, 255, 82);
                    break;
                case Skills.PoisonAttack:
                    glowColor = new Color(35, 255, 85, 86);
                    break;
                case Skills.CriticalAttack:
                    glowColor = new Color(125, 80, 190, 94);
                    break;
                default:
                    return;
            }

            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Globals.SpriteBatch.Draw(texture, position, sourceRectangle, glowColor, rotation, origin,
                scale * 1.65f, spriteEffect, layerDepth);
            Globals.SpriteBatch.End();
        }

        private void CollisionDetectionWithEnemies()
        {

            for (int i = EnemySystem.enemyList.Count - 1; i >= 0; i--)
            {
                if (!visible) continue;

                Enemy enemy = EnemySystem.enemyList[i];

                if (enemy != null)
                {
                    Rectangle rect1 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)size.X, (int)size.Y), transformMatrix);
                    Rectangle rect2 = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, (int)enemy.size.X, (int)enemy.size.Y), enemy.transformMatrix);

                    if (rect1.Intersects(rect2))
                    {
                        if (specificTextureData == null || enemy.specificTextureData == null)
                            continue;

                        if (Sprite.CollisionDetection(transformMatrix, (int)size.X, (int)size.Y, specificTextureData, enemy.transformMatrix, (int)enemy.size.X, (int)enemy.size.Y, enemy.specificTextureData))
                        {
                            TakeDamage(enemy);

                            enemy.TakeDamage(this);

            SoundManager.Play("hit");

            Player playerOwner = owner as Player;
            if (playerOwner != null && playerOwner.ability.GetSkill() == Skills.CriticalAttack)
            {
                playerOwner.ShowFloatingMessage(Loc.T(LocKeys.Gameplay.CriticalHit), enemy.position, Color.Red);
                ScreenShake.Trigger();
            }

            if (GetHealth() <= 0)
                alive = false;

            if (playerOwner != null && playerOwner.ability.GetSkill() == Skills.PoisonAttack)
                            {
                                SlowMovement(enemy);
                            }
                        }
                    }

                }

            }

        }
        public override void Fire()
        {
            base.Fire();
            SoundManager.Play("laserFire");
        }

        public void SlowMovement(Enemy enemy)
        {
            if (!(enemy is Boss))
            {
                enemy.velocity = new Vector2(enemy.velocity.X * 0.5f, enemy.velocity.Y);
                enemy.color = Color.Green;
                enemy.isPoisoned = true;
            }
        }
    }
}
