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
            
            this.clickableArea = new ClickableArea(Rectangle.Empty);
            RecalculatePosition();
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
