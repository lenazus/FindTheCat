using System.Reflection;
using Newtonsoft.Json.Linq;
using Serilog;

namespace FindDeCat.Services
{
    public class Translations : ITranslations
    {
        private Dictionary<string, Dictionary<string, string>> _allTranslations;
        private Dictionary<string, string> _emojis; // Store emojis by key
        private readonly Dictionary<string, int> _categoryQueues = new();
        private Random _random = new();
        private int? _previousIndex = null; // For avoiding repetition in getting random translation

        public Translations()
        {
            _allTranslations = new Dictionary<string, Dictionary<string, string>>();
            _emojis = new Dictionary<string, string>();
        }

        #region Public Methods

        // Asynchronously loads JSON translation resource and processes it
        public async Task LoadJsonAsync(string resourceName)
        {
            _allTranslations.Clear();
            _emojis.Clear();

            var fullResourceName = $"{Assembly.GetExecutingAssembly().GetName().Name}/Resources/{resourceName}";
            try
            {
                using (Stream stream = GetResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Log.Error($"Resource not found: {resourceName}");
                        return;
                    }

                    var jsonContent = await ReadStreamAsync(stream);
                    ProcessJsonContent(jsonContent);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error loading resource: {resourceName}");
            }
        }

        // Retrieves a random translation key, avoiding repetition
        public KeyValuePair<string, string> GetRandomTranslationKey(string languageCode)
        {
            var translations = GetAvailableTranslations(languageCode);

            int newIndex;
            do
            {
                newIndex = _random.Next(translations.Count);
            } while (translations.Count > 1 && _previousIndex.HasValue && newIndex == _previousIndex.Value);

            _previousIndex = newIndex;

            return translations[newIndex];
        }

        // Retrieves the next translation key in a category queue
        public KeyValuePair<string, string> GetNextTranslationKeyByQueue(string languageCode, string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category cannot be null or empty.", nameof(category));
            }

            var translations = GetAvailableTranslations(languageCode);

            if (!_categoryQueues.ContainsKey(category))
            {
                _categoryQueues[category] = 0; // Initialize queue position
            }

            var currentIndex = _categoryQueues[category];
            var selectedTranslation = translations[currentIndex];

            _categoryQueues[category] = (currentIndex + 1) % translations.Count;

            return selectedTranslation;
        }

        // Retrieves emoji by key
        public string GetEmoji(string key)
        {
            return _emojis.TryGetValue(key, out var emoji) ? emoji : string.Empty;
        }

        #endregion

        #region Private Methods

        // Helper method to retrieve available translations for a specific language code
        private List<KeyValuePair<string, string>> GetAvailableTranslations(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                throw new ArgumentException("Language code cannot be null or empty.", nameof(languageCode));
            }

            if (!_allTranslations.ContainsKey(languageCode))
            {
                throw new KeyNotFoundException($"Translations for language code '{languageCode}' not found.");
            }

            var translations = _allTranslations[languageCode].ToList();

            if (translations.Count == 0)
            {
                throw new InvalidOperationException("No translations available for the specified language.");
            }

            return translations;
        }

        // Processes the JSON content from the resource file
        private void ProcessJsonContent(string jsonContent)
        {
            var translationList = JArray.Parse(jsonContent);

            foreach (var item in translationList)
            {
                var key = item["Key"].ToString();
                var emoji = item["Emoji"].ToString();
                var translations = item["Translations"] as JObject;

                _emojis[key] = emoji; // Store the emoji for the key

                foreach (var language in translations)
                {
                    string langCode = language.Key.ToUpperInvariant();
                    if (!_allTranslations.ContainsKey(langCode))
                    {
                        _allTranslations[langCode] = new Dictionary<string, string>();
                    }
                    _allTranslations[langCode][key] = language.Value.ToString();
                }
            }
        }

        // Helper method to read stream asynchronously
        private async Task<string> ReadStreamAsync(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        // Helper method to retrieve the resource stream
        private Stream GetResourceStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }

        #endregion
    }
}