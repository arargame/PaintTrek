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
        public MenuButton button;

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
            button = new MenuButton(Text, Vector2.Zero);
        }

        public MenuEntry(string text,bool enabled,int entryNumber) 
        {
            this.Text = text;
            this.Enabled = enabled;
            this.entryNumber = entryNumber;
            this.entryRect = Rectangle.Empty;
            clickableArea = new ClickableArea();
            button = new MenuButton(Text, Vector2.Zero);
        }

        public void Update() 
        {
            button?.SetInfo(Text, new Vector2(entryRect.X, entryRect.Y), Enabled);
        }

        public void Draw(Vector2 position, bool isSelected, bool inverted = false)
        {
            button.SetInfo(Text, position, Enabled);
            entryRect = button.ButtonRect;
            clickableArea.SetRect(entryRect);
            button.Draw(isSelected || clickableArea.IsOverlapped, inverted);
            
        }

        public void SetEntryRect(Vector2 position) 
        {
            button.SetInfo(Text, position, Enabled);
            entryRect = button.ButtonRect;
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
