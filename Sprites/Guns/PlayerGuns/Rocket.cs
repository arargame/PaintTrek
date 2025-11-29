using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Rocket:PlayerBullet
    {
        Enemy target;
        bool isFocusedOnTarget;

        public Rocket(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Rocket", 10, 20, 10);
            SetVelocity();
            isFocusedOnTarget = false;
            target = null;
        }

        public override void Load()
        {
            base.Load();
            SetTexture(GlobalTexture.rocketTexture, 1, 5, 5, true);
        }

        public override void Update()
        {
            base.Update();

            if (isFocusedOnTarget)
            {
                if (visible && target != null)
                {
                    if (target.alive != false)
                    {
                        //float distance = Vector2.Distance(this.position, targetPosition);
                        float distance = Math.Abs(this.position.X - target.position.X);

                        Vector2 direction = Vector2.Normalize(this.position - target.position);
                        velocity = (-1) * direction * Globals.Random.Next(10, 20);
                        double angle = Math.Atan2(velocity.Y, velocity.X);
                        rotation = (float)angle;


                        /*if (distance < size.X)
                        {



                            Vector2 direction = Vector2.Normalize(this.position - target.position);
                            velocity = (-1) * direction * Globals.Random.Next(15, 25);
                            double angle = Math.Atan2(velocity.Y, velocity.X);
                            rotation = (float)angle;
                        }
                        else
                        {
                            if (this.position.X < target.position.X)
                            {
                                velocity.X = 15;

                                if (this.position.Y < target.position.Y)
                                    velocity.Y += 0.1f;
                                else velocity.Y -= 0.1f;

                            }
                            else
                            {
                                velocity.X = -15;

                                if (this.position.Y < target.position.Y)
                                    velocity.Y += 0.1f;
                                else velocity.Y -= 0.1f;
                            }

                            double angle = Math.Atan2(velocity.Y, velocity.X);
                            rotation = (float)angle;
                        }*/

                    }
                    else
                    {
                        velocity.X = 15;
                        velocity.Y = 0;
                        double angle = Math.Atan2(velocity.Y, velocity.X);
                        rotation = (float)angle;
                    }

                }
                // Vector2 direction = Vector2.Normalize(owner.position - targetPosition);
                // velocity = (-1) * direction * Globals.Random.Next(10, 20);
            }
            else CheckEnemy();

        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Fire()
        {
            base.Fire();
        }
        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
        }

        public override void SetVelocity()
        {
            this.velocity = new Vector2(1, 1);
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

        private void CheckEnemy() 
        {

            for (int i = 0; i < EnemySystem.enemyList.Count; i++)
            {
                Enemy enemy = EnemySystem.enemyList[i];
                if (enemy == null) continue;

                if (Globals.GameRect.Contains((int)enemy.position.X, (int)enemy.position.Y))
                    target = enemy;
                else continue;

                isFocusedOnTarget = true;

                break;
            }
        }
    }
}
