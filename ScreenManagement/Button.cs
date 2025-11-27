using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    public enum Anchor
    {
        None,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight
    }

    abstract class Button
    {
        protected string text;
        protected Vector2 position;
        protected Rectangle rect;
        protected ClickableArea clickableArea;
        protected SpriteFont font;
        protected Color color;
        protected Color hoverColor = Color.Yellow;
        protected bool isOverlapped;
        
        protected Anchor anchor;
        protected Vector2 offset;
        protected Vector2 lastGameSize;

        public bool IsClicked
        {
            get { return clickableArea != null && clickableArea.IsClicked; }
        }

        public Rectangle Rect
        {
            get { return rect; }
        }

        public Button()
        {
            color = Color.White;
            font = Globals.GameFont;
            // Pre-load sound
            SoundManager.Load("menu-click", "Sounds/SoundEffects/menu-click");
            lastGameSize = Globals.GameSize;
            anchor = Anchor.None;
        }

        public virtual void Update()
        {
            if (Globals.GameSize != lastGameSize)
            {
                RecalculatePosition();
                lastGameSize = Globals.GameSize;
            }

            // Note: clickableArea.Update() is called by InputState.Update()
            // We only check the state here
            if (clickableArea != null)
            {
                if (clickableArea.IsOverlapped)
                {
                    color = hoverColor;
                    if (!isOverlapped)
                    {
                        // Play sound only if entering overlap
                         SoundManager.Play("menu-click");
                    }
                    isOverlapped = true;
                }
                else
                {
                    color = Color.White;
                    isOverlapped = false;
                }
            }
        }

        protected virtual void RecalculatePosition()
        {
            if (anchor == Anchor.BottomLeft)
            {
                position = new Vector2(offset.X, Globals.GameSize.Y - offset.Y);
            }
            else if (anchor == Anchor.BottomRight)
            {
                position = new Vector2(Globals.GameSize.X - offset.X, Globals.GameSize.Y - offset.Y);
            }
            
            // Update rect and clickable area
            if (font != null && text != null)
            {
                Vector2 size = font.MeasureString(text);
                rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                
                // CRITICAL: Use SetRect which updates position and size internally
                // This ensures ClickableArea.Update() rebuilds the rect correctly
                if (clickableArea != null)
                    clickableArea.SetRect(rect);
            }
        }

        public void SetAnchor(Anchor anchor, Vector2 offset)
        {
            this.anchor = anchor;
            this.offset = offset;
            RecalculatePosition(); // Apply immediately
        }

        public virtual void Draw()
        {
            if (clickableArea != null)
            {
                if (Globals.DebugMode)
                    clickableArea.Draw(); // Show yellow debug rectangle
                    
                Globals.SpriteBatch.Begin();
                Globals.SpriteBatch.DrawString(font, text, position, color);
                Globals.SpriteBatch.End();
            }
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
