using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class Animation
    {

        private int tilesX;
        private int tilesY;
        public int tileWidth;
        public int tileHeight;

        private int frame;
        private int frameCount;

        private double frameInterval;
        private double frameTimeRemaining;


        private bool active;
        private bool looping;

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

    }
}
