using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;

namespace PaintTrek
{
    /// <summary>
    /// Opens an external HTTPS/store link through the packaged-app URI broker.
    /// Do not replace this with Process.Start: WACK marks process launching as
    /// incompatible with Windows S Mode, while Launcher preserves the same
    /// player-facing browser/store behaviour.
    /// </summary>
    internal static class ExternalUriLauncher
    {
        public static void Open(string url, string source)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                Debug.WriteLine($"[{source}] Invalid external URL: {url}");
                return;
            }

            _ = OpenAsync(uri, source);
        }

        private static async Task OpenAsync(Uri uri, string source)
        {
            try
            {
                bool launched = await Launcher.LaunchUriAsync(uri);
                if (!launched)
                    Debug.WriteLine($"[{source}] Windows did not launch: {uri}");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"[{source}] Could not open external URL: {exception.Message}");
            }
        }
    }
}
