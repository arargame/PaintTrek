using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    class CreditsScreen:GameScreen
    {
        List<string> credits = new List<string>();
        DroppingLightSystem droppingLightSystem;
        List<string> keyLabels = new List<string>();

        List<TextButton> socialButtons = new List<TextButton>();

        public CreditsScreen() 
        {
            Initialize();
        }

        ~CreditsScreen() 
        {
        
        }

        public override void Initialize()
        {
            base.Initialize();
            
            screenTitle = "Credits Screen";
            Globals.Window.Title = screenTitle;
            
            droppingLightSystem = new DroppingLightSystem();

            backButton = new BackButton(Loc.T(LocKeys.Menu.Back), this, true);

            // Social Media Buttons - Center Bottom
            float buttonsY = 620;
            float iconSize = 64f;
            float iconScale = 0.25f;
            float spacing = 20f;
            int totalLinks = PaintTrek.Shared.SocialMediaRegistry.Links.Count;
            float totalW = (iconSize * totalLinks) + (spacing * (totalLinks - 1));
            float startX = (Globals.GameSize.X - totalW) / 2;
            
            socialButtons.Clear();
            for (int i = 0; i < totalLinks; i++)
            {
                var link = PaintTrek.Shared.SocialMediaRegistry.Links[i];
                var buttonPos = new Vector2(startX + i * (iconSize + spacing), buttonsY);
                var btn = new TextButton("", buttonPos);
                btn.IconTexture = Globals.Content.Load<Texture2D>(link.IconPath);
                btn.IconScale = iconScale;
                btn.SetOwnerScreen(this);
                
                string targetUrl = link.Url;
                btn.Click += (s, e) => {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetUrl) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to open URL: {ex.Message}");
                    }
                };

                socialButtons.Add(btn);
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
            droppingLightSystem.Update();

            foreach (var btn in socialButtons)
            {
                btn.Update();
            }
        }

        public override void Draw()
        {
            Vector2 position = new Vector2(100, 200);
            Vector2 origin = new Vector2(0, Globals.GameFont.LineSpacing / 2);

            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            droppingLightSystem.Draw();

            Globals.SpriteBatch.Begin();
            

            // Draw "Programming and Graphics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, Loc.T(LocKeys.Credits.ProgrammingGraphics), position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Koray Arar" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Koray Arar", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing * 2;
            
            // Draw "Musics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, Loc.T(LocKeys.Credits.Musics), position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Sahin Meric" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Sahin Meric", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            
            // Draw "FOLLOW US :" in Beige
            string followUsText = Loc.T(LocKeys.Credits.FollowUs);
            Vector2 followUsPos = new Vector2((Globals.GameSize.X - Globals.GameFont.MeasureString(followUsText).X) / 2, 550);
            Vector2 originFollow = new Vector2(0, Globals.GameFont.LineSpacing / 2);
            Globals.SpriteBatch.DrawString(Globals.GameFont, followUsText, followUsPos, Color.Beige, 0, originFollow, 1f, SpriteEffects.None, 0);

            Globals.SpriteBatch.End();

            foreach (var btn in socialButtons)
            {
                btn.Draw();
            }

            base.Draw();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }


    }
}
