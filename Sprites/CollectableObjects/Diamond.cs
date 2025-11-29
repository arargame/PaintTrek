using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    enum Style
    {
        Red,
        Blue,
        Green,
        Black
    }
    class Diamond:CollectableObject
    {
        Style diamondStyle;
        float angle;

        bool isIncreasing;
        double maxScale;

        public Diamond() 
        {
            RandomStyle();
            Initialize();
            isIncreasing = false;
            maxScale = 1.5f;
            scale = Globals.Random.Next(1,2);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Diamond", 10, 0, 10);
            SetVelocity();
            do
            {
                if (Globals.Random.Next(0,9) % 2 == 1)
                    angle = -1 * (float)Globals.Random.NextDouble();
                else
                    angle = (float)Globals.Random.NextDouble();
            } while (angle == 0);
        }

        public override void Load()
        {

            if (diamondStyle == Style.Black)
            {
                //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/blackDiamond")); 
                SetTexture(GlobalTexture.blackDiamondTexture, 1, 1, 1, true);
            }
            if (diamondStyle == Style.Blue)
            {
                //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/blueDiamond"));
                SetTexture(GlobalTexture.blueDiamondTexture, 1, 1, 1, true);
            }
            if (diamondStyle == Style.Green)
            {
                //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/greenDiamond"));
                SetTexture(GlobalTexture.greenDiamondTexture, 1, 1, 1, true);
            }
            if (diamondStyle == Style.Red)
            {
                //SetTextures(Globals.Content.Load<Texture2D>("CollectableObjects/redDiamond"));
                SetTexture(GlobalTexture.redDiamondTexture, 1, 1, 1, true);
            }
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);
            Rotate(angle);


            if (isIncreasing)
            {
                scale += 0.008f;
                if (scale > maxScale)
                    isIncreasing = false;
            }
            else 
            {
                scale -= 0.008f;
                if (scale <= 0.5f)
                    isIncreasing = true;
            }

        }

        public override void Draw()
        {
            if (alive && visible && !texture.IsDisposed)
            {
                // Glow effect calculation
                float time = (float)Globals.GameTime.TotalGameTime.TotalSeconds;
                float glowScale = scale * 1.4f; // %40 daha büyük
                float alpha = (float)(Math.Sin(time * 10) + 1) / 2.0f * 0.5f; // Hızlı yanıp sönme, max 0.5 alpha

                Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                // Draw glow (arkaya çizilir)
                Globals.SpriteBatch.Draw(texture,
                    position,
                    sourceRectangle,
                    color * alpha, // Transparent color
                    rotation,
                    origin,
                    glowScale,
                    spriteEffect,
                    layerDepth);

                Globals.SpriteBatch.End();
            }

            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-(Globals.Random.Next(1,4)),0);
            if (Globals.Graphics.IsFullScreen)
            {
                velocity = new Vector2((velocity.X * 1280) / 800, (velocity.Y * 800) / 600);
            }
        }

        private void RandomStyle()
        {
            int number = Globals.Random.Next(0, 100);


            if (number >= 0 && number <= 30)
            {
                diamondStyle = Style.Red;
            }
            else if (number > 30 && number <= 60)
            {
                diamondStyle = Style.Blue;
            }
            else if (number > 60 && number <= 90)
            {
                diamondStyle = Style.Green;
            }
            else if (number > 90 && number <= 100)
            {
                diamondStyle = Style.Black;
            }

        }
        public Style ShowStyle() 
        {
            return diamondStyle;
        }

        internal static Diamond GetDiamond()
        {
            return new Diamond();
        }

    }
}
