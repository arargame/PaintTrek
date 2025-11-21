using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    abstract class CollectableObject : Sprite
    {
        Texture2D damageTexture; 

        public override void Initialize()
        {
            base.Initialize();
            CollectableObjectSystem.Add(this);
            SetName("Collectable Object");
            visible = false;
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
                CollectableObjectSystem.Remove(this);
            }
        }

        public override void Draw()
        {
            base.Draw();
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
            position.X = (float)Globals.Random.Next((int)Globals.GameSize.X, (int)(Globals.GameSize.X + Globals.GameSize.X / 2));
            position.Y = (float)Globals.Random.Next(0, (int)(Globals.GameSize.Y - size.Y));
        }

        public override void SetVelocity()
        {
            base.SetVelocity();
        }

        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
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
            if (this.destinationRectangle.Intersects(GraphicSettings.leftAreaRectofScreen))
            {
                alive = false;
            }
        }

    }
}
