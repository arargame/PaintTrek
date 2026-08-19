using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class ParallaxingBackground
    {
        // A small overlap removes the transparent/bilinear edge seam between two scrolling tiles.
        private const int TileOverlapPixels = 3;

        // The image representing the parallaxing background
        Texture2D texture;


        // An array of positions of the parallaxing background
        Vector2[] positions;

        // The speed which the background is moving
        int speed;

        bool initialized = false;

        public bool Initialized
        {
            get { return initialized; }
        }

        public void Initialize(string texturePath, int screenWidth, int speed)
        {
            // Load the background texture we will be using
            texture = Globals.Content.Load<Texture2D>(texturePath);

            if (texture != null)
            {
                // Set the speed of the background
                this.speed = speed;


                // Every tile is scaled to the current virtual viewport. Two adjacent tiles cover
                // the screen continuously, independently of the PNG's native width.
                positions = new Vector2[2];
                positions[0] = Vector2.Zero;
                positions[1] = new Vector2(screenWidth, 0);

                initialized = true;
            }
            else initialized = false;
        }

        public void UnloadContent()
        {
        }

        public void Update()
        {
            int screenWidth = (int)Globals.GameSize.X;

            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update the position of the screen by adding the speed
                positions[i].X += speed;
                // If the speed has the background moving to the left
                if (speed <= 0)
                {
                    if (positions[i].X <= -screenWidth)
                        positions[i].X = positions[1 - i].X + screenWidth;
                }


                // If the speed has the background moving to the right
                else
                {
                    if (positions[i].X >= screenWidth)
                        positions[i].X = positions[1 - i].X - screenWidth;
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < positions.Length; i++)
            {
                // PointClamp keeps the edge texels from being blended with transparent pixels.
                // The 3px overlap makes the two destinations cover every virtual pixel.
                Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                Globals.SpriteBatch.Draw(texture, new Rectangle(
                    (int)positions[i].X,
                    (int)positions[i].Y,
                    (int)Globals.GameSize.X + TileOverlapPixels,
                    (int)Globals.GameSize.Y), Color.White);
                Globals.SpriteBatch.End();
            }
        }

        
    }
}
