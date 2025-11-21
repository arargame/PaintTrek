using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class ExitDoor
    {
        Vector2 position;
        Texture2D texture;
        bool exitDoorIsOpen;
        Animation animation;
        float rotation;
        Vector2 size;
        Vector2 origin;
        Color[,] textureData2D;
        Color[] textureData;
        public Color[] specificTextureData;
        public Matrix transformMatrix;
        Rectangle sourceRectangle;

        public ExitDoor()
        {
            texture = Globals.Content.Load<Texture2D>("Textures/imposibleObjectSpriteSheet");
            animation = new Animation(texture, 5, 2, 5, true);
            position = new Vector2(Globals.GameSize.X - animation.tileWidth, Globals.GameSize.Y / 2 - animation.tileHeight / 2);
            exitDoorIsOpen = false;
            rotation = 0.0f;
            transformMatrix = Matrix.Identity;
            SetTextureData();
        }

        ~ExitDoor() 
        {
        }

        public void Update()
        {
            if (BossSystem.bossHasFallen)
            {
                exitDoorIsOpen = true;
            }
            else exitDoorIsOpen = false;

            if (exitDoorIsOpen)
            {
                animation.Update();
                rotation += 0.02f;
                size = new Vector2(animation.Width,animation.Height);
                origin = new Vector2(size.X / 2, size.Y / 2);
                SetSpecificTextureData();
                CalculateTransformMatrix();
                sourceRectangle = new Rectangle(animation.FrameBounds.X, animation.FrameBounds.Y, (int)size.X, (int)size.Y);

            }

        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();

            if (exitDoorIsOpen)
            {
                Globals.SpriteBatch.Draw(texture, position, sourceRectangle, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
            }

            Globals.SpriteBatch.End();
        }
        public bool IsOpen()
        {
            return exitDoorIsOpen;
        }
        public Rectangle GetExitArea()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public Vector2 GetSize() 
        {
            return size;
        }

        private void CalculateTransformMatrix()
        {
            transformMatrix = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(1f) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
        }

        private void SetSpecificTextureData()
        {
            specificTextureData = GetSpecificAreaColorArray();
        }

        private Color[] GetSpecificAreaColorArray()
        {
            int width = (int)size.X;
            int height = (int)size.Y;

            Color[] specificAreaColorArray = new Color[width * height];

            int counter = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    specificAreaColorArray[counter] = textureData2D[j, i];
                    counter++;
                }
            }

            return specificAreaColorArray;
        }

        private void SetTextureData()
        {
            int pixelCount = texture.Width * texture.Height;

            if (pixelCount <= 0)
            {
                textureData = new Color[1];
                textureData2D = new Color[1, 1];
            }
            else
            {
                textureData = new Color[pixelCount];
                texture.GetData(textureData);
                textureData2D = Sprite.TextureTo2DArray(texture);
            }

        }

    }
}
