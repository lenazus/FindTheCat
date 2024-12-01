using FindDeCat.Configuration;

namespace FindDeCat.Services
{
    public class AutoClickService : IAutoClickService
    {
        #region Fields

        private readonly Random _random = new();
        private CancellationTokenSource _autoClickCancellationTokenSource;
        private bool _isAutoClickEnabled;

        #endregion

        #region Constructor

        public AutoClickService()
        {
            _isAutoClickEnabled = false;
        }

        #endregion

        #region Properties
        public bool IsAutoClickEnabled => _isAutoClickEnabled;

        #endregion

        #region Public Methods

        public void ToggleAutoClick(IButtonTextUpdater autoClickButton, Image[] rectangles, Action<Image> performClickAction)
        {
            _isAutoClickEnabled = !_isAutoClickEnabled;

            if (_isAutoClickEnabled)
            {
                autoClickButton.Text = AppConfiguration.GameSettings.PAUSE;
                StartAutoClick(rectangles, performClickAction);
            }
            else
            {
                autoClickButton.Text = AppConfiguration.GameSettings.PLAY;
                StopAutoClick();
            }
        }

        public void ToggleAutoClick(Button autoClickButton, Image[] rectangles, Action<Image> performClickAction)
        {
            var buttonUpdater = new MauiButtonTextUpdater(autoClickButton);
            ToggleAutoClick(buttonUpdater, rectangles, performClickAction);
        }

        /*public void OnAutoClickButtonClicked(object sender, EventArgs e)
        {
            _autoClickService.ToggleAutoClick(AutoClickButton, _rectangles, SimulateRectangleTap);
        }*/

        public void PauseAutoClick()
        {
            if (_isAutoClickEnabled)
            {
                StopAutoClick();
            }
        }

        #endregion

        #region Private Methods

        private void StartAutoClick(Image[] rectangles, Action<Image> performClickAction)
        {
            _autoClickCancellationTokenSource = new CancellationTokenSource();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (_isAutoClickEnabled)
                {
                    PerformRandomRectangleClick(rectangles, performClickAction);
                    return true; // Keep the timer running
                }
                return false; // Stop the timer if auto-click is disabled
            });
        }

        private void StopAutoClick()
        {
            _autoClickCancellationTokenSource?.Cancel();
            _autoClickCancellationTokenSource = null;
        }

        private void PerformRandomRectangleClick(Image[] rectangles, Action<Image> performClickAction)
        {
            int randomIndex = _random.Next(rectangles.Length);
            performClickAction(rectangles[randomIndex]); // Simulate a click
        }

        #endregion
    }
}