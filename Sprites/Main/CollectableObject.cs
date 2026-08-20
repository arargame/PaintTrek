using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Statistics;

namespace PaintTrek
{
    public abstract class CollectableObject : Sprite
    {
        Texture2D damageTexture; 

        // Android moves collectables from its base class while desktop leaves movement to the
        // concrete object. Magnet-controlled objects opt out so both platforms move once only.
        protected virtual bool UsesAutomaticMovement => true;

        public override void Initialize()
        {
            base.Initialize();
            CollectableObjectSystem.Add(this);
            Reset();
        }

        public virtual void Reset()
        {
            try
            {
                // CRITICAL FIX: Re-add to systems to prevent freezing if pooled
                if (!CollectableObjectSystem.collactableObjectList.Contains(this)) CollectableObjectSystem.Add(this);
                if (!SpriteSystem.spriteList.Contains(this)) SpriteSystem.Add(this);

                SetName("Collectable Object");
                visible = false;
                alive = true;
                SetStartingPosition();
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CollectableObject] CRASH in Reset! Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                throw; // Re-throw to pause debugger if caught
            }
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

        public override void TakeDamage(Sprite another)
        {
            texture = damageTexture;
            base.TakeDamage(another);
        }

        public override void SetStartingPosition()
        {
            position.X = (float)Globals.Random.Next((int)Globals.GameSize.X, (int)(Globals.GameSize.X + Globals.GameSize.X / 2));
            
            // Fix ArgumentOutOfRangeException if GameSize.Y < size.Y
            int maxY = (int)(Globals.GameSize.Y - size.Y);
            if (maxY <= 0) maxY = 1;
            position.Y = (float)Globals.Random.Next(0, maxY);
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
            if (!alive) return; // FIX: Do not make visible if dead

            if (this.destinationRectangle.Intersects(Globals.GameRect))
            {
                visible = true;
            }
        }
        public virtual void Kill()
        {
            if (this.destinationRectangle.Intersects(GraphicSettings.leftAreaRectofScreen) || this.position.X < -200)
            {
                alive = false;
            }
        }

        protected override void RemoveFromAllSystems()
        {
            CollectableObjectSystem.Remove(this);
            base.RemoveFromAllSystems();
            alive = false;
            visible = false;
        }

    }
}
