using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class TextureData
    {
        public Texture2D texture;
        public Color[,] textureData2D;
        public Color[] textureData;
        public List<Color[]> specificTextureDataList;

        private int tilesX;
        private int tilesY;
        private int tileWidth;
        private int tileHeight;

        private int frame;
        private int frameCount;


        public TextureData(Texture2D texture, int tilesX, int tilesY)
        {
            this.texture = texture;
            this.tilesX = tilesX;
            this.tilesY = tilesY;
            if (tilesX == 0) tilesX = 1;
            this.tileWidth = texture.Width / tilesX;
            if (tilesY == 0) tilesY = 1;
            this.tileHeight = texture.Height / tilesY;
            this.frameCount = tilesX * tilesY;

            Initialize();
        }

        public void Initialize()
        {
            specificTextureDataList = new List<Color[]>();
            SetTextureData();
            SetSpecificAreaColorArray();
        }

        public void Draw()
        {
            Globals.SpriteBatch.Begin();
            for (int i = 0; i < specificTextureDataList.Count; i++)
            {
                Texture2D texture = new Texture2D(Globals.Graphics.GraphicsDevice, tileWidth, tileHeight, false, SurfaceFormat.Color);
                texture.SetData<Color>(specificTextureDataList[i]);
                Globals.SpriteBatch.Draw(texture, new Rectangle(0 + tileWidth * i, 100, tileWidth, tileHeight), Color.White);
            }

            Globals.SpriteBatch.End();
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
                textureData2D = TextureTo2DArray(texture);
            }

        }

        private void SetSpecificAreaColorArray()
        {
            int width = tileWidth;
            int height = tileHeight;
            int row = 0;
            int column = 0;

            for (int i = 0; i < this.frameCount; i++)
            {
                int finishX = 0;
                int finishY = 0;
                int startX = 0;
                int startY = 0;


                if (row == 0)
                {
                    startY = 0;
                    finishY = height;
                }
                else
                {
                    startY = row * height;
                    finishY = (row * height) + height;
                }


                if (column == 0)
                {
                    startX = 0;
                    finishX = width;
                }
                else
                {
                    startX = column * width;
                    finishX = (column * width) + width;
                }

                int counter = 0;

                Color[] specificAreaColorArray = new Color[width * height];

                for (int k = startY; k < finishY; k++)
                {
                    for (int j = startX; j < finishX; j++)
                    {
                        specificAreaColorArray[counter] = textureData2D[j, k];
                        counter++;
                    }
                }

                this.frame++;

                if (column < tilesX - 1)
                {
                    column++;
                }
                else
                {
                    row++;
                    column = 0;
                }

                //this.frame %= this.frameCount;
                specificTextureDataList.Add(specificAreaColorArray);

            }
        }

        public static Color[,] TextureTo2DArray(Texture2D Texture)
        {
            Color[] TextureColor1D = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureColor1D);
            Color[,] TextureColor2D = new Color[Texture.Width, Texture.Height];
            for (int x = 0; x < Texture.Width; x++)
                for (int y = 0; y < Texture.Height; y++)
                    TextureColor2D[x, y] = TextureColor1D[x + y * Texture.Width];
            return TextureColor2D;
        }

        public Color[] getSpecificAreaColorArray(int frame)
        {
            return specificTextureDataList[frame];
        }

    }
}
