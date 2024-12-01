using FindDeCat;
using FindDeCat.Configuration;
using FindDeCat.Helpers;
using FindDeCat.Services;
using Newtonsoft.Json;

namespace FindDeCat.Helpers
{
    /// <summary>
    /// The current code for GetCachedLanguage is synchronous and interacts with Preferences, 
    /// which operates in-memory and doesn’t involve disk or network I/O, making it a fast, lightweight operation. 
    /// Therefore, changing GetCachedLanguage to async wouldn't offer any real benefit because:
    ///  Preferences API is Synchronous: The Preferences API in Xamarin/Microsoft.Maui is designed for small key-value storage, and it works synchronously.This is unlike file or database operations that would typically require async handling.
    ///  No I/O-bound Work: Since there’s no long-running operation, the "async all the way" principle doesn’t apply here.
    ///  The class doesn't maintain any instance-specific state. In your example, this is true because CacheService only works with external state (via Preferences).
    /// </summary>
    public static class CacheService
    {
        private const string LanguagePreferenceKey = "languagePreference";

        public static string GetCachedLanguage()
        {
            return Preferences.Get(LanguagePreferenceKey, AppConfiguration.Pickers.LANGUAGES.First().Value);
        }

        public static void SaveLanguagePreference(string languageCode)
        {
            Preferences.Set(LanguagePreferenceKey, languageCode);
        }
    }
}