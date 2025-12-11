using System;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    /// <summary>
    /// Pulsate effect - changes scale of objects periodically
    /// </summary>
    class PulsateEffect
    {
        private float originalScale;
        private float speed;
        private float range;
        private float currentScale;

        /// <summary>
        /// Current scale value
        /// </summary>
        public float Scale
        {
            get { return currentScale; }
        }

        /// <summary>
        /// Creates PulsateEffect
        /// </summary>
        /// <param name="originalScale">Starting scale (e.g., 1.0f)</param>
        /// <param name="speed">Pulse speed (e.g., 6.0f)</param>
        /// <param name="range">Variation amount (e.g., 0.05f)</param>
        public PulsateEffect(float originalScale = 1.0f, float speed = 4.0f, float range = 0.05f)
        {
            this.originalScale = originalScale;
            this.speed = speed;
            this.range = range;
            this.currentScale = originalScale;
        }

        /// <summary>
        /// Updates the effect
        /// </summary>
        public void Update()
        {
            if (Globals.GameTime == null)
                return;

            double time = Globals.GameTime.TotalGameTime.TotalSeconds;

            // Sine wave for smooth transition
            // (Sin + 1) -> 0 to 2
            // multiplied by range -> 0 to 2*range
            // added to originalScale -> oscilates between originalScale and originalScale + 2*range
            
            float pulsate = ((float)Math.Sin(time * speed) + 1.0f); 
            pulsate = pulsate * range; 
            
            currentScale = originalScale + pulsate;
        }

        /// <summary>
        /// Resets scale value
        /// </summary>
        public void Reset()
        {
            currentScale = originalScale;
        }

        /// <summary>
        /// Applies effect to a Rectangle (scales from center)
        /// </summary>
        public Rectangle Apply(Rectangle originalRect)
        {
            int centerX = originalRect.X + originalRect.Width / 2;
            int centerY = originalRect.Y + originalRect.Height / 2;

            int newWidth = (int)(originalRect.Width * currentScale);
            int newHeight = (int)(originalRect.Height * currentScale);

            return new Rectangle(
                centerX - newWidth / 2,
                centerY - newHeight / 2,
                newWidth,
                newHeight
            );
        }

        /// <summary>
        /// Applies effect to a Vector2 size
        /// </summary>
        public Vector2 ApplyToSize(Vector2 originalSize)
        {
            return new Vector2(
                originalSize.X * currentScale,
                originalSize.Y * currentScale
            );
        }
    }
}
