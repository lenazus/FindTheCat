namespace FindDeCat.Configuration
{
    /// <summary>
    /// Centralized application configuration.
    /// </summary>
    public static class AppConfiguration
    {
        public static class GameSettings
        {
            public const string BACKGROUND_COLOR = "#FFDAB9";
            public const int DISPLAY_WORD_FONT_SIZE  = 48;

            public const string PLAY = "▶️";
            public const string PAUSE = "⏸️";

            public const string CAT_WORD = "Meow!";
            public const string CAT_SOUND = "meow1.m4a";
            public const string CAT_IMAGE = "meow1.gif";

            public const int DEFAULT_CAT_GIF_DISPLAY_DURATION = 4000;

            public const int HIGH_CHANCE_TO_GET_CAT = 4; //1 to 4
            public const int LOW_CHANCE_TO_GET_CAT = 100; //1 to 100

            public const string HIGH_CHANCE_TO_GET_EMOJI = "😻"; 
            public const string LOW_CHANCE_TO_GET_EMOJI = "😿"; 

            public const int DEFAULT_DELAY_SECONDS = 1;

            public const int NUM_OF_CAT_GIFS_IN_RESOURCES = 23;
            public const int NUM_OF_CAT_SOUNDS_IN_RESOURCES = 15;

            //Use readonly for immutable values initialized at runtime or via a constructor.
            public static readonly string[] RECTANGLE_IMAGES = { "rectangle21.png", "rectangle22.png", "rectangle23.png", "rectangle24.png" };
        }

        //are runtime objects and must be static readonly:
        public static class Pickers
        {
            public static readonly Dictionary<string, string> CATEGORIES = new()
            {
                { "👜", "translations.json" },
                { "🍎", "fruits_and_vegs.json" },
                { "🚗", "transport.json" },
                { "🌸", "colors.json" },
                { "💯", "numbers.json" },
            };

            public static readonly string[] CATEGORIES_TO_NOT_RANDOMISE = { "colors.json", "numbers.json" };

            public static readonly Dictionary<string, string> LANGUAGES = new()
            {
                { "🇪🇸", "ES" },
                { "🇬🇧", "EN" },
                { "🇸🇦", "AR" },
                { "🇷🇺", "RU" },
                { "🇩🇪", "DE" },
                { "🇫🇷", "FR" }
            };

            public static readonly List<string> DELAY_PICKER_OPTIONS =
            Enumerable.Range(1, 10)
              .Select(i => $"{i} ⏱️")
              .ToList();
        }

        public static class WindowConstants
        {
            public const double TYPICAL_MOBILE_WIDTH = 360;
            public const double TYPICAL_MOBILE_HEIGHT = 640;
        }

        public static class Messages
        {
            public const string GENERAL_ERROR_MESSAGE = "Meow, an error occurred. Please try to restart the app.";
            public const string SERVICE_PROVIDER_NOT_INITIALIZED = "Service provider is not initialized.";
        }

        public static class ApiEndpoints
        {
            private static readonly string BASE_URL = "https://api.FindDeCat.com/";
            public static string GetCatImageUrl => $"{BASE_URL}get-cat-image";
        }
    }
}
