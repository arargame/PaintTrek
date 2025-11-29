using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaintTrek
{
    // ControllersScreen provides a tutorial UI for input controls.
    class ControllersScreen : GameScreen
    {
        // DroppingLightSystem is kept active for visual effects.
        DroppingLightSystem droppingLightSystem;
        Player testPlayer;
        SpriteSystem spriteSystem;
        Color foreColor;
        Texture2D button;
        Texture2D fill;
        SpriteFont font;
        Vector2 position;
        MobileController mobileController;
        private BackButton backButton; // Added back button field

        public ControllersScreen()
        {
            Initialize();

            // Reset global lists and systems.
            SpriteSystem.ClearList();
            GunSystem.Clear();
            droppingLightSystem = new DroppingLightSystem();
            testPlayer = new Player();
            testPlayer.Load(); // Load player texture and animation
            spriteSystem = new SpriteSystem();
            button = Globals.Content.Load<Texture2D>("Textures/button");
            font = Globals.MenuFont;
            fill = Globals.Content.Load<Texture2D>("Textures/fill");
            foreColor = Color.Blue;
            position = new Vector2(Globals.GameSize.X / 3, Globals.GameSize.Y * 0.1f);
            mobileController = new MobileController();
            backButton = new BackButton("Back", this, true); // Initialize back button (enabled)
        }

        ~ControllersScreen()
        {
            UnloadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Controllers Screen";
            Globals.Window.Title = screenTitle;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
            droppingLightSystem.Update(); // Keep DroppingLightSystem active
            spriteSystem.Update();
            backButton.Update(); // Update back button state
            mobileController.Update(this.testPlayer);
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();

            // Draw background effects first
            droppingLightSystem.Draw(); // Render dropping light effects

#if WINDOWS_PHONE
            mobileController.Draw();
#endif

            Globals.SpriteBatch.Begin();
            string str1, str2;

#if WINDOWS_PHONE
            str1 = "Tap Screen or Red Area ";
            str2 = "Fire";
            Globals.SpriteBatch.DrawString(font, str1, position, Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y), mobileController.pressedFire || inputState.IsMouseLeftPressed(), str2);

            str1 = "Press 'Up' To Move ";
            str2 = "Up";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 50), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 50), mobileController.pressedUp, str2);

            str1 = "Press 'Down' To Move ";
            str2 = "Down";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 100), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 100), mobileController.pressedDown, str2);

            str1 = "Press 'Left' To Move ";
            str2 = "Left";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 150), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 150), mobileController.pressedLeft, str2);

            str1 = "Press 'Right' To Move ";
            str2 = "Right";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 200), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 200), mobileController.pressedRight, str2);

            str1 = "Press 'Back Button' To ";
            str2 = "Pause";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 250), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 250), Keyboard.GetState().IsKeyDown(Keys.P) || Keyboard.GetState().IsKeyDown(Keys.Escape), str2);

#elif WINDOWS
            str1 = "Press 'Space' or 'K' or Click Left To ";
            str2 = "Fire";
            Globals.SpriteBatch.DrawString(font, str1, position, Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y), Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.K) || Mouse.GetState().LeftButton == ButtonState.Pressed, str2);

            str1 = "Press 'W' or 'Up' To Move ";
            str2 = "Up";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 50), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 50), Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up), str2);

            str1 = "Press 'S' or 'Down' To Move ";
            str2 = "Down";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 100), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 100), Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down), str2);

            str1 = "Press 'A' or 'Left' To Move ";
            str2 = "Left";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 150), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 150), Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left), str2);

            str1 = "Press 'D' or 'Right' To Move ";
            str2 = "Right";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 200), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 200), Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right), str2);

            str1 = "Press 'P' or 'Esc' To ";
            str2 = "Pause";
            Globals.SpriteBatch.DrawString(font, str1, new Vector2(position.X, position.Y + 250), Color.White);
            DrawKey(new Vector2(position.X + font.MeasureString(str1).X, position.Y + 250), Keyboard.GetState().IsKeyDown(Keys.P) || Keyboard.GetState().IsKeyDown(Keys.Escape), str2);

#else
            #error The platform is not specified or is unsupported by this game.
#endif

            Globals.SpriteBatch.End();
            
            // Draw sprites and back button after main UI
            spriteSystem.Draw();
            backButton.Draw(); // Render back button last so it appears on top
        }

        public override void HandleInput()
        {
            base.HandleInput();
            InputState input = inputState;
            
            // Check if back button is clicked
            if (backButton.IsClicked)
            {
                ExitScreen();
                return;
            }
            
            testPlayer.HandleInput(input);
            if (input.Fire)
            {
                testPlayer.Attack();
            }
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
            ScreenManager.AddScreen(new OptionsScreen());
        }

        // Helper method to draw a key label with optional highlight.
        private void DrawKey(Vector2 position, bool pressed, string label)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, (int)font.MeasureString(label).X, (int)font.MeasureString(label).Y);
            Rectangle rect2 = new Rectangle((int)position.X, (int)position.Y, (int)(rect.Width + font.MeasureString(label).X), rect.Height);

            if (pressed)
                Globals.SpriteBatch.Draw(fill, new Rectangle((int)position.X + 1, (int)position.Y + 1, rect2.Width - 2, rect2.Height - 2), Color.CornflowerBlue);

            Globals.SpriteBatch.Draw(button, new Rectangle(rect.Left, rect.Top, (int)(rect.Width + font.MeasureString(label).X), rect.Height), Color.Blue);
            Globals.SpriteBatch.DrawString(font, label, new Vector2(position.X + rect2.Width / 4, position.Y), Color.White);
        }
    }
}
