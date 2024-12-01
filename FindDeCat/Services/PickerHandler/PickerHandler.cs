using FindDeCat.Configuration;
using FindDeCat.Helpers;
using FindDeCat.Models;
using FindDeCat.Services;

namespace FindDeCat.Services
{
    public class PickerHandler : IPickerHandler
    {
        private readonly GameState _gameState;
        private readonly ITranslations _translationsService;

        public PickerHandler(GameState gameState, ITranslations translationsService)
        {
            _gameState = gameState;
            _translationsService = translationsService;
        }
        public void LoadPicker<T>(Picker picker, Dictionary<string, T> itemsSource, string pickerType)
        {
            if (itemsSource != null && itemsSource.Any())
            {
                picker.Items.Clear();

                foreach (var key in itemsSource.Keys)
                {
                    picker.Items.Add(key);
                }

                picker.SelectedItem = itemsSource.Keys.First();
            }
            else
            {
                DisplayAlert($"Configuration not found for {pickerType} picker.");
            }
        }

        public void HandlePickerSelectionChanged<T>(
            Picker picker,
            Dictionary<string, T> itemsSource,
            Action<T> onValidSelection,
            string errorMessage)
        {
            if (picker != null && picker.SelectedIndex != -1 && picker.SelectedItem != null)
            {
                string selectedKey = picker.Items[picker.SelectedIndex];

                if (itemsSource.TryGetValue(selectedKey, out T selectedValue))
                {
                    onValidSelection?.Invoke(selectedValue);
                }
                else
                {
                    DisplayAlert(errorMessage);
                }
            }
        }

        public void InitializeLanguagePicker(Picker picker)
        {
            var languageOptions = AppConfiguration.Pickers.LANGUAGES;
            LoadPicker(picker, languageOptions, "Language");
            //picker.SelectedIndex = _cacheService.GetCachedLanguage(); need to find the index

            picker.SelectedIndexChanged += (sender, e) =>
            {
                HandlePickerSelectionChanged(
                    picker,
                    languageOptions,
                    selectedValue =>
                    {
                        _gameState.SelectedLanguageCode = selectedValue;
                        CacheService.SaveLanguagePreference(selectedValue);
                    },
                    "Selected flag has no associated language code."
                );
            };

            // Preselect cached language if available
            var cachedLanguage = CacheService.GetCachedLanguage();
            if (!string.IsNullOrEmpty(cachedLanguage) && languageOptions.ContainsKey(cachedLanguage))
            {
                picker.SelectedItem = cachedLanguage;
                _gameState.SelectedLanguageCode = cachedLanguage;
            }
        }

        public void InitializeCategoryPicker(Picker picker)
        {
            var categoryOptions = AppConfiguration.Pickers.CATEGORIES;
            LoadPicker(picker, categoryOptions, "Categories");

            picker.SelectedIndexChanged += (sender, e) =>
            {
                HandlePickerSelectionChanged(
                    picker,
                    categoryOptions,
                    selectedValue =>
                    {
                        _gameState.CategoryFile = selectedValue;
                        _translationsService.LoadJsonAsync(_gameState.CategoryFile);
                    },
                    "Selected category is invalid."
                );
            };
        }

        public void InitializeDelayPicker(Picker picker)
        {
            var delayOptions = AppConfiguration.Pickers.DELAY_PICKER_OPTIONS?.ToDictionary(option => option, option => option);
            LoadPicker(picker, delayOptions, "Delay");

            picker.SelectedIndexChanged += (sender, e) =>
            {
                HandlePickerSelectionChanged(
                    picker,
                    delayOptions,
                    selectedValue =>
                    {
                        // Extract the integer value from emoji-text (e.g., "5 ⏱️" -> 5)
                        var delayOptionString = new string(selectedValue.ToString().Where(char.IsDigit).ToArray());
                        if (int.TryParse(delayOptionString, out int delayOption))
                        {
                            _gameState.DelayOption = delayOption;
                        }
                    },
                    "Please select a valid delay option."
                );
            };
        }

        private void DisplayAlert(string message)
        {
            Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
}