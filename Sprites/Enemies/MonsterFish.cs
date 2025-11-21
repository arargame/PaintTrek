using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class MonsterFish : Enemy
    {
        public MonsterFish()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            SetCharacterInfo("Monster Fish", 40, 20, (20 + 5 * Level.LevelCounter));
            SetVelocity();
        }

        public override void Load()
        {
            base.Load();
           // SetTextures(Globals.Content.Load<Texture2D>("Sprites/Target/monsterFishSpriteSheet"));
            SetTextures(GlobalTexture.monsterFishTexture);
            animation = new Animation(texture, 3, 1, 6, true);
        }

        public override void Update()
        {
            base.Update();
            SimpleMovement(velocity);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void SetVelocity()
        {
            velocity = new Vector2(-2, 0);

            if (Globals.Graphics.IsFullScreen)
                this.velocity.X = (this.velocity.X * 1280) / 800;
        }

        internal static MonsterFish GetMonsterFish()
        {
            return new MonsterFish();
        }
    }

}
