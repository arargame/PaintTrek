using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class Bone : Enemy
    {
        enum BoneStyle
        {
            headBone1,
            headBone2,
            longBone,
        }

        BoneStyle style;
        float r;

        public Bone()
        {
            Initialize();

        }

        public override void Initialize()
        {
            base.Initialize();
            SetVelocity();
            SetCharacterInfo("Bone", 10, 20, 10);
            r = (float)Globals.Random.NextDouble();
        }

        public override void Load()
        {
            RandomizeStyle();
            switch (style)
            {
                case BoneStyle.headBone1:
                   // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/headBone1"));
                    SetTextures(GlobalTexture.headBone1Texture);
                    break;
                case BoneStyle.headBone2:
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/headBone2"));
                    SetTextures(GlobalTexture.headBone2Texture);
                    break;
                case BoneStyle.longBone:
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/longBone"));
                    SetTextures(GlobalTexture.longBoneTexture);
                    break;
                default:
                    //SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/longBone"));
                    SetTextures(GlobalTexture.longBoneTexture);
                    break;
            }
            animation = new Animation(texture, 1, 1, 1, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);

            Rotate(r);
        }

        public override void Draw()
        {
            base.Draw();
        }

        private void RandomizeStyle()
        {
            int r = Globals.Random.Next(-1, 2);

            if (r == 0) style = BoneStyle.headBone1;
            else if (r == 1) style = BoneStyle.headBone2;
            else if (r == 2) style = BoneStyle.longBone;
            else style = BoneStyle.longBone;
        }

        public override void SetStartingPosition()
        {

            for (int i = 0; i < EnemySystem.enemyList.Count; i++)
            {
                int counter = 0;
                do
                {
                    float width = Globals.GameSize.X;
                    float height = Globals.GameSize.Y - texture.Height;
                    position = new Vector2(Globals.Random.Next((int)width / 2, (int)(width + width / 3)), Globals.Random.Next((int)-height, (int)-texture.Height));
                    position.X = MathHelper.Clamp(position.X, 0, width + width / 3);
                    position.Y = MathHelper.Clamp(position.Y, 0, height);

                    counter++;
                    if (counter == 5)
                        break;

                } while (Sprite.SimpleCollisionDetection(this, EnemySystem.enemyList[i]));
            }
        }
        public override void SetVelocity()
        {
            this.velocity = new Vector2(Globals.Random.Next(-3, -1), Globals.Random.Next(1, 2));
        }

        public override void SimpleMovement(Vector2 amount)
        {
            position.X += amount.X;
            position.Y += amount.Y;


            if (position.Y > Globals.GameSize.Y)
                position = new Vector2(Globals.Random.Next(texture.Width, (int)Globals.GameSize.X), -texture.Height);
        }

        internal static Bone GetBone()
        {
            return new Bone();
        }
    }
}
