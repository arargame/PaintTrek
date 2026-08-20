using Microsoft.Xna.Framework;

namespace PaintTrek
{
    static class ScreenShake
    {
        private static float timeRemaining;
        private static float duration;
        private static float power;

        public static Vector2 Offset { get; private set; }

        public static void Trigger(float shakePower = 8f, float shakeDuration = 0.12f)
        {
            power = System.Math.Max(power, shakePower);
            duration = System.Math.Max(duration, shakeDuration);
            timeRemaining = duration;
        }

        public static void Update(GameTime gameTime)
        {
            if (timeRemaining <= 0f)
            {
                Offset = Vector2.Zero;
                power = 0f;
                return;
            }

            timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            float falloff = System.Math.Max(0f, timeRemaining / duration);
            float currentPower = power * falloff * falloff;
            Offset = new Vector2(
                ((float)Globals.Random.NextDouble() * 2f - 1f) * currentPower,
                ((float)Globals.Random.NextDouble() * 2f - 1f) * currentPower);
        }
    }
}
