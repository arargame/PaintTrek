using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    public abstract class Bullet : Sprite
    {
        bool isOut;
        public Sprite owner;
        double invisibleTime;

        /// <summary>
        /// Resets the bullet state for reuse (Object Pooling)
        /// </summary>
        public virtual void Reset(Sprite owner)
        {
            this.owner = owner;
            // Initialize(); // REMOVED: Do NOT call Initialize as it adds to lists again
            
            // CRITICAL FIX: Re-add to systems because RemoveFromAllSystems removed them!
            // Without this, the bullet is "Alive" but receives no Update() calls, causing it to freeze.
            if (!GunSystem.bulletList.Contains(this)) GunSystem.Add(this);
            if (!SpriteSystem.spriteList.Contains(this)) SpriteSystem.Add(this);
            
            // Manually reset state
            visible = false;
            alive = true;
            isOut = false;
            invisibleTime = 0;
            
            SetStartingPosition();
        }

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
            
            // STRICT SAFETY CHECK: Force kill if way off screen
            // Reduced buffer to 10 to catch bullets stuck just outside (e.g. 1283)
            if (position.X < -50 || position.X > Globals.GameSize.X + 10 || 
                position.Y < -50 || position.Y > Globals.GameSize.Y + 10)
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
            else
            {
                // ZOMBIE BULLET CHECK:
                // If alive but not visible for too long, kill it.
                // Increased to 5.0s to allow Cacao aiming time
                invisibleTime += Globals.GameTime.ElapsedGameTime.TotalSeconds;
                if (invisibleTime > 5.0)
                {
                   alive = false;
                   GunSystem.Remove(this); // Ensure removal
                   return;
                }
            }

            if (!alive)
            {
                // RemoveFromAllSystems(); // Already called by base or logic? 
                // Wait, logic says 'if (!alive)'. 
                // Desktop code usually removes here.
                
                RemoveFromAllSystems();
                visible = false;
            }
        }
        
        protected override void RemoveFromAllSystems()
        {
             GunSystem.Remove(this);
             base.RemoveFromAllSystems(); // Calls SpriteSystem.Remove(this)
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
                
                // IMPORTANT: Ensure bullet is in GunSystem (for pooled bullets)
                if (!GunSystem.bulletList.Contains(this))
                {
                    GunSystem.Add(this);
                }
            }
        }

    }
}
