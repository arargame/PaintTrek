using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    abstract class Bullet : Sprite
    {
        bool isOut;
        public Sprite owner;

        public override void Initialize()
        {
            base.Initialize();
            GunSystem.Add(this);
            visible = false;
            isOut = false;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();

            if (!this.destinationRectangle.Intersects(Globals.GameRect))
            {
                alive = false;
            }

            if (visible)
            {
                SimpleMovement(velocity);

                Rectangle bulletRect = Sprite.CalculateBoundingRectangle(new Rectangle(0,0, (int)this.size.X, (int)this.size.Y), this.transformMatrix);
                Rectangle ownerRect=Sprite.CalculateBoundingRectangle(new Rectangle((int)owner.size.X/4,(int)owner.size.Y/4 , (int)owner.size.X/3, (int)owner.size.Y/3), owner.transformMatrix);

                if (bulletRect.Intersects(ownerRect) && !(this is BouncingFire) && !(this is OrbitalFire))
                {
                    isOut = false;
                }
                else isOut = true;
            }

            if (!alive)
            {
                GunSystem.Remove(this);
                SpriteSystem.Remove(this);
            }
        }

        public override void Draw()
        {
            if(isOut)
            base.Draw();
        }

        public void AssignOwner(Sprite owner)
        {
            this.owner = owner;
        }
        public override void SetStartingPosition()
        {
            if (owner == null)
                position = new Vector2(GraphicSettings.topAreaofScreen.X, GraphicSettings.topAreaofScreen.Y);
            else
            {
                position.X = this.owner.destinationRectangle.X + this.owner.animation.Width / 2;
                position.Y = this.owner.destinationRectangle.Y + this.owner.animation.Height / 2;
            }
        }

        public virtual void Fire()
        {
            if (!visible)
            {
                this.visible = true;
                position.X = this.owner.destinationRectangle.X+this.owner.animation.Width/2;
                position.Y = this.owner.destinationRectangle.Y+this.owner.animation.Height/2;
            }
        }

    }
}
