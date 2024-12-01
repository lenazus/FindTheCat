using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FindDeCat;
using FindDeCat.Services;
//using FindDeCat.Services.Translations;

namespace FindDeCat.Services
{
    public interface ITranslations
    {
        Task LoadJsonAsync(string filePath);

        KeyValuePair<string, string> GetRandomTranslationKey(string languageCode);

        KeyValuePair<string, string> GetNextTranslationKeyByQueue(string languageCode, string category);

        public string GetEmoji(string key);
    }
}