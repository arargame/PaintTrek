using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    /// <summary>
    /// Font isteklerini aktif dilin yazı ailesine yönlendiren ve dil değişiminde
    /// font önbelleğini akıllıca temizleyen ContentManager.
    /// </summary>
    public class LanguageAwareContentManager : ContentManager
    {
        private static readonly HashSet<string> FamilyAwareFonts =
            new(StringComparer.Ordinal)
        {
            "Fonts/GameFont_1",
            "Fonts/GameFont_2",
            "Fonts/MenuFont_1",
            "Fonts/MenuFont_2",
            "Fonts/demoFont",
        };

        private LanguageCode _lastLanguage = LanguageCode.English;

        public LanguageAwareContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            _lastLanguage = Loc.Current;
        }

        /// <summary>
        /// Sonek yönlendirmesini atlayarak ham varlığı yükler.
        /// </summary>
        public T LoadRaw<T>(string assetName)
        {
            return base.Load<T>(assetName);
        }

        public override T Load<T>(string assetName)
        {
            if (Loc.Current != _lastLanguage)
            {
                ClearFontCache();
                _lastLanguage = Loc.Current;
            }

            T result;
            if (typeof(T) == typeof(SpriteFont) && FamilyAwareFonts.Contains(assetName))
            {
                string suffix = Languages.FontSuffixOf(Loc.Current);
                if (suffix.Length > 0) 
                    result = base.Load<T>(assetName + suffix);
                else
                    result = base.Load<T>(assetName);
            }
            else
            {
                result = base.Load<T>(assetName);
            }

            // SpriteFont için DefaultCharacter'ı güvenli bir şekilde ayarla
            if (result is SpriteFont sf)
            {
                try { sf.DefaultCharacter = '?'; } catch { }
            }

            return result;
        }

        /// <summary>
        /// Sadece font dosyalarını önbellekten silerek bellek sızıntısını ve
        /// tüm dokuların gereksiz yere tekrar yüklenmesini önler.
        /// </summary>
        private void ClearFontCache()
        {
            try
            {
                var field = typeof(ContentManager).GetField("loadedAssets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var loadedAssets = field.GetValue(this) as System.Collections.IDictionary;
                    if (loadedAssets != null)
                    {
                        var keysToRemove = new List<string>();
                        foreach (var key in loadedAssets.Keys)
                        {
                            if (key is string s && (s.StartsWith("Fonts/") || s.Contains("Font")))
                            {
                                keysToRemove.Add(s);
                            }
                        }
                        foreach (var key in keysToRemove)
                        {
                            loadedAssets.Remove(key);
                        }
                        System.Diagnostics.Debug.WriteLine($"[LanguageAwareContentManager] Font önbelleği temizlendi ({keysToRemove.Count} font).");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LanguageAwareContentManager] Önbellek temizleme hatasi: {ex.Message}");
            }
        }
    }
}
