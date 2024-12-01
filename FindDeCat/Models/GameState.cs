using FindDeCat.Configuration;

namespace FindDeCat.Models
{
    public class GameState
    {
        public List<GameCard> GameCards { get; set; }  // maybe not used?
        public string SelectedLanguageCode { get; set; }
        public string LastSelectedLanguageCode { get; set; }
        public int CatRectangleIndex { get; set; }
        public bool IsSoundPlaying { get; set; }
        public bool IsBusy { get; set; }
        public int AwardChance { get; set; }
        public int DelayOption { get; set; }
        public string CategoryFile { get; set; }
        public GameState()
        {
            SelectedLanguageCode = AppConfiguration.Pickers.LANGUAGES.First().Value;
            LastSelectedLanguageCode = string.Empty;
            IsSoundPlaying = false;
            IsBusy = false;
            AwardChance = AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_CAT;
        }
    }
}