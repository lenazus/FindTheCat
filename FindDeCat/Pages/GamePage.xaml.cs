using FindDeCat.Configuration;
using FindDeCat.Helpers;
using FindDeCat.Models;
using FindDeCat.Services;
using Serilog;

namespace FindDeCat.Pages
{
    /// <summary>
    /// Represents the main game interface for the "Find the Cat" application. 
    /// This page is responsible for managing user interactions, game logic, and UI updates.
    /// 
    /// Key Responsibilities:
    /// - Initializes and manages pickers for language, category, and delay options.
    /// - Handles interactions with game elements
    /// 
    /// Dependencies:
    /// - <see cref="IPickerHandler"/> for managing picker data and selection changes.
    /// - <see cref="IGameUIService"/> for updating UI elements based on user interactions.
    /// - <see cref="IAutoClickService"/> for simulating user clicks.
    /// 
    /// Notes:
    /// - Implements async patterns to ensure smooth user experience.
    /// - Contains event handlers for UI interactions, dynamically wired to UI elements at runtime.
    /// 
    /// The GamePage class is partial to integrate XAML and code-behind logic seamlessly. It’s a key feature of the Xamarin and .NET MAUI frameworks, enabling clean separation of UI definition and behavior, as well as allowing for future extensibility.
    /// </summary>
    public partial class GamePage : ContentPage
    {
        #region Fields
        private readonly IPickerHandler _pickerHandler;
        private readonly IGameUIService _gameUIService;
        private readonly IAutoClickService _autoClickService;
        private readonly GameState _gameState;
        private readonly Image[] _rectangles;
        #endregion

        #region Constructor
        public GamePage(
            IPickerHandler pickerHandler,
            GameState gameState,
            IGameUIService gameUIService,
            IAutoClickService autoClickService)
        {
            _pickerHandler = pickerHandler;
            _gameUIService = gameUIService;
            _autoClickService = autoClickService;
            _gameState = gameState;

            InitializeComponent();
            InitializePickers();

            _rectangles = new[] { Rect1, Rect2, Rect3, Rect4 };
            _gameState.AwardChance = AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_CAT;
            ChanceToGetCatGifButton.Text = AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_EMOJI;
        }
        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Called when the Page is first displayed, When returning to a page, When the app resumes
        /// </summary>
        protected override async void OnAppearing()
        {
            LoadCachedLanguage();
            base.OnAppearing();
            await InitializeGameAsync();
        }

        public async Task InitializeGameAsync()
        {
            await _gameUIService.InitializeGame(_gameState, _rectangles, GameLabel);
        }

        /// <summary>
        /// Called when App Goes into the Background, navigating from page to page
        /// </summary>
        protected override void OnDisappearing()
        {
            _autoClickService.PauseAutoClick();
        }
        
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            OnLayoutChanged(this, EventArgs.Empty);
        }

        public void OnLayoutChanged(object sender, EventArgs e)
        {
            LayoutHelper.UpdateLayout(Width, Height, _rectangles, GameLabel);
        }

        private void LoadCachedLanguage()
        {
            var cachedLanguage = CacheService.GetCachedLanguage();
            if (cachedLanguage != null)
            {
                _gameState.SelectedLanguageCode = cachedLanguage;
            }
        }

        #endregion

        #region UI Handlers
        public void OnCatGifChanceClick(object sender, EventArgs e)
        {
            _gameState.AwardChance = (_gameState.AwardChance == AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_CAT
                ? AppConfiguration.GameSettings.HIGH_CHANCE_TO_GET_CAT
                : AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_CAT);

            if (sender is Button button)
            {
                button.Text = _gameState.AwardChance == AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_CAT
                    ? AppConfiguration.GameSettings.LOW_CHANCE_TO_GET_EMOJI
                    : AppConfiguration.GameSettings.HIGH_CHANCE_TO_GET_EMOJI;
            }
        }

        /*public void OnAutoClickButtonClicked(object sender, EventArgs e)
        {
            _autoClickService.ToggleAutoClick(AutoClickButton, _rectangles, SimulateRectangleTap);
        }*/

        public void OnAutoClickButtonClicked(object sender, EventArgs e)
        {
            var buttonUpdater = new MauiButtonTextUpdater(AutoClickButton);
            _autoClickService.ToggleAutoClick(buttonUpdater, _rectangles, SimulateRectangleTap);
        }

        private void SimulateRectangleTap(Image rectangle)
        {
            OnRectangleTapped(rectangle, EventArgs.Empty);
        }

        private async void OnRectangleTapped(object sender, EventArgs e)
        {
            if (!_gameState.IsBusy && sender is Image tappedRectangle)
            {
                await _gameUIService.HandleRectangleTappedAsync(tappedRectangle, GameLabel, _rectangles, _gameState);
            }
        }
        #endregion

        #region Pickers
        public void InitializePickers()
        {
            _pickerHandler.InitializeLanguagePicker(LanguagePicker);
            _pickerHandler.InitializeCategoryPicker(CategoryPicker);
            _pickerHandler.InitializeDelayPicker(DelayPicker);
        }


        public virtual Picker GetLanguagePicker() => LanguagePicker;
        public virtual Picker GetCategoryPicker() => CategoryPicker;
        public virtual Picker GetDelayPicker() => DelayPicker;

        #endregion

        #region Error Handling
        private async Task DisplayErrorAndCloseApp(string error)
        {
            Log.Error(error);
            await Application.Current.MainPage.DisplayAlert("Error", AppConfiguration.Messages.GENERAL_ERROR_MESSAGE, "OK");
            //ApplicationCloser.CloseApplication();
        }
        #endregion
    }
}