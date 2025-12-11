using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaintTrek
{
    class ContinueScreen : MenuScreen
    {
        private List<LevelCard> levelCards;
        private int maxUnlockedLevel;
        private FileSystem fileSystem;
        private bool isLevelSelected = false;

        private int selectedCardIndex = 0; // For keyboard/controller navigation
        private int rows = 2;
        private int cols = 5;
        private int totalLevels = 10;
        
        // Input handling timers/state
        private TimeSpan inputDelay = TimeSpan.FromMilliseconds(150);
        private TimeSpan lastInputTime = TimeSpan.Zero;


        public ContinueScreen()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Select Level";
            Globals.Window.Title = screenTitle;

            levelCards = new List<LevelCard>();
            
            // Re-using FileSystem logic or GameSettings singleton
            // Using GameSettings as per recent refactors in Desktop
            maxUnlockedLevel = GameSettings.Instance.MaxLevel;
            if (maxUnlockedLevel < 1) maxUnlockedLevel = 1;

            // Grid Layout
            int screenWidth = (int)Globals.GameSize.X;
            int screenHeight = (int)Globals.GameSize.Y;
            
            int marginX = 50;
            int marginY = 100;
            int spacingX = 50; 
            int spacingY = 50; 

            // Footer / Button calculations (Back button handled by MenuScreen or manually?)
            // MenuScreen usually handles MenuEntries. Since this is a custom grid, 
            // we will handle the "Back" button as a manual entry or just listen for Esc/B.
            // But we need to draw a visual "Back" button or hint.
            
            float buttonScale = 1.3f;
            SpriteFont buttonFont = Globals.GameFont;
            Vector2 buttonSize = buttonFont.MeasureString("Previous") * buttonScale;
            // Manual footer Y calculation
            float footerY = screenHeight - buttonSize.Y - 20;
            
            int availableHeight = (int)footerY - marginY - 20;
            int availableWidth = screenWidth - (2 * marginX) - ((cols - 1) * spacingX);
            
            int cardWidth = availableWidth / cols;
            int cardHeight = (availableHeight - ((rows - 1) * spacingY)) / rows;
            
            for (int i = 0; i < totalLevels; i++)
            {
                int levelNum = i + 1;
                int row = i / cols;
                int col = i % cols;

                int x = marginX + (col * (cardWidth + spacingX));
                int y = marginY + (row * (cardHeight + spacingY));

                Rectangle cardRect = new Rectangle(x, y, cardWidth, cardHeight);
                
                // Check lock status
                bool isLocked = levelNum > maxUnlockedLevel;
                
                int levelScore = GameSettings.Instance.GetLevelScore(levelNum);

                LevelCard card = new LevelCard(levelNum, cardRect, isLocked, levelScore);
                levelCards.Add(card);
            }
            
            // Initial selection: Select the last unlocked level to be helpful
            selectedCardIndex = maxUnlockedLevel - 1;
            if (selectedCardIndex < 0) selectedCardIndex = 0;
            if (selectedCardIndex >= levelCards.Count) selectedCardIndex = levelCards.Count - 1;
            
            UpdateSelection();

            // Add a Back menu entry so base MenuScreen handles drawing/input for it?
            // Or just manual? Let's add it as a visual help, but main nav is grid.
            // MenuScreen typically expects vertical list.
            // We'll manage input manually in HandleInput override.
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update()
        {
            base.Update();
            
            foreach (var card in levelCards)
            {
                card.Update();
                
                // Mouse/Touch interaction support from LevelCard
                if (card.IsSelected && !card.IsLocked && IsMouseMoving()) 
                {
                    // If mouse moved and hovered this card, update selected index
                    selectedCardIndex = levelCards.IndexOf(card);
                }
                
                if (card.IsClicked)
                {
                    LoadLevel(card.LevelNumber);
                }
            }
        }
        
        private bool IsMouseMoving()
        {
             // Simple check if mouse moved significantly or is active
             // For now assume if card.IsSelected becomes true via Mouse Hover (in LevelCard), we sync index.
             // LevelCard sets IsSelected = true on hover.
             // But we need to de-select others.
             return true;
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            Globals.SpriteBatch.Begin();

            foreach (var card in levelCards)
            {
                card.Draw();
            }
            
            string title = "SELECT LEVEL";
            if (selectedCardIndex >= 0 && selectedCardIndex < levelCards.Count)
            {
                title += " (" + levelCards[selectedCardIndex].LevelNumber + ")";
            }
            Vector2 titleSize = Globals.GameFont.MeasureString(title);
            Vector2 titlePos = new Vector2(
                (Globals.GameSize.X - titleSize.X) / 2,
                30
            );
            Globals.SpriteBatch.DrawString(Globals.GameFont, title, titlePos, Color.White);

            // Draw "Press ESC to Back" hint
            string backHint = "Press ESC to Back";
            Vector2 hintSize = Globals.GameFont.MeasureString(backHint);
             Vector2 hintPos = new Vector2(
                (Globals.GameSize.X - hintSize.X) / 2,
                Globals.GameSize.Y - 50
            );
            Globals.SpriteBatch.DrawString(Globals.GameFont, backHint, hintPos, Color.Gray);
            
            Globals.SpriteBatch.End();

            // Do not call base.Draw() as it might draw MenuEntries we didn't add or don't want
            // base.Draw(); 
        }

        public override void HandleInput()
        {
            if (inputState == null) return;
            
            // Critical: Update input state since we are overriding base.HandleInput
            inputState.Update();

            // Handle Global Input for Back
            if (inputState.IsNewKeyPress(Keys.Escape) || inputState.IsNewButtonPress(Buttons.B) || inputState.IsNewButtonPress(Buttons.Back))
            {
                ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen());
                return;
            }

            // Grid Navigation
            bool selectionChanged = false;
            
            // Keyboard
            if (inputState.IsNewKeyPress(Keys.Right)) { selectedCardIndex++; selectionChanged = true; }
            if (inputState.IsNewKeyPress(Keys.Left)) { selectedCardIndex--; selectionChanged = true; }
            if (inputState.IsNewKeyPress(Keys.Down)) { selectedCardIndex += cols; selectionChanged = true; }
            if (inputState.IsNewKeyPress(Keys.Up)) { selectedCardIndex -= cols; selectionChanged = true; }

            // GamePad
            if (inputState.IsNewButtonPress(Buttons.DPadRight) || inputState.IsNewButtonPress(Buttons.LeftThumbstickRight)) { selectedCardIndex++; selectionChanged = true; }
            if (inputState.IsNewButtonPress(Buttons.DPadLeft) || inputState.IsNewButtonPress(Buttons.LeftThumbstickLeft)) { selectedCardIndex--; selectionChanged = true; }
            if (inputState.IsNewButtonPress(Buttons.DPadDown) || inputState.IsNewButtonPress(Buttons.LeftThumbstickDown)) { selectedCardIndex += cols; selectionChanged = true; }
            if (inputState.IsNewButtonPress(Buttons.DPadUp) || inputState.IsNewButtonPress(Buttons.LeftThumbstickUp)) { selectedCardIndex -= cols; selectionChanged = true; }

            // Bounds Check
            if (selectionChanged)
            {
                if (selectedCardIndex < 0) selectedCardIndex = 0;
                if (selectedCardIndex >= levelCards.Count) selectedCardIndex = levelCards.Count - 1;
                
                UpdateSelection();
                
                SoundManager.Play("menu-click"); 
            }

            // Selection
            if (inputState.IsNewKeyPress(Keys.Enter) || inputState.IsNewButtonPress(Buttons.A))
            {
                LevelCard selectedCard = levelCards[selectedCardIndex];
                if (!selectedCard.IsLocked)
                {
                    LoadLevel(selectedCard.LevelNumber);
                }
                else
                {
                    // Locked sound?
                }
            }
        }
        
        private void UpdateSelection()
        {
            for (int i = 0; i < levelCards.Count; i++)
            {
                levelCards[i].IsSelected = (i == selectedCardIndex);
            }
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
        }

        private void LoadLevel(int levelNumber)
        {
            Level.LevelCounter = levelNumber;
            
            if (levelNumber > 1)
            {
                int previousLevelScore = GameSettings.Instance.GetLevelScore(levelNumber - 1);
                Level.Score = previousLevelScore;
            }
            else
            {
                Level.Score = 0;
            }

            isLevelSelected = true;
            ExitScreen();
            ScreenManager.AddScreen(GameBoard.CreateNewGame());
        }
    }
}
