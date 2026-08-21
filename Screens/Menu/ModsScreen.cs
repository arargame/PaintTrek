using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    /// <summary>Desktop's free, two-panel counterpart of the mobile Mods screen.</summary>
    class ModsScreen : MenuScreen
    {
        Texture2D endlessBanner, ufoBanner, bossesBanner, pixel;
        TextButton playButton;
        BackButton backButton;
        Rectangle rightPanel, bannerRect, detailRect;
        int lastSelected = -1;
        string title, description, rules;

        public ModsScreen()
        {
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.EndlessMode), true, 0));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.UfoInvasion), true, 1));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.AgainstAllBosses), true, 2));
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = Loc.T(LocKeys.Menu.Mods);
            Globals.Window.Title = screenTitle;
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");
            endlessBanner = Globals.Content.Load<Texture2D>("Textures/EndlessBanner");
            ufoBanner = Globals.Content.Load<Texture2D>("Textures/UfoBanner");
            bossesBanner = Globals.Content.Load<Texture2D>("Textures/AgainstAllBossesBanner");

            rightPanel = new Rectangle((int)(Globals.GameSize.X * .39f), 34, (int)(Globals.GameSize.X * .57f), (int)(Globals.GameSize.Y * .82f));
            bannerRect = new Rectangle(rightPanel.X + 24, rightPanel.Y + 24, rightPanel.Width - 48, (int)(rightPanel.Height * .31f));
            detailRect = new Rectangle(rightPanel.X + 38, bannerRect.Bottom + 28, rightPanel.Width - 76, rightPanel.Bottom - bannerRect.Bottom - 80);

            playButton = new TextButton(Loc.T(LocKeys.Menu.NewGame), Vector2.Zero) { backgroundColor = Color.DarkRed * .9f, hasBackground = true };
            RegisterClickableArea(playButton.clickableArea);
            backButton = new BackButton(Loc.T(LocKeys.Menu.Back), this, true);
            RegisterClickableArea(backButton.clickableArea);
            UpdateDetails();
        }

        public override void Update()
        {
            base.Update();
            backButton.Update();
            playButton.Update();
            if (playButton.IsClicked) StartSelectedGame();
            if (lastSelected != SelectedEntry) { lastSelected = SelectedEntry; UpdateDetails(); }
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();
            Globals.SpriteBatch.Begin();
            Globals.SpriteBatch.Draw(pixel, rightPanel, Color.Black * .72f);
            DrawBorder(rightPanel, Color.DimGray);
            Texture2D banner = SelectedEntry == 0 ? endlessBanner : SelectedEntry == 1 ? ufoBanner : bossesBanner;
            Globals.SpriteBatch.Draw(banner, bannerRect, Color.White);
            DrawBorder(bannerRect, Color.Gold * .75f);
            Globals.SpriteBatch.DrawString(Globals.MenuFont, title, new Vector2(detailRect.X, detailRect.Y), Color.Beige);
            float y = detailRect.Y + Globals.MenuFont.MeasureString(title).Y + 28;
            Globals.SpriteBatch.DrawString(Globals.GameFont, description, new Vector2(detailRect.X, y), Color.White * .9f);
            y += Globals.GameFont.MeasureString(description).Y + 26;
            Globals.SpriteBatch.DrawString(Globals.GameFont, rules, new Vector2(detailRect.X + 24, y), Color.Gold);
            Globals.SpriteBatch.End();
            playButton.Draw();
            backButton.Draw();
        }

        public override void MenuSelect(int selectedEntry)
        {
            this.selectedEntry = selectedEntry;
            UpdateDetails();
        }

        void UpdateDetails()
        {
            title = SelectedEntry switch { 0 => Loc.T(LocKeys.Mods.EndlessMode), 1 => Loc.T(LocKeys.Mods.UfoInvasion), _ => Loc.T(LocKeys.Mods.AgainstAllBosses) };
            if (SelectedEntry == 0)
            {
                description = Loc.T(LocKeys.Mods.EndlessDesc);
                rules = Loc.T(LocKeys.Mods.EndlessRules);
            }
            else if (SelectedEntry == 1)
            {
                description = Loc.T(LocKeys.Mods.UfoDesc);
                rules = Loc.T(LocKeys.Mods.UfoRules);
            }
            else
            {
                description = Loc.T(LocKeys.Mods.BossesDesc);
                rules = Loc.T(LocKeys.Mods.BossesRules);
            }
            Vector2 playSize = Globals.GameFont.MeasureString(Loc.T(LocKeys.Menu.NewGame));
            playButton.SetPosition(new Vector2(rightPanel.Right - playSize.X - 78, detailRect.Bottom - playSize.Y - 34));
        }

        void StartSelectedGame()
        {
            Globals.CurrentMode = SelectedEntry switch { 0 => GameMode.Endless, 1 => GameMode.UfoInvasion, _ => GameMode.AgainstAllBosses };
            Level.LevelCounter = 1;
            Level.Score = 0;
            ExitScreen();
            ScreenManager.AddScreen(GameBoard.CreateNewGame());
        }

        void DrawBorder(Rectangle rect, Color color)
        {
            const int t = 2;
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, t), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - t, rect.Width, t), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, t, rect.Height), color);
            Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.Right - t, rect.Y, t, rect.Height), color);
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }
    }
}
