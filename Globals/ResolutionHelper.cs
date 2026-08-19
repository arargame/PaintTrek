using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    /// <summary>
    /// Keeps the desktop presentation surface independent from the game's coordinate system.
    /// The game always runs in Globals.GameSize coordinates; this class letterboxes that image
    /// into the current window and converts mouse input back into those coordinates.
    /// </summary>
    public static class ResolutionHelper
    {
        private static int lastWidth;
        private static int lastHeight;

        private const float VirtualHeight = 800f;

        public static Rectangle DestinationRectangle { get; private set; }
        public static float Scale { get; private set; } = 1f;

        public static bool Initialize(GraphicsDevice graphicsDevice)
        {
            return EnsureCurrent(graphicsDevice);
        }

        /// <returns>True when the virtual canvas changed and its render target must be recreated.</returns>
        public static bool EnsureCurrent(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice?.PresentationParameters == null)
                return false;

            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;
            if (width <= 0 || height <= 0 || (width == lastWidth && height == lastHeight))
                return false;

            lastWidth = width;
            lastHeight = height;
            Globals.ActualScreenSize = new Vector2(width, height);

            // Same policy as Android: fixed design height, width derived from the live aspect
            // ratio. This fills 16:9/16:10/4:3 without letterbox bars.
            float virtualWidth = (int)Math.Round(VirtualHeight * width / (float)height);
            bool canvasChanged = Globals.GameSize.X != virtualWidth || Globals.GameSize.Y != VirtualHeight;
            Globals.GameSize = new Vector2(virtualWidth, VirtualHeight);
            GraphicSettings.ExecuteAllRect();

            Scale = height / VirtualHeight;
            DestinationRectangle = new Rectangle(0, 0, width, height);
            return canvasChanged;
        }

        public static Vector2 ToVirtual(Vector2 screenPosition)
        {
            if (Scale <= 0f)
                return screenPosition;

            return new Vector2(
                (screenPosition.X - DestinationRectangle.X) / Scale,
                (screenPosition.Y - DestinationRectangle.Y) / Scale);
        }
    }
}
