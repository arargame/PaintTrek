using System.IO;
using Microsoft.Xna.Framework;
using PaintTrek.Shared.Localization;

namespace PaintTrek
{
    /// <summary>
    /// MonoGame TitleContainer kullanarak yerelleştirme dosyalarını açan sağlayıcı.
    /// </summary>
    public class TitleContainerStreamProvider : IStreamProvider
    {
        public Stream Open(string path)
        {
            return TitleContainer.OpenStream(path);
        }
    }
}
