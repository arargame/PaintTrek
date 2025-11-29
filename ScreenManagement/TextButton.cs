using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaintTrek
{
    class TextButton : Button
    {
        public event EventHandler Click;

        public TextButton(string text, Vector2 position) : base()
        {
            this.text = text;
            this.position = position;
            
            Vector2 size = font.MeasureString(text);
            this.rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            this.clickableArea = new ClickableArea(this.rect);
        }
        
        // Set owner screen for clickable area
        public void SetOwnerScreen(Screen screen)
        {
            if (this.clickableArea != null && screen != null)
            {
                this.clickableArea.OwnerScreen = screen;
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsClicked)
            {
                if (Click != null)
                    Click(this, EventArgs.Empty);
            }
        }

        public Rectangle getRect()
        {
            return rect;
        }
    }
}
