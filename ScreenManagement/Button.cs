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
        public ClickableArea clickableArea; // Made public for owner screen registration
        protected SpriteFont font;
        public Color color;
        public Color backgroundColor = Color.Transparent;
        public bool hasBackground = false;
        protected Color hoverColor = Color.Yellow;
        protected bool isOverlapped;
        protected float scale = 1.0f; // Font scale

        // Icon Support
        private Texture2D iconTexture;
        public Texture2D IconTexture 
        { 
            get => iconTexture; 
            set { iconTexture = value; RecalculatePosition(); } 
        }
        public Rectangle? IconSourceRect { get; set; }
        
        private float iconScale = 1.0f;
        public float IconScale 
        { 
            get => iconScale; 
            set { iconScale = value; RecalculatePosition(); } 
        }
        public Vector2 IconOffset { get; set; } = Vector2.Zero;
        public bool IconOnLeft { get; set; } = true;
        
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

        public virtual void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            anchor = Anchor.None;
            RecalculatePosition();
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
            
            // Update rect and clickable area - scale and icon into account
            if (font != null && text != null)
            {
                Vector2 size = font.MeasureString(text) * scale;
                float iconSpace = 0;
                if (IconTexture != null)
                {
                    float iconW = IconSourceRect?.Width ?? IconTexture.Width;
                    iconSpace = (iconW * IconScale) + 10; // Icon + 10px gap
                }
                rect = new Rectangle((int)position.X, (int)position.Y, (int)(size.X + iconSpace), (int)Math.Max(size.Y, (IconSourceRect?.Height ?? (IconTexture?.Height ?? 0)) * IconScale));
                
                // CRITICAL: Use SetRect which updates position and size internally
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

        private static Texture2D _singlePixelTexture;
        private static Texture2D GetSinglePixel()
        {
            if (_singlePixelTexture == null || _singlePixelTexture.IsDisposed)
            {
                try
                {
                    _singlePixelTexture = Globals.Content.Load<Texture2D>("Textures/singlePixel");
                }
                catch
                {
                    // Fallback to programmatically generated 1x1 texture if content load fails
                    var gd = Globals.Graphics.GraphicsDevice;
                    _singlePixelTexture = new Texture2D(gd, 1, 1);
                    _singlePixelTexture.SetData(new[] { Color.White });
                }
            }
            return _singlePixelTexture;
        }

        public virtual void Draw()
        {
            if (clickableArea != null)
            {
                Globals.SpriteBatch.Begin();

                // Draw Background if enabled
                var singlePixel = GetSinglePixel();
                if (hasBackground && singlePixel != null)
                {
                    // Add some padding to the rect
                    Rectangle bgRect = new Rectangle(rect.X - 10, rect.Y - 5, rect.Width + 20, rect.Height + 10);
                    Globals.SpriteBatch.Draw(singlePixel, bgRect, backgroundColor);
                }

                // Content (Icon + Text) logic
                Vector2 drawPos = position;
                float iconW = 0;
                float iconH = 0;
                if (IconTexture != null)
                {
                    iconW = (IconSourceRect?.Width ?? IconTexture.Width) * IconScale;
                    iconH = (IconSourceRect?.Height ?? IconTexture.Height) * IconScale;
                }

                Vector2 textSize = font.MeasureString(text) * scale;
                float contentHeight = Math.Max(textSize.Y, iconH);

                if (IconTexture != null && IconOnLeft)
                {
                    Vector2 iconPos = new Vector2(drawPos.X + IconOffset.X, drawPos.Y + (contentHeight - iconH) / 2 + IconOffset.Y);
                    Globals.SpriteBatch.Draw(IconTexture, iconPos, IconSourceRect, Color.White, 0f, Vector2.Zero, IconScale, SpriteEffects.None, 0f);
                    drawPos.X += iconW + 10;
                }

                // Draw text with scale
                Vector2 textFinalPos = new Vector2(drawPos.X, drawPos.Y + (contentHeight - textSize.Y) / 2);
                Globals.SpriteBatch.DrawString(font, text, textFinalPos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                if (IconTexture != null && !IconOnLeft)
                {
                    Vector2 iconPos = new Vector2(textFinalPos.X + textSize.X + 10 + IconOffset.X, drawPos.Y + (contentHeight - iconH) / 2 + IconOffset.Y);
                    Globals.SpriteBatch.Draw(IconTexture, iconPos, IconSourceRect, Color.White, 0f, Vector2.Zero, IconScale, SpriteEffects.None, 0f);
                }

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
