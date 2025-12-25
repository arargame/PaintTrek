using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class MRBrain : Enemy
    {
        HeartBreaker hearthBreaker;
        float fireTime;
        TimeSpan timeUntilFire;
        bool canFire = false;

        enum MovementStyle
        {
            starting,
            jumping
        }
        MovementStyle movementStyle;

        public MRBrain()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Mr Brain", 50, 30, 50);
            SetVelocity();
            movementStyle = MovementStyle.starting;
            hearthBreaker = null;
            timeUntilFire = TimeSpan.FromSeconds(Globals.Random.NextDouble());
        }

        public override void Load()
        {
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/mrBrainSpriteSheet"));
            SetTexture(GlobalTexture.mrBrainTexture, 3, 1, 3, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            if (movementStyle == MovementStyle.starting && position.X <= Globals.GameSize.X - size.X)
            {
                this.velocity = new Vector2(-2, -2);
                movementStyle = MovementStyle.jumping;
            }

            if (movementStyle == MovementStyle.jumping)
            {
                if (position.X <= Globals.GameSize.X / 2 + size.X)
                {
                    this.velocity.X *= -1;
                    this.velocity.Y *= 1;
                }
                else if (position.X + size.X / 2 >= Globals.GameSize.X)
                {
                    this.velocity.X *= -1;
                    this.velocity.Y *= 1;
                }

                if (position.Y <= 0)
                {
                    this.velocity.X *= 1;
                    this.velocity.Y *= -1;
                }
                else if (position.Y + size.Y / 2 >= Globals.GameSize.Y)
                {
                    this.velocity.X *= 1;
                    this.velocity.Y *= -1;
                }
            }

            FireSettings();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void FireSettings()
        {

            fireTime += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds;

            float fireThreshold = Globals.Random.Next(2, 5);
            if (isPoisoned) fireThreshold *= 3.0f;

            if (fireTime > fireThreshold)
            {
                fireTime = 0;
                if (canFire)
                {
                    // PERFORMANCE: Reuse existing HeartBreaker if dead, otherwise create new (or do nothing if active)
                    if (hearthBreaker == null)
                    {
                        hearthBreaker = new HeartBreaker(this);
                    }
                    else if (!hearthBreaker.alive)
                    {
                         hearthBreaker.Initialize();
                         hearthBreaker.alive = true;
                         // Add back to systems if removed? usually handled by Initialize or specific method
                         // For now assuming Initialize resets state and position relative to owner
                         hearthBreaker.SetStartingPosition();
                    }
                    
                    canFire = false;
                }
            }

            timeUntilFire = TimeSpan.FromSeconds(timeUntilFire.TotalSeconds - Globals.GameTime.ElapsedGameTime.TotalSeconds);
            if (timeUntilFire.TotalSeconds <= 0)
                canFire = true;

        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-2, 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static MRBrain GetMRBrain()
        {
            return new MRBrain();
        }
    }
}
