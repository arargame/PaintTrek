using System.Threading.Tasks;
using PaintTrek.Shared.Platform;

namespace PaintTrek
{
    /// <summary>
    /// Windows Store policy: every gameplay mode is included with the game. Store-specific
    /// leaderboards can be added later without changing any screen or gameplay code.
    /// </summary>
    internal sealed class WindowsStorePlatformServices : IGamePlatformServices
    {
        public StorePlatform Platform => StorePlatform.MicrosoftStore;
        public bool HasLeaderboards => false;

        public bool IsModeAvailable(GameModeId mode) => true;

        public Task<bool> RequestModeAccessAsync(GameModeId mode) => Task.FromResult(true);

        public void SubmitScore(string leaderboardId, long score)
        {
            // Microsoft Store leaderboard integration belongs here when a provider is selected.
        }
    }
}
