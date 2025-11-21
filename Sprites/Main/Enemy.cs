using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
                alive = false;
            }
        }

        public override void SetTextures(Texture2D newTexture)
        {
            base.SetTextures(newTexture);
            damageTexture = MakeDamageTexture(normalTexture);
        }

        public override void TakeDamage(Sprite another)
        {
            texture = damageTexture;
            base.TakeDamage(another);
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
