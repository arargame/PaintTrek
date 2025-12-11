using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class LevelCard
    {
        public int LevelNumber { get; private set; }
        public bool IsLocked { get; private set; }
        public int LevelScore { get; private set; }
        public bool IsSelected { get; set; } // New property for keyboard/controller selection
        
        private static Texture2D pixel;
        private Texture2D texture;
        private Rectangle bounds;
        private ClickableArea clickableArea;
        private PulsateEffect pulsateEffect;
        private SpriteFont font;
        
        // Locked level overlay color
        private Color lockedColor = new Color(0, 0, 255, 128); 
        private Color unlockedColor = Color.White;

        public bool IsClicked 
        { 
            get 
            { 
                return !IsLocked && clickableArea != null && clickableArea.IsClicked; 
            } 
        }

        public LevelCard(int levelNumber, Rectangle bounds, bool isLocked, int levelScore = 0)
        {
            this.LevelNumber = levelNumber;
            this.bounds = bounds;
            this.IsLocked = isLocked;
            this.LevelScore = levelScore;
            this.IsSelected = false;
            
            // Load Texture
            try 
            {
                // LoadingScene textures are typically reused for level previews
                this.texture = Globals.Content.Load<Texture2D>("LoadingScene/transitionTexture" + levelNumber);
            }
            catch
            {
                // Fallback handled by drawing placeholder
            }

            if (!IsLocked)
            {
                // Pulsate effect and clickable area for unlocked levels
                this.pulsateEffect = new PulsateEffect(1.0f, 4.0f, 0.04f);
                
                this.clickableArea = new ClickableArea(bounds);
            }
            
            this.font = Globals.GameFont; 
        }

        public void Update()
        {
            if (!IsLocked)
            {
                if (pulsateEffect != null)
                {
                    // Only update pulsate if selected or just to show it's active?
                    // Original Android logic: always pulsate.
                    // Enhanced logic: maybe pulsate more if selected? 
                    // For now, keep it simple.
                    pulsateEffect.Update();
                }
                
                if (clickableArea != null)
                {
                    clickableArea.Update();
                    
                    // Mouse hover logic for selection
                    if (clickableArea.IsOverlapped)
                    {
                        IsSelected = true;
                    }
                }
            }
        }

        public void Draw()
        {
            Rectangle drawRect = bounds;
            
            // Apply pulsate effect if unlocked
            if (!IsLocked && pulsateEffect != null)
            {
                drawRect = pulsateEffect.Apply(bounds);
            }
            
            // Special scaling for Selected item
            if (IsSelected && !IsLocked)
            {
                // Expand a bit more if selected
                drawRect.Inflate(5, 5); 
            }

            // Draw Background/Texture
            if (texture != null)
            {
                Color drawColor = IsLocked ? new Color(50, 50, 200, 200) : Color.White;
                
                // Highlight selected item
                if (IsSelected && !IsLocked)
                {
                     drawColor = Color.Yellow; // Tint yellow if selected
                }

                Globals.SpriteBatch.Draw(texture, drawRect, drawColor);
            }
            else
            {
                // Placeholder
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Textures/clickableArea"), drawRect, IsLocked ? Color.Blue : (IsSelected ? Color.Yellow : Color.Green));
            }
            
            // Draw Selection Border using Primitives if needed, or just rely on Tint/Inflate
            if (IsSelected)
            {
               // Can add a border drawing here if we had a primitive drawing tool
            }

            // Text Rendering
            if (font != null)
            {
                string text = "Level " + LevelNumber;
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPos = new Vector2(
                    drawRect.X + (drawRect.Width - textSize.X) / 2,
                    drawRect.Y + (drawRect.Height - textSize.Y) / 2 - 15
                );
                
                Color textColor = Color.White;
                if (!IsLocked) textColor = Color.Yellow;
                if (IsSelected) textColor = Color.Black; // Contrast against Yellow background if tinted? 
                // Wait, tinting texture completely Yellow might look bad. 
                // Let's stick to White text for better visibility usually. 
                if (IsSelected && !IsLocked && texture != null) textColor = Color.Gold;

                Globals.SpriteBatch.DrawString(font, text, textPos, textColor);
                
            // Score
                if (!IsLocked && LevelScore > 0)
                {
                    string scoreText = "Score: " + LevelScore;
                    Vector2 scoreSize = font.MeasureString(scoreText);
                    Vector2 scorePos = new Vector2(
                        drawRect.X + (drawRect.Width - scoreSize.X) / 2,
                        textPos.Y + textSize.Y + 5
                    );
                    Globals.SpriteBatch.DrawString(font, scoreText, scorePos, Color.LightGreen);
                }
            }

            // Draw Selection Line (Green if unlocked, Red if locked)
            if (IsSelected)
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                }

                // Line below the card
                Rectangle lineRect = new Rectangle(drawRect.X, drawRect.Bottom + 10, drawRect.Width, 5);
                Color lineColor = IsLocked ? Color.Red : Color.Lime; // Red for locked, Green (Lime) for unlocked

                Globals.SpriteBatch.Draw(pixel, lineRect, lineColor);
            }
        }
    }
}
