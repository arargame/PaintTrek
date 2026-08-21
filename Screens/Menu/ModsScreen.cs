using Microsoft.Xna.Framework;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    /// <summary>
    /// Store-free desktop mode selection.  Desktop platform services grant every mode, so a
    /// selection launches immediately and this screen never references billing or Google APIs.
    /// </summary>
    class ModsScreen : MenuScreen
    {
        public ModsScreen()
        {
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.EndlessMode), true, 0));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.UfoInvasion), true, 1));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Mods.AgainstAllBosses), true, 2));
            AddEntry(new MenuEntry(Loc.T(LocKeys.Menu.Back), true, 3));
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = Loc.T(LocKeys.Menu.Mods);
            Globals.Window.Title = screenTitle;
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw();

            Globals.SpriteBatch.Begin();
            string title = Loc.T(LocKeys.Menu.Mods);
            Vector2 titleSize = Globals.MenuFont.MeasureString(title);
            Globals.SpriteBatch.DrawString(Globals.MenuFont,
                title, new Vector2(Globals.GameSize.X * .66f - titleSize.X / 2f, 130), Color.Beige);

            string selectedName = SelectedEntry switch
            {
                0 => Loc.T(LocKeys.Mods.EndlessMode),
                1 => Loc.T(LocKeys.Mods.UfoInvasion),
                2 => Loc.T(LocKeys.Mods.AgainstAllBosses),
                _ => Loc.T(LocKeys.Menu.Back)
            };
            Vector2 nameSize = Globals.GameFont.MeasureString(selectedName);
            Globals.SpriteBatch.DrawString(Globals.GameFont, selectedName,
                new Vector2(Globals.GameSize.X * .66f - nameSize.X / 2f, 235), Color.Gold);
            Globals.SpriteBatch.End();
        }

        public override void MenuSelect(int selectedEntry)
        {
            if (selectedEntry == 3)
            {
                MenuCancel(selectedEntry);
                return;
            }

            // The Windows/Steam policy is free access for all three modes.  Keeping the
            // platform check here makes that rule explicit without adding purchase UI.
            if (!Globals.PlatformServices.IsModeAvailable(ToModeId(selectedEntry)))
                return;

            Globals.CurrentMode = selectedEntry switch
            {
                0 => GameMode.Endless,
                1 => GameMode.UfoInvasion,
                _ => GameMode.AgainstAllBosses
            };
            Level.LevelCounter = 1;
            Level.Score = 0;
            ExitScreen();
            ScreenManager.AddScreen(GameBoard.CreateNewGame());
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen());
        }

        private static PaintTrek.Shared.Platform.GameModeId ToModeId(int index) => index switch
        {
            0 => PaintTrek.Shared.Platform.GameModeId.Endless,
            1 => PaintTrek.Shared.Platform.GameModeId.UfoInvasion,
            _ => PaintTrek.Shared.Platform.GameModeId.AgainstAllBosses
        };
    }
}
