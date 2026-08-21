using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    /// <summary>
    /// Dil seçim ekranı. Dikey kaydırma desteği barındırır ve her dil adını kendi font varyantıyla çizer.
    /// </summary>
    class LanguageScreen : MenuScreen
    {
        private Type callerType;
        private readonly Dictionary<ScriptFamily, SpriteFont> familyFonts = new();
        private int scrollOffset = 0;
        private const int MaxVisibleEntries = 10;
        private const int EntryHeight = 48;
        private const int EntryGap = 14;
        private const int HorizontalPadding = 32;
        private Texture2D pixel;
        private BackButton backButton;
        private int languageButtonWidth;

        // Dil geçişi sırasında gösterilecek kısa bekleme ekranı
        private LanguageCode? pendingLanguage;
        private double switchTimer = 0;
        private const double SwitchDuration = 0.35;

        public LanguageScreen(Type callerType = null)
        {
            this.callerType = callerType;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            screenTitle = "Language Screen";
            Globals.Window.Title = screenTitle;
            backButton = new BackButton(Loc.T(LocKeys.Menu.Back), this, true);
            RegisterClickableArea(backButton.clickableArea);
        }

        public override void Load()
        {
            base.Load();

            // Pixel texture yükle
            pixel = Globals.Content.Load<Texture2D>("Textures/singlePixel");

            // LanguageAwareContentManager kullanarak ham font varyantlarını yükle
            var content = Globals.Content as LanguageAwareContentManager;

            foreach (var info in Languages.Selectable)
            {
                if (familyFonts.ContainsKey(info.Family)) continue;

                SpriteFont font = null;
                try
                {
                    string asset = "Fonts/MenuFont_2" + info.FontSuffix;
                    if (content != null)
                        font = content.LoadRaw<SpriteFont>(asset);
                    else
                        font = Globals.Content.Load<SpriteFont>(asset);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[LanguageScreen] Font yüklenemedi '{info.Family}': {ex.Message}");
                    font = Globals.MenuFont;
                }

                if (font != null)
                {
                    try { font.DefaultCharacter = '?'; } catch { }
                    familyFonts[info.Family] = font;
                }
            }

            LoadMenuEntries();
            CalculateButtonWidth();

            // Seçili dili başlangıçta listenin odağına al
            for (int i = 0; i < Languages.Selectable.Length; i++)
            {
                if (Languages.Selectable[i].Language == Loc.Current)
                {
                    selectedEntry = i;
                    if (selectedEntry >= MaxVisibleEntries)
                    {
                        scrollOffset = selectedEntry - MaxVisibleEntries / 2;
                        scrollOffset = Math.Max(0, Math.Min(scrollOffset, MenuEntries.Count - MaxVisibleEntries));
                    }
                    break;
                }
            }
        }

        private void LoadMenuEntries()
        {
            if (MenuEntries.Count > 0)
                MenuEntries.Clear();

            for (int i = 0; i < Languages.Selectable.Length; i++)
            {
                var info = Languages.Selectable[i];
                string label = info.DisplayName;

                var entry = new MenuEntry(label, true, i);
                AddEntry(entry);
            }
        }

        private void CalculateButtonWidth()
        {
            float widestLabel = 0;
            foreach (var info in Languages.Selectable)
            {
                SpriteFont font = Globals.MenuFont;
                if (familyFonts.TryGetValue(info.Family, out var familyFont)) font = familyFont;
                widestLabel = Math.Max(widestLabel, font.MeasureString(info.DisplayName).X);
            }

            // All buttons use the longest language label plus equal in-button padding.
            // This keeps the list content-driven instead of tying it to screen percentage.
            languageButtonWidth = (int)Math.Ceiling(widestLabel + HorizontalPadding * 2);
            languageButtonWidth = Math.Min(languageButtonWidth, (int)Globals.GameSize.X - 160);
        }

        public override void Update()
        {
            // Dil geçiş animasyonu ve yüklemesi
            if (pendingLanguage.HasValue)
            {
                switchTimer -= Globals.GameTime.ElapsedGameTime.TotalSeconds;
                if (switchTimer <= 0)
                {
                    var target = pendingLanguage.Value;
                    pendingLanguage = null;

                    // Dili kaydet ve ayarla
                    Loc.SetLanguage(target);
                    GameSettings.Instance.UpdateSettings(language: Languages.CodeOf(target));

                    // Fontları temizleyip yeniden yükle
                    Loader.Load();

                    // Üst ekrana dön
                    ExitScreen();
                    ScreenManager.AddScreen(new OptionsScreen());
                }
                return;
            }

            backButton?.Update();

            // Seçili kaydırma görünümünü sınırla
            if (selectedEntry < scrollOffset)
            {
                scrollOffset = selectedEntry;
            }
            else if (selectedEntry >= scrollOffset + MaxVisibleEntries)
            {
                scrollOffset = selectedEntry - MaxVisibleEntries + 1;
            }

            // ClickableArea konumlarını ve görünürlüklerini scrollOffset'e göre güncelle
            float startY = 160;
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                var entry = MenuEntries[i];
                if (i >= scrollOffset && i < scrollOffset + MaxVisibleEntries)
                {
                    var info = Languages.Selectable[i];
                    SpriteFont font = Globals.MenuFont;
                    if (familyFonts.TryGetValue(info.Family, out var f)) font = f;

                    int x = (int)(Globals.GameSize.X / 2 - languageButtonWidth / 2);
                    int y = (int)(startY + (i - scrollOffset) * (EntryHeight + EntryGap));
                    entry.clickableArea.SetRect(new Rectangle(x, y, languageButtonWidth, EntryHeight));
                }
                else
                {
                    // Görünmeyen girişlerin tıklama alanını devre dışı bırakmak için boş Rectangle atıyoruz
                    entry.clickableArea.SetRect(Rectangle.Empty);
                }
            }

            base.Update();
        }

        public override void Draw()
        {
            Globals.Graphics.GraphicsDevice.Clear(Color.Black);

            Globals.SpriteBatch.Begin();

            // Başlık çizimi (DİL / LANGUAGE)
            string titleText = Loc.T(LocKeys.Options.Language);
            Vector2 titleSize = Globals.MenuFont.MeasureString(titleText);
            Vector2 titlePos = new Vector2(Globals.GameSize.X / 2 - titleSize.X / 2, 80);
            Globals.SpriteBatch.DrawString(Globals.MenuFont, titleText, titlePos, Color.Beige);

            float startY = 160;
            int end = Math.Min(scrollOffset + MaxVisibleEntries, MenuEntries.Count);

            // Dilleri çiz
            for (int i = scrollOffset; i < end; i++)
            {
                var entry = MenuEntries[i];
                var info = Languages.Selectable[i];

                SpriteFont font = Globals.MenuFont;
                if (familyFonts.TryGetValue(info.Family, out var f)) font = f;

                string text = entry.Text;
                Rectangle rect = entry.clickableArea.GetRect();
                bool selected = i == selectedEntry || entry.clickableArea.IsOverlapped;
                Color textColor = selected ? Color.Black : (info.Language == Loc.Current ? Color.Yellow : Color.White);
                Color borderColor = selected ? Color.Gold : Color.White * 0.8f;
                Globals.SpriteBatch.Draw(pixel, rect, selected ? Color.White : Color.Black * 0.85f);
                Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), borderColor);
                Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), borderColor);
                Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 2, rect.Height), borderColor);
                Globals.SpriteBatch.Draw(pixel, new Rectangle(rect.Right - 2, rect.Y, 2, rect.Height), borderColor);
                Vector2 textSize = font.MeasureString(text);
                Vector2 pos = new Vector2(rect.Center.X - textSize.X / 2, rect.Center.Y - textSize.Y / 2);
                Globals.SpriteBatch.DrawString(font, text, pos, textColor);
            }

            // Scrollbar (Dikey Kaydırma Çubuğu) Çizimi
            if (MenuEntries.Count > MaxVisibleEntries && pixel != null)
            {
                float barHeight = MaxVisibleEntries * (EntryHeight + EntryGap) - EntryGap;
                float barWidth = 6;
                float barX = Globals.GameSize.X - 50;
                float barY = startY;

                // Scrollbar arka plan
                Globals.SpriteBatch.Draw(pixel, new Rectangle((int)barX, (int)barY, (int)barWidth, (int)barHeight), Color.DimGray * 0.3f);

                // Scrollbar tutamak (Thumb)
                float thumbHeight = barHeight * ((float)MaxVisibleEntries / MenuEntries.Count);
                float thumbY = barY + (barHeight * ((float)scrollOffset / MenuEntries.Count));

                Globals.SpriteBatch.Draw(pixel, new Rectangle((int)barX, (int)thumbY, (int)barWidth, (int)thumbHeight), Color.CornflowerBlue);
            }

            // Dil geçişi sırasında siyah örtü çiz
            if (pendingLanguage.HasValue && pixel != null)
            {
                Globals.SpriteBatch.Draw(pixel, new Rectangle(0, 0, (int)Globals.GameSize.X, (int)Globals.GameSize.Y), Color.Black * 0.9f);
                string dots = "...";
                Vector2 dotsSize = Globals.MenuFont.MeasureString(dots);
                Globals.SpriteBatch.DrawString(Globals.MenuFont, dots, new Vector2(Globals.GameSize.X / 2 - dotsSize.X / 2, Globals.GameSize.Y / 2 - dotsSize.Y / 2), Color.White);
            }

            Globals.SpriteBatch.End();

            backButton?.Draw();
        }

        public override void HandleInput()
        {
            if (pendingLanguage.HasValue) return;

            // Fare tekerleği (Mouse Wheel) scroll desteği
            int scroll = inputState.currentMouseState.ScrollWheelValue - inputState.lastMouseState.ScrollWheelValue;
            if (scroll > 0)
            {
                selectedEntry = Math.Max(0, selectedEntry - 1);
            }
            else if (scroll < 0)
            {
                selectedEntry = Math.Min(MenuEntries.Count - 1, selectedEntry + 1);
            }

            base.HandleInput();
        }

        public override void MenuSelect(int selectedEntry)
        {
            if (selectedEntry < 0 || selectedEntry >= Languages.Selectable.Length) return;

            var picked = Languages.Selectable[selectedEntry].Language;
            if (picked == Loc.Current) return;

            pendingLanguage = picked;
            switchTimer = SwitchDuration;
        }

        public override void MenuCancel(int selectedEntry)
        {
            base.MenuCancel(selectedEntry);
            ExitScreen();
            ScreenManager.AddScreen(new OptionsScreen());
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            backButton?.Dispose();
        }
    }
}
