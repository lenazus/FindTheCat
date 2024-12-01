namespace FindDeCat.Services
{
    public interface IPickerHandler
    {
        // Generic methods
        void LoadPicker<T>(Picker picker, Dictionary<string, T> itemsSource, string pickerType);
        void HandlePickerSelectionChanged<T>(
            Picker picker,
            Dictionary<string, T> itemsSource,
            Action<T> onValidSelection,
            string errorMessage);

        void InitializeLanguagePicker(Picker picker);
        void InitializeCategoryPicker(Picker picker);
        void InitializeDelayPicker(Picker picker);
    }
}