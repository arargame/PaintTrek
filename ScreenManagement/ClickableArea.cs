using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PaintTrek
{
    class ClickableArea
    {
        Texture2D texture;
        Rectangle rectangle;
        Vector2 position;
        Vector2 size;

        public bool IsOverlapped;
        public bool IsClicked;

        public ClickableArea(Rectangle rect) 
        {
            texture = Globals.Content.Load<Texture2D>("Textures/clickableArea");
            this.rectangle = rect;
            this.position = new Vector2(rectangle.X, rectangle.Y);

            if (size == Vector2.Zero)
                this.size = new Vector2(rectangle.Width, rectangle.Height);
            else
                this.size = new Vector2(texture.Width, texture.Height);

            ClickableAreaSystem.Add(this);
        }

        public ClickableArea(Texture2D texture, Rectangle rectangle) 
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.position = new Vector2(rectangle.X, rectangle.Y);

            if (size == Vector2.Zero)
                this.size = new Vector2(rectangle.Width, rectangle.Height);
            else
                this.size = new Vector2(texture.Width, texture.Height);

            ClickableAreaSystem.Add(this);
        }

        public ClickableArea()
        {
            texture = Globals.Content.Load<Texture2D>("Textures/clickableArea");
            position = Vector2.Zero;
            size = Vector2.Zero;
            ClickableAreaSystem.Add(this);
        }

        ~ClickableArea() 
        {
            texture.Dispose();
        }

        public void Initalize()
        {
        
        }

        public void Load() 
        {
        
        }

        public void Update() 
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public void Draw() 
        {
            if (Globals.DebugMode)
            {
                // Draw yellow rectangle for debug
                Globals.SpriteBatch.Begin();
                Globals.SpriteBatch.Draw(texture, rectangle, Color.Yellow * 0.3f);
                Globals.SpriteBatch.End();
            }
        }

        public void SetRect(Rectangle rect)
        {
            this.rectangle = rect;
            this.position = new Vector2(rect.X, rect.Y);
            this.size = new Vector2(rect.Width, rect.Height);
        }
        public Rectangle GetRect()
        {
            return rectangle;
        }

        public void RemoveFromSystem()
        {
            ClickableAreaSystem.Remove(this);
        }

    }

    //Clickable Area System Class
    class ClickableAreaSystem
    {
        public static List<ClickableArea> clickableAreas;

        public ClickableAreaSystem()
        {
            clickableAreas = new List<ClickableArea>();
        }
        public static void Add(ClickableArea ca)
        {
            clickableAreas.Add(ca);
        }

        public void Update()
        {
            for (int i = 0; i < clickableAreas.Count; i++)
            {
                clickableAreas[i].Update();
            }
        }

        public void Draw()
        {
            for (int i = 0; i < clickableAreas.Count; i++)
            {
                clickableAreas[i].Draw();
            }
        }

        public static void Clear()
        {
            clickableAreas.Clear();
        }



        public static void Remove(ClickableArea ca)
        {
            clickableAreas.Remove(ca);
        }

    }
}
