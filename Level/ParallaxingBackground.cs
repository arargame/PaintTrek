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


                // If we divide the screen with the texture width then we can determine the number of tiles need.
                // We add 1 to it so that we won't have a gap in the tiling
                positions = new Vector2[screenWidth / texture.Width + 1];


                // Set the initial positions of the parallaxing background
                for (int i = 0; i < positions.Length; i++)
                {
                    // We need the tiles to be side by side to create a tiling effect
                    positions[i] = new Vector2(i * texture.Width, 0);
                }

                initialized = true;
            }
            else initialized = false;
        }

        public void UnloadContent()
        {
        }

        public void Update()
        {
            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update the position of the screen by adding the speed
                positions[i].X += speed;
                // If the speed has the background moving to the left
                if (speed <= 0)
                {
                    // Check the texture is out of view then put that texture at the end of the screen
                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);
                    }
                }


                // If the speed has the background moving to the right
                else
                {
                    // Check if the texture is out of view then position it to the start of the screen
                    if (positions[i].X >= texture.Width * (positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;
                    }
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Globals.SpriteBatch.Begin();
                Globals.SpriteBatch.Draw(texture, new Rectangle((int)positions[i].X, (int)positions[i].Y, (int)Globals.GameSize.X, (int)Globals.GameSize.Y), Color.White);
                Globals.SpriteBatch.End();
            }
        }

        
    }
}
