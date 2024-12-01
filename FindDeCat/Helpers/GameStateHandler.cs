using FindDeCat;
using FindDeCat.Configuration;
using FindDeCat.Helpers;
using FindDeCat.Models;
using FindDeCat.Services;

namespace FindDeCat.Helpers
{
    public static class GameStateHandler
    {
        private static readonly Random _random = new();

        public static bool IsBusy(GameState gameState) => gameState.IsBusy;

        public static void SetBusy(GameState gameState, bool isBusy) => gameState.IsBusy = isBusy;

        public static bool IsCorrectSelection(int awardChance) => _random.Next(awardChance) == 0;

        public static int GetDelay(GameState gameState)
        {
            int delayInSeconds = gameState.DelayOption > 0
                ? gameState.DelayOption
                : AppConfiguration.GameSettings.DEFAULT_DELAY_SECONDS;

            return delayInSeconds * 1000;
        }
    }
}