using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaintTrek
{
    class CreditsScreen:GameScreen
    {
        List<string> credits = new List<string>();
        DroppingLightSystem droppingLightSystem;
        List<string> keyLabels = new List<string>();

        TextButton xButton;
        TextButton fbButton;
        TextButton ytButton;
        TextButton ttButton;

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

            backButton = new BackButton("Back", this, true);

            // Social Media Buttons - Center Bottom
            float buttonsY = 620;
            float spacing = 170;
            float totalW = spacing * 3 + 120; // estimate width
            float startX = (Globals.GameSize.X - totalW) / 2;
            
            xButton = new TextButton("X (Twitter)", new Vector2(startX, buttonsY));
            xButton.SetOwnerScreen(this);

            fbButton = new TextButton("Facebook", new Vector2(startX + spacing, buttonsY));
            fbButton.SetOwnerScreen(this);
            
            ytButton = new TextButton("YouTube", new Vector2(startX + spacing * 2, buttonsY));
            ytButton.SetOwnerScreen(this);
            
            ttButton = new TextButton("TikTok", new Vector2(startX + spacing * 3, buttonsY));
            ttButton.SetOwnerScreen(this);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
            droppingLightSystem.Update();

            if (xButton != null) xButton.Update();
            if (fbButton != null) fbButton.Update();
            if (ytButton != null) ytButton.Update();
            if (ttButton != null) ttButton.Update();
        }

        public override void Draw()
        {
            Vector2 position = new Vector2(100, 200);
            Vector2 origin = new Vector2(0, Globals.GameFont.LineSpacing / 2);

            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            droppingLightSystem.Draw();

            Globals.SpriteBatch.Begin();
            

            // Draw "Programming and Graphics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Programming and Graphics :", position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Koray Arar" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Koray Arar", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing * 2;
            
            // Draw "Musics :" in Beige
            Globals.SpriteBatch.DrawString(Globals.GameFont, "Musics :", position, Color.Beige, 0, origin, 1f, SpriteEffects.None, 0);
            position.Y += Globals.GameFont.LineSpacing;
            
            // Draw "    Sahin Meric" in White
            Globals.SpriteBatch.DrawString(Globals.GameFont, "    Sahin Meric", position, Color.White, 0, origin, 1f, SpriteEffects.None, 0);
            
            // Draw "FOLLOW US :" in Beige
            Vector2 followUsPos = new Vector2((Globals.GameSize.X - Globals.GameFont.MeasureString("FOLLOW US :").X) / 2, 550);
            Vector2 originFollow = new Vector2(0, Globals.GameFont.LineSpacing / 2);
            Globals.SpriteBatch.DrawString(Globals.GameFont, "FOLLOW US :", followUsPos, Color.Beige, 0, originFollow, 1f, SpriteEffects.None, 0);

            Globals.SpriteBatch.End();

            if (xButton != null) xButton.Draw();
            if (fbButton != null) fbButton.Draw();
            if (ytButton != null) ytButton.Draw();
            if (ttButton != null) ttButton.Draw();

            base.Draw();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (xButton != null && xButton.IsClicked)
            {
#if ANDROID
                var uri = Android.Net.Uri.Parse("https://x.com/arargamesstudio");
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
#else
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://x.com/arargamesstudio") { UseShellExecute = true });
#endif
            }

            if (fbButton != null && fbButton.IsClicked)
            {
#if ANDROID
                var uri = Android.Net.Uri.Parse("https://www.facebook.com/arargamesstudio");
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
#else
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.facebook.com/arargamesstudio") { UseShellExecute = true });
#endif
            }

            if (ytButton != null && ytButton.IsClicked)
            {
#if ANDROID
                var uri = Android.Net.Uri.Parse("https://www.youtube.com/@koreaaria");
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
#else
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.youtube.com/@koreaaria") { UseShellExecute = true });
#endif
            }

            if (ttButton != null && ttButton.IsClicked)
            {
#if ANDROID
                var uri = Android.Net.Uri.Parse("https://www.tiktok.com/@arargamesstudio");
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
#else
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.tiktok.com/@arargamesstudio") { UseShellExecute = true });
#endif
            }
        }

        public override void ExitScreen()
        {
            base.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }


    }
}
