using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class SampleEnemy
    {
        Texture2D texture;
        Animation animation;
        Vector2 origin;
        string name;
        double health;
        double damage;
        Vector2 size;
        Rectangle sourceRect;

        public SampleEnemy(Enemy enemy)
        {
            this.texture = enemy.texture;
            this.animation = enemy.animation;
            this.name = enemy.GetName();
            this.health = enemy.GetHealth();
            this.damage = enemy.GetDamage();
            size = enemy.size;
            sourceRect = new Rectangle(animation.FrameBounds.X, animation.FrameBounds.Y, (int)size.X, (int)size.Y);
            enemy = null;
        }

        public void Update()
        {
            if (animation != null)
            {
                animation.Update();
                size = new Vector2(animation.Width, animation.Height);
                sourceRect = new Rectangle(animation.FrameBounds.X, animation.FrameBounds.Y, (int)size.X, (int)size.Y);
                origin = new Vector2(animation.tileWidth / 2, animation.tileHeight / 2);
            }
        }

        public Texture2D GetTexture()
        {
            return texture;
        }

        public string GetName()
        {
            return name;
        }

        public double GetDamage()
        {
            return damage;
        }

        public double GetHealth()
        {
            return health;
        }

        public Vector2 GetSize()
        {
            return size;
        }

        public Rectangle GetSourceRect()
        {
            return sourceRect;
        }

        public Vector2 GetOrigin()
        {
            return origin;
        }

        public void UnLoadContent()
        {
            texture = null;
            animation = null;
        }
    }
}
