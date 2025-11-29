using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaintTrek
{
    class EnemyScreen:MenuScreen
    {
        int counter;

        List<SampleEnemy> enemies;

        BackButton backButton;
        TextButton previousButton;
        TextButton nextButton;

        public EnemyScreen() 
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            enemies = ListCreator();
            counter = 0;
            backButton = new BackButton("Back",this,true);
            
            float fontHeight = Globals.GameFont.MeasureString("Previous").Y;
            
            previousButton = new TextButton("Previous", Vector2.Zero);
            previousButton.SetAnchor(Anchor.BottomLeft, new Vector2(50, fontHeight * 2));
            previousButton.SetOwnerScreen(this);
            RegisterClickableArea(previousButton.clickableArea);

            nextButton = new TextButton("Next", Vector2.Zero);
            float prevWidth = Globals.GameFont.MeasureString("Previous").X;
            nextButton.SetAnchor(Anchor.BottomLeft, new Vector2(50 + prevWidth + 50, fontHeight * 2));
            nextButton.SetOwnerScreen(this);
            RegisterClickableArea(nextButton.clickableArea);

            previousButton.Click += new EventHandler(previousButton_Click);
            nextButton.Click += new EventHandler(nextButton_Click);
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
            backButton.Update();
            previousButton.Update();
            nextButton.Update();
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update();
            }
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();

            backButton.Draw();
            previousButton.Draw();
            nextButton.Draw();

            Vector2 position = new Vector2(Globals.GameSize.X * 0.6f, Globals.GameSize.Y / 3);
            //     Vector2 size=new Vector2(enemyList[counter].animation.Width, enemyList[counter].animation.Height);
            //    Rectangle sourceRectangle = new Rectangle(enemyList[counter].animation.FrameBounds.X, enemyList[counter].animation.FrameBounds.Y, (int)size.X, (int)size.Y); ;
            Vector2 infoPosition = new Vector2(Globals.GameSize.X / 8, Globals.GameSize.Y / 4);
            if (enemies.Count > 0)
            {
                Globals.SpriteBatch.Begin();

                Globals.SpriteBatch.DrawString(Globals.GameFont, "Name :" + enemies[counter].GetName(), infoPosition, Color.White);
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Health :" + enemies[counter].GetHealth(), new Vector2(infoPosition.X, infoPosition.Y + 50), Color.White);
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Damage :" + (-1) * enemies[counter].GetDamage(), new Vector2(infoPosition.X, infoPosition.Y + 100), Color.White);
                Globals.SpriteBatch.Draw(enemies[counter].GetTexture(),
                    position, enemies[counter].GetSourceRect(), Color.White, 0f, enemies[counter].GetOrigin(), new Vector2(1, 1), SpriteEffects.None, 0f);
                Globals.SpriteBatch.End();
            }
            else
            {
                Globals.SpriteBatch.Begin();
                Globals.SpriteBatch.DrawString(Globals.GameFont, "Sorry,there is no enemy to show.", infoPosition, Color.White);
                Globals.SpriteBatch.End();
            }
            
            // Debug visualization
            if (Globals.DebugMode && inputState != null)
            {
                Globals.SpriteBatch.Begin();
                
                // Draw red rectangle for mouse cursor (10x10)
                Texture2D pixel = new Texture2D(Globals.Graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                Rectangle mouseRect = new Rectangle((int)inputState.cursorPosition.X - 5, (int)inputState.cursorPosition.Y - 5, 10, 10);
                Globals.SpriteBatch.Draw(pixel, mouseRect, Color.Red * 0.7f);
                
                // Draw debug info (top-left corner)
                var viewport = Globals.Graphics.GraphicsDevice.Viewport;
                string debugText = $"Mouse: {(int)inputState.cursorPosition.X}, {(int)inputState.cursorPosition.Y}\n";
                debugText += $"Raw Mouse: {Mouse.GetState().X}, {Mouse.GetState().Y}\n";
                debugText += $"Resolution: {(int)Globals.GameSize.X}x{(int)Globals.GameSize.Y}\n";
                debugText += $"Viewport: {viewport.X},{viewport.Y} {viewport.Width}x{viewport.Height}\n";
                debugText += $"Fullscreen: {Globals.Graphics.IsFullScreen}";
                Globals.SpriteBatch.DrawString(Globals.GameFont, debugText, new Vector2(10, 10), Color.Lime);
                
                Globals.SpriteBatch.End();
            }
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }
        
        void previousButton_Click(object sender, EventArgs e)
        {
            MenuLeft(0);
        }

        void nextButton_Click(object sender, EventArgs e)
        {
            MenuRight(0);
        }

        public override void ExitScreen()
        {
            // Dispose sample enemies to prevent memory leak
            if (enemies != null)
            {
                enemies.Clear();
            }
            
            base.ExitScreen();
        }


        private List<SampleEnemy> ListCreator()
        {
            FileSystem FS = new FileSystem("game.save");
            int maxLevel = FS.LoadFile()[1];
            MathHelper.Clamp(maxLevel, 1, 10);

            enemies = new List<SampleEnemy>();

            int c = 1;
            switch (c)
            {
                case 1:
                    enemies.Add(new SampleEnemy(new Cacao()));
                    enemies.Add(new SampleEnemy(new Eye()));
                    enemies.Add(new SampleEnemy(new Asteroid()));
                    enemies.Add(new SampleEnemy(new Boss1()));

                    if (c < maxLevel)
                    {
                        c++;
                        goto case 2;
                    }
                    else break;
                case 2:
                    enemies.Add(new SampleEnemy(new JellyFish()));
                    enemies.Add(new SampleEnemy(new SharpCube()));
                    enemies.Add(new SampleEnemy(new Comet()));
                    enemies.Add(new SampleEnemy(new Boss2()));
                    if (c < maxLevel)
                    {
                        c++;
                        goto case 3;
                    }
                    else break;
                case 3:
                    enemies.Add(new SampleEnemy(new MonsterFish()));
                    enemies.Add(new SampleEnemy(new MRBrain()));
                    enemies.Add(new SampleEnemy(new Bone()));
                    enemies.Add(new SampleEnemy(new Boss3()));
                    if (c < maxLevel)
                    {
                        c++;
                        goto case 4;
                    }
                    else break;

                case 4:
                    enemies.Add(new SampleEnemy(new Bristle()));
                    enemies.Add(new SampleEnemy(new SpaceSnake()));
                    enemies.Add(new SampleEnemy(new Ufo()));
                    enemies.Add(new SampleEnemy(new SnakeStone()));
                    enemies.Add(new SampleEnemy(new Boss4()));
                    if (c < maxLevel)
                    {
                        c++;
                        goto case 5;
                    }
                    else break;

                case 5:
                    enemies.Add(new SampleEnemy(new Boss5()));

                    if (c < maxLevel)
                    {
                        c++;
                        goto case 6;
                    }
                    else break;

                case 6:
                    enemies.Add(new SampleEnemy(new Boss6()));

                    if (c < maxLevel)
                    {
                        c++;
                        goto case 7;
                    }
                    else break;

                case 7:
                    enemies.Add(new SampleEnemy(new Invader1()));
                    enemies.Add(new SampleEnemy(new Invader2()));
                    enemies.Add(new SampleEnemy(new Invader3()));
                    enemies.Add(new SampleEnemy(new Boss7()));

                    if (c < maxLevel)
                    {
                        c++;
                        goto case 8;
                    }
                    else break;

                case 8:
                    enemies.Add(new SampleEnemy(new ChildTrilobit()));
                    enemies.Add(new SampleEnemy(new Boss8()));
                    if (c < maxLevel)
                    {
                        c++;
                        goto case 9;
                    }
                    else break;

                case 9:
                    enemies.Add(new SampleEnemy(new Boss9()));
                    if (c < maxLevel)
                    {
                        c++;
                        goto case 10;
                    }
                    else break;

                case 10:
                    enemies.Add(new SampleEnemy(new Boss10()));
                    break;

                default:
                    break;
            }

            return enemies;
        }

        public override void MenuSelect(int selectedEntry)
        {
            base.MenuSelect(selectedEntry);
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new ExtraScreen());
        }

        public override void MenuLeft(int selectedEntry)
        {
            counter--;
            if (counter < 0)
                counter = enemies.Count - 1;
        }

        public override void MenuRight(int selectedEntry)
        {
            counter++;
            if (counter > enemies.Count - 1)
                counter = 0;
        }
    }
}
