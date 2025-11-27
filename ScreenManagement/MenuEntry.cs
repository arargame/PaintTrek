using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class MenuEntry
    {

        public bool Enabled;
        public string Text;
        public int entryNumber;
        public Rectangle entryRect;
        public ClickableArea clickableArea;

        public MenuEntry() 
        {
            Enabled = true;
            Text = "Null";
            entryNumber = 0;
            entryRect = Rectangle.Empty;
            clickableArea = new ClickableArea();
        }

        public MenuEntry(string text,bool enabled,int entryNumber) 
        {
            this.Text = text;
            this.Enabled = enabled;
            this.entryNumber = entryNumber;
            this.entryRect = Rectangle.Empty;
            clickableArea = new ClickableArea();
        }

        public void Update() 
        {

        }

        public void Draw( Vector2 position, bool isSelected) 
        {
            Vector2 origin = new Vector2(0, Globals.MenuFont.LineSpacing / 2);

            Color color = isSelected ? Color.Yellow : Color.White;
            color = new Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)255);

            float pulsate = (float)(Math.Sin(Globals.GameTime.TotalGameTime.TotalSeconds * 3) + 1);
            float scale = isSelected ? (1 + pulsate * 0.05f) : 1.0f;

            SetEntryRect(new Vector2(position.X,position.Y-(origin.Y/2)));
            clickableArea.Draw();

            Globals.SpriteBatch.Begin();
            if (Enabled)
            {
                Globals.SpriteBatch.DrawString(Globals.MenuFont, Text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                if (!isSelected)
                    Globals.SpriteBatch.DrawString(Globals.MenuFont, Text, position, Color.Gray, 0, origin, scale, SpriteEffects.None, 0);
                else
                    Globals.SpriteBatch.DrawString(Globals.MenuFont, Text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }
            Globals.SpriteBatch.End();
            
        }

        public void SetEntryRect(Vector2 position) 
        {
            entryRect = new Rectangle((int)position.X,(int)position.Y,(int)Globals.MenuFont.MeasureString(this.Text).X,(int)Globals.MenuFont.MeasureString(this.Text).Y);
            clickableArea.SetRect(entryRect);
        }

        public void Dispose()
        {
            if (clickableArea != null)
            {
                clickableArea.RemoveFromSystem();
            }
        }
    }
}
