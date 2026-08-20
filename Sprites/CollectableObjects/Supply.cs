using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    public abstract class Supply:CollectableObject
    {
        public static bool MagnetEnabled = true;
        public static float MagnetRadius = 175f;
        public static float MagnetAccel = 950f;
        public static float MagnetDamping = 1.05f;
        public static float TravelPullOutside = 7f;
        public static float TravelPullInside = 0.8f;
        public static float SpawnSideSpeed = 38f;

        bool isIncreasing;
        double maxScale;
        float R;
        Vector2 magnetVelocity;
        Vector2 naturalVelocity;

        public bool IsBeingPulled { get; private set; }

        protected override bool UsesAutomaticMovement => false;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Reset()
        {
            base.Reset();
            isIncreasing = false;
            maxScale = 1.5f;
            scale = Globals.Random.Next(1, 2);
            R = (float)(Globals.Random.Next(5, 8) / 10f);
            SetVelocity();
            IsBeingPulled = false;
        }

        public override void Update()
        {
            float dt = Math.Min(0.05f, (float)Globals.GameTime.ElapsedGameTime.TotalSeconds);
            magnetVelocity = StepMagnetMotion(magnetVelocity, naturalVelocity, position, dt, out bool isBeingPulled);
            velocity = magnetVelocity * dt;
            IsBeingPulled = isBeingPulled;

            base.Update();

            if (isIncreasing)
            {
                scale += 0.008f;
                if (scale > maxScale)
                    isIncreasing = false;
            }
            else
            {
                scale -= 0.008f;
                if (scale <= 0.5f)
                    isIncreasing = true;
            }
            Rotate(R);
        }

        public override void SetVelocity()
        {
            float travelSpeed = Globals.Random.Next(100, 201);
            float sideSpeed = ((float)Globals.Random.NextDouble() * 2f - 1f) * SpawnSideSpeed;
            naturalVelocity = new Vector2(-travelSpeed, 0f);
            magnetVelocity = naturalVelocity + new Vector2(0f, sideSpeed);
            velocity = Vector2.Zero;
        }

        /// <summary>
        /// The Blocked magnet model: constant radial acceleration plus light damping toward the
        /// natural travel vector. Existing side momentum produces a stable spiral instead of a
        /// rigid straight-line pull.
        /// </summary>
        public static Vector2 StepMagnetMotion(Vector2 currentVelocity, Vector2 naturalVelocity, Vector2 itemCenter, float dt, out bool inField)
        {
            inField = false;
            Player player = Player.Instance;

            if (MagnetEnabled && player != null && player.alive)
            {
                Vector2 toPlayer = player.position - itemCenter;
                float distance = toPlayer.Length();
                if (distance > 1f && distance < MagnetRadius)
                {
                    currentVelocity += toPlayer / distance * (MagnetAccel * dt);
                    inField = true;
                }
            }

            float travelPull = inField ? TravelPullInside : TravelPullOutside;
            currentVelocity += (naturalVelocity - currentVelocity) * (1f - MathF.Exp(-travelPull * dt));
            return naturalVelocity + (currentVelocity - naturalVelocity) * MathF.Exp(-MagnetDamping * dt);
        }
    }
}
