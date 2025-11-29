using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Android.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaintTrek
{
    public class TextureData
    {
        private Texture2D texture;
        private int tilesX;
        private int tilesY;
        private int tileWidth;
        private int tileHeight;

        private Color[] textureData; // Full texture pixel array
        private LRUCache<int, Color[]> frameCache;

        public int FrameWidth => tileWidth;
        public int FrameHeight => tileHeight;
        public int FrameCount => tilesX * tilesY;

        public TextureData(Texture2D texture, int tilesX, int tilesY)
        {
            this.texture = texture;
            this.tilesX = Math.Max(tilesX, 1);
            this.tilesY = Math.Max(tilesY, 1);
            this.tileWidth = texture.Width / this.tilesX;
            this.tileHeight = texture.Height / this.tilesY;
        }

        /// <summary>
        /// Initialize lazy frame extraction with LRU cache
        /// </summary>
        public void InitializeLazy(int cacheSize = 8)
        {
            frameCache = new LRUCache<int, Color[]>(cacheSize);

            int pixelCount = texture.Width * texture.Height;
            textureData = new Color[pixelCount];
            texture.GetData(textureData);
        }

        /// <summary>
        /// Get a specific frame's pixels (cached)
        /// </summary>
        public Color[] GetSpecificAreaColorArray(int frame)
        {
            frame = Math.Clamp(frame, 0, FrameCount - 1);

            if (frameCache.TryGet(frame, out var cachedPixels))
                return cachedPixels;

            var pixels = ExtractFramePixels(frame);
            frameCache.Add(frame, pixels);
            return pixels;
        }

        /// <summary>
        /// Extract pixels for a single frame
        /// </summary>
        private Color[] ExtractFramePixels(int frame)
        {
            int x = (frame % tilesX) * tileWidth;
            int y = (frame / tilesX) * tileHeight;

            Color[] pixels = new Color[tileWidth * tileHeight];
            int index = 0;

            for (int row = 0; row < tileHeight; row++)
            {
                int srcIndex = (y + row) * texture.Width + x;
                for (int col = 0; col < tileWidth; col++)
                {
                    pixels[index++] = textureData[srcIndex + col];
                }
            }

            return pixels;
        }
    }
}
