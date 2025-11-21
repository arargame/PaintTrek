using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class OrbitalFire:PlayerBullet
    {

        public OrbitalFire(Sprite owner) 
        {
            Initialize();
            AssignOwner(owner);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Guided Fire", 10, 20, 10);
            SetVelocity();
        }

        public override void Load()
        {
            base.Load();
           //SetTextures(Globals.Content.Load<Texture2D>("Guns/orbitalFireSpriteSheet"));
            SetTextures(GlobalTexture.orbitalFireTexture);
            animation = new Animation(texture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();

            this.position = new Vector2(owner.destinationRectangle.Center.X, owner.destinationRectangle.Center.Y);
            this.origin = new Vector2(owner.destinationRectangle.Width,owner.destinationRectangle.Height);
            Rotate(4f);
            originChanged = true;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SimpleMovement(Vector2 amount)
        {
            base.SimpleMovement(amount);
        }

        public override void SetStartingPosition()
        {
            base.SetStartingPosition();
        }

        public override void SetVelocity()
        {
            this.velocity = new Vector2(0, 0);
        }

        public override void Fire()
        {
            base.Fire();
        }

        public override void TakeDamage(Sprite another)
        {
            base.TakeDamage(another);
            Level.AddScore(this.GetPoint());
        }

    }
}
