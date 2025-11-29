using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    public class Animation
    {
        private PaintTrek.TextureData textureDataObj;
        private Texture2D texture;
        public int tilesX;
        public int tilesY;
        public int tileWidth;
        public int tileHeight;

        private int frame;
        public int frameCount;

        private double frameInterval;
        private double frameTimeRemaining;


        private bool active;
        public bool looping;

        public int Width
        {
            get { return tileWidth; }
        }
        public int Height
        {
            get { return tileHeight; }
        }

        public bool Active
        {
            get { return active; }
        }

        public Rectangle FrameBounds
        {
            get
            {
                int x = frame % tilesX * tileWidth;
                int y = frame / tilesX * tileHeight;
                return new Rectangle(x, y, tileWidth, tileHeight);
            }
        }

        public Animation(Texture2D texture, int tilesX, int tilesY, double frameRate, bool looping)
            : this(texture, tilesX, tilesY, frameRate, tilesX * tilesY, looping)
        {

        }

        public Animation(Texture2D texture, int tilesX, int tilesY, double frameRate, int frameCount, bool looping)
        {
            this.texture = texture; // Texture'ı sakla
            this.tilesX = tilesX;
            this.tilesY = tilesY;

            if (tilesX == 0) tilesX = 1;
            this.tileWidth = texture.Width / tilesX;
            if (tilesY == 0) tilesY = 1;
            this.tileHeight = texture.Height / tilesY;
            this.frameCount = frameCount;

            if (frameRate == 0) frameRate = 1;
            this.frameInterval = 1 / frameRate;

            this.frameTimeRemaining = frameInterval;

            this.looping = looping;

            active = true;

            textureDataObj = new TextureData(texture, this.tilesX, this.tilesY);
            textureDataObj.InitializeLazy();
        }

        public void Update()
        {
            if (!active)
                return;

            if (frame == frameCount - 1)
            {
                if (looping == false)
                    active = false;
            }

            this.frameTimeRemaining -= Globals.GameTime.ElapsedGameTime.TotalSeconds;
            if (this.frameTimeRemaining <= 0)
            {
                this.frame++;
                this.frame %= this.frameCount;
                this.frameTimeRemaining = this.frameInterval;
            }

        }

        public int GetFrameCount()
        {
            return this.frameCount;
        }

        public int GetCurrentFrame()
        {
            return this.frame;
        }

        /// <summary>
        /// Bu animation için TextureData objesi oluşturur.
        /// Sprite'ların Load() metodunda kullanılmak üzere.
        /// </summary>
        public TextureData CreateTextureData()
        {
            return new TextureData(this.texture, this.tilesX, this.tilesY);
        }

        public Color[] GetSpecificAreaColorArray()
        {
            if(textureDataObj==null)
                return new Color[1] { Color.Transparent };

            return textureDataObj.GetSpecificAreaColorArray(frame);
        }

    }
}
