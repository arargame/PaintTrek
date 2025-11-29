using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

                            if (GetHealth() <= 0)
                                alive = false;

                            if (this.owner != null && (this.owner as Player).ability.GetSkill() == Skills.PoisonAttack)
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
            }
        }
    }
}
