using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Light
    {
        public Texture2D texture;
        Vector2 position;
        Color color;
        Vector2 speed;

        public Light()
        {
            texture = Globals.Content.Load<Texture2D>("Textures/rainDrop");
            color = new Color(255, 255, 255, Globals.Random.Next(10, 255));
            SetStartingPosition();
            speed = new Vector2(Globals.Random.Next(-3, 3), Globals.Random.Next(1, 3));
        }
        ~Light() 
        {
        }

        void SetStartingPosition()
        {
            position = new Vector2(Globals.Random.Next(0, (int)Globals.GameSize.X), Globals.Random.Next(-10, (int)Globals.GameSize.Y));
        }

        public void Update()
        {
            position.X += speed.X;
            position.Y += speed.Y;


            if (position.Y > Globals.GameSize.Y)
                position = new Vector2(Globals.Random.Next(0, (int)Globals.GameSize.X), 0);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(texture, position, color);
            Globals.SpriteBatch.End();
        }
    }
}
