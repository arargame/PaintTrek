using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    /// <summary>
    /// Desktop counterpart of the Android menu button. It owns visual state only; MenuEntry owns
    /// hit-testing so keyboard, mouse and future controller navigation use the same selection path.
    /// </summary>
    internal sealed class MenuButton
    {
        private string text;
        private Vector2 position;
        private readonly SpriteFont font;
        private readonly Texture2D fillTexture;
        private readonly Vector2 padding;
        private readonly float scale;

        public Rectangle ButtonRect { get; private set; }
        public bool Enabled { get; private set; }

        public MenuButton(string text, Vector2 position, float scale = 1.1f)
        {
            this.text = text;
            this.position = position;
            this.scale = scale;
            font = Globals.MenuFont;
            padding = new Vector2(25, 10) * scale;
            try
            {
                fillTexture = Globals.Content.Load<Texture2D>("Textures/fill");
            }
            catch
            {
                fillTexture = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
                fillTexture.SetData(new[] { Color.White });
            }

            UpdateRect();
        }

        public void SetInfo(string newText, Vector2 newPosition, bool enabled)
        {
            text = newText;
            position = newPosition;
            Enabled = enabled;
            UpdateRect();
        }

        public void Draw(bool selectedOrHovered, bool inverted)
        {
            Globals.SpriteBatch.Begin();

            Color background = inverted
                ? (selectedOrHovered ? Color.Black : Color.White * 0.92f)
                : (!Enabled ? Color.Black * 0.35f : selectedOrHovered ? Color.White : Color.Black * 0.72f);
            Globals.SpriteBatch.Draw(fillTexture, ButtonRect, background);

            Color textColor = !Enabled ? Color.Gray : inverted
                ? (selectedOrHovered ? Color.White : Color.Black)
                : (selectedOrHovered ? Color.Black : Color.White);
            Vector2 textPosition = position + padding;
            Globals.SpriteBatch.DrawString(font, text, textPosition, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Color borderColor = inverted
                ? (selectedOrHovered ? Color.White * 0.8f : Color.Black * 0.55f)
                : (selectedOrHovered ? Color.Black * 0.55f : Color.White * 0.45f);
            DrawBorder(borderColor, selectedOrHovered ? 2 : 1);
            Globals.SpriteBatch.End();
        }

        private void DrawBorder(Color color, int thickness)
        {
            Globals.SpriteBatch.Draw(fillTexture, new Rectangle(ButtonRect.X, ButtonRect.Y, ButtonRect.Width, thickness), color);
            Globals.SpriteBatch.Draw(fillTexture, new Rectangle(ButtonRect.X, ButtonRect.Bottom - thickness, ButtonRect.Width, thickness), color);
            Globals.SpriteBatch.Draw(fillTexture, new Rectangle(ButtonRect.X, ButtonRect.Y, thickness, ButtonRect.Height), color);
            Globals.SpriteBatch.Draw(fillTexture, new Rectangle(ButtonRect.Right - thickness, ButtonRect.Y, thickness, ButtonRect.Height), color);
        }

        private void UpdateRect()
        {
            Vector2 textSize = font.MeasureString(text) * scale;
            ButtonRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)(textSize.X + (2 * padding.X)),
                (int)(textSize.Y + (2 * padding.Y)));
        }
    }
}
