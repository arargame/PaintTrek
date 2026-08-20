using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    /// <summary>
    /// A short, clearly labelled first-party promotion displayed before a level starts.
    /// The selected card is random so the two Android games receive equal exposure.
    /// </summary>
    class HousePromotionScreen : GameScreen
    {
        private const double PromotionDurationSeconds = 5.0;

        private readonly GameBoard gameBoard;
        private readonly Promotion promotion;

        private Texture2D pixel;
        private Texture2D artwork;
        private double elapsedSeconds;
        private float pulse;
        private Rectangle cardRect;
        private Rectangle playRect;
        private ClickableArea cardArea;
        private ClickableArea playArea;

        private readonly struct Promotion
        {
            public Promotion(string title, string description, string assetName, string storeUrl, Color accent)
            {
                Title = title;
                Description = description;
                AssetName = assetName;
                StoreUrl = storeUrl;
                Accent = accent;
            }

            public string Title { get; }
            public string Description { get; }
            public string AssetName { get; }
            public string StoreUrl { get; }
            public Color Accent { get; }
        }

        public HousePromotionScreen(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
            promotion = (Globals.Random ?? new Random()).Next(2) == 0
                ? new Promotion(
                    "BLOCKED: PIXEL PANZER",
                    "Defend the city with your tank, build your skills, and chase the next high score.",
                    "SelfAds/BlockedIcon",
                    "https://play.google.com/store/apps/details?id=com.arargames.blocked",
                    new Color(250, 153, 38))
                : new Promotion(
                    "PAINT TREK",
                    "A colorful arcade shooter with hand-drawn worlds, bosses, and fast score chasing.",
                    "SelfAds/PaintTrekIcon",
                    "https://play.google.com/store/apps/details?id=com.arargame.PaintTrek.Android",
                    new Color(61, 156, 255));

            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Discover Arar Games";
            Globals.Window.Title = screenTitle;

            int width = (int)Globals.GameSize.X;
            int height = (int)Globals.GameSize.Y;
            int cardWidth = Math.Min((int)(width * 0.70f), 900);
            int cardHeight = Math.Min((int)(height * 0.58f), 470);
            cardRect = new Rectangle((width - cardWidth) / 2, (height - cardHeight) / 2 - 20, cardWidth, cardHeight);
            playRect = new Rectangle(width - 190, height - 105, 150, 58);

            cardArea = new ClickableArea(cardRect) { OwnerScreen = this };
            playArea = new ClickableArea(playRect) { OwnerScreen = this };
            RegisterClickableArea(cardArea);
            RegisterClickableArea(playArea);
        }

        public override void Load()
        {
            base.Load();
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");
            artwork = Globals.Content.Load<Texture2D>(promotion.AssetName);
        }

        public override void Update()
        {
            if (screenState != ScreenState.Active)
                return;

            inputState.Update();
            elapsedSeconds += Globals.GameTime.ElapsedGameTime.TotalSeconds;
            pulse += (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * 2.5f;

            if (elapsedSeconds >= 0.2 && cardArea.IsClicked)
            {
                OpenStorePage();
            }

            bool mayContinue = elapsedSeconds >= PromotionDurationSeconds;
            if (mayContinue && (playArea.IsClicked || inputState.MenuSelect || inputState.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Space)))
            {
                ContinueToLevel();
            }
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            Globals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp);

            float shimmer = (MathF.Sin(pulse) + 1f) * 0.5f;
            Globals.SpriteBatch.Draw(pixel, cardRect, new Color(10, 17, 31));
            DrawBorder(cardRect, 3, Color.Lerp(promotion.Accent * 0.60f, Color.White, shimmer * 0.22f));

            Rectangle contentRect = new Rectangle(cardRect.X + 20, cardRect.Y + 20, cardRect.Width - 40, cardRect.Height - 72);
            int artworkSize = Math.Min(contentRect.Height - 20, contentRect.Width / 3);
            Rectangle artworkRect = new Rectangle(contentRect.X + 10, contentRect.Center.Y - artworkSize / 2, artworkSize, artworkSize);
            Globals.SpriteBatch.Draw(artwork, artworkRect, Color.White);
            DrawBorder(artworkRect, 2, Color.White * 0.30f);

            int textX = artworkRect.Right + 30;
            float textY = contentRect.Y + 28;
            Globals.SpriteBatch.DrawString(gameFont, promotion.Title, new Vector2(textX, textY), Color.White, 0f, Vector2.Zero, 1.05f, SpriteEffects.None, 0f);
            textY += gameFont.LineSpacing * 1.5f;

            string description = WrapText(promotion.Description, cardRect.Right - textX - 25, 0.72f);
            Globals.SpriteBatch.DrawString(gameFont, description, new Vector2(textX, textY), Color.LightGray, 0f, Vector2.Zero, 0.72f, SpriteEffects.None, 0f);

            string tapHint = "CLICK THE CARD TO VIEW ON GOOGLE PLAY";
            Vector2 hintSize = gameFont.MeasureString(tapHint) * 0.52f;
            Globals.SpriteBatch.DrawString(gameFont, tapHint, new Vector2(cardRect.Center.X - hintSize.X / 2, cardRect.Bottom - 39), Color.Gold, 0f, Vector2.Zero, 0.52f, SpriteEffects.None, 0f);

            if (elapsedSeconds < PromotionDurationSeconds)
            {
                DrawCountdown();
            }
            else
            {
                Globals.SpriteBatch.Draw(pixel, playRect, playArea.IsOverlapped ? Color.White : promotion.Accent);
                DrawBorder(playRect, 2, Color.White * 0.8f);
                string continueText = "CONTINUE";
                Vector2 textSize = gameFont.MeasureString(continueText) * 0.58f;
                Globals.SpriteBatch.DrawString(gameFont, continueText, new Vector2(playRect.Center.X - textSize.X / 2, playRect.Center.Y - textSize.Y / 2), playArea.IsOverlapped ? Color.Black : Color.White, 0f, Vector2.Zero, 0.58f, SpriteEffects.None, 0f);
            }

            Globals.SpriteBatch.End();
        }

        public override void HandleInput()
        {
            // Input is read directly in Update so only this topmost overlay can consume it.
        }

        public override void ExitScreen()
        {
            ContinueToLevel();
        }

        private void ContinueToLevel()
        {
            if (screenState == ScreenState.Inactive)
                return;

            screenState = ScreenState.Inactive;
            CleanupClickableAreas();
            gameBoard.ResumeAfterHousePromotion();
        }

        private void OpenStorePage()
        {
            try
            {
                Process.Start(new ProcessStartInfo(promotion.StoreUrl) { UseShellExecute = true });
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"[HousePromotion] Could not open store page: {exception.Message}");
            }
        }

        private void DrawCountdown()
        {
            const int squareSize = 18;
            const int squareGap = 12;
            const int totalSquares = 5;
            int totalWidth = totalSquares * squareSize + (totalSquares - 1) * squareGap;
            int startX = (int)Globals.GameSize.X - totalWidth - 40;
            int y = (int)Globals.GameSize.Y - squareSize - 42;
            int visibleSquares = Math.Min(totalSquares, (int)Math.Ceiling(PromotionDurationSeconds - elapsedSeconds));

            for (int index = 0; index < totalSquares; index++)
            {
                Rectangle square = new Rectangle(startX + index * (squareSize + squareGap), y, squareSize, squareSize);
                Globals.SpriteBatch.Draw(pixel, square, Color.White * 0.16f);
                if (index < visibleSquares)
                    Globals.SpriteBatch.Draw(pixel, square, Color.White * 0.85f);
            }
        }

        private void DrawBorder(Rectangle rectangle, int thickness, Color color)
        {
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rectangle.Right - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }

        private string WrapText(string text, float maxWidth, float scale)
        {
            string[] words = text.Split(' ');
            string currentLine = string.Empty;
            string result = string.Empty;

            foreach (string word in words)
            {
                string candidate = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (gameFont.MeasureString(candidate).X * scale <= maxWidth)
                {
                    currentLine = candidate;
                }
                else
                {
                    result += currentLine + "\n";
                    currentLine = word;
                }
            }

            return result + currentLine;
        }
    }
}
