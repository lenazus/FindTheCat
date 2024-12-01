using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FindDeCat.Configuration;
using FindDeCat.Helpers;
using FindDeCat.Models;
using FindDeCat.Services.Sound;
using FindDeCat.Services;  
using FindDeCat.Views;
using Microsoft.Maui;
using Serilog;

namespace FindDeCat.Services
{
    public class GameUIService : IGameUIService
    {
        private readonly ITranslations _translationsService;
        private readonly ISoundPlayerService _soundPlayerService;

        public GameUIService(ITranslations translationsService, ISoundPlayerService soundPlayerService)
        {
            _translationsService = translationsService;
            _soundPlayerService = soundPlayerService;
        }


            public async Task InitializeGame(Models.GameState gameState, Image[] rectangles, Label gameLabel)
            {
                try
                {

                _translationsService.LoadJsonAsync(gameState.CategoryFile);

                // Setup game state or translations here
                foreach (var rectangle in rectangles)
                    {
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (sender, e) =>
                        {
                            if (sender is Image tappedRectangle)
                            {
                                HandleRectangleTapped(tappedRectangle, gameLabel, rectangles, gameState);
                            }
                        };
                        rectangle.GestureRecognizers.Add(tapGestureRecognizer);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error initializing game", ex);
                }
            }

        private void HandleRectangleTapped(Image rectangle, Label gameLabel, Image[] rectangles, Models.GameState gameState)
        {
            if (!gameState.IsBusy)
            {
                HandleRectangleTappedAsync(rectangle, gameLabel, rectangles, gameState);
            }
        }

        public async Task HandleRectangleTappedAsync(Image tappedRectangle, Label gameLabel, Image[] rectangles, Models.GameState gameState)
        {
            if (GameStateHandler.IsBusy(gameState)) return;

            GameStateHandler.SetBusy(gameState, true);

            try
            {
                await DisplayCatOrWord(tappedRectangle, gameLabel, gameState);

                await Task.Delay(300); // Adding delay after operation before resetting images
                MediaHelper.ResetImages(rectangles);
                GameStateHandler.SetBusy(gameState, false);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", AppConfiguration.Messages.GENERAL_ERROR_MESSAGE, "OK");
            }
        }

        private async Task DisplayCatOrWord(Image tappedRectangle, Label gameLabel, Models.GameState gameState)
        {
            bool isCorrect = GameStateHandler.IsCorrectSelection(gameState.AwardChance);
            if (isCorrect)
                await DisplayCatAsync(tappedRectangle, gameLabel, gameState);
            else
                await DisplayRandomWordAsync(tappedRectangle, gameLabel, gameState);
        }

        private async Task DisplayCatAsync(Image tappedRectangle, Label gameLabel, GameState gameState)
        {
            try
            {
                SetAllInteractiveElementsEnabled(false);

                var randomCatImage = MediaHelper.GetRandomCatImage();
                var randomCatSound = MediaHelper.GetRandomCatSound();

                var tasks = new List<Task>();

                var wordData = new WordData
                {
                    Image = randomCatImage ?? AppConfiguration.GameSettings.CAT_IMAGE,
                    Sound = randomCatSound ?? AppConfiguration.GameSettings.CAT_SOUND,
                    Word = AppConfiguration.GameSettings.CAT_WORD,
                    Emoji = string.Empty
                };

                await PresentWordWithImageAndSoundAsync(tappedRectangle, gameLabel, gameState, wordData);
            }
            finally
            {
                SetAllInteractiveElementsEnabled(true);
            }
        }

        private void SetAllInteractiveElementsEnabled(bool isEnabled)
        {
            if (Application.Current.MainPage is not null)
            {
                foreach (var element in GetAllInteractiveElements(Application.Current.MainPage))
                {
                    element.IsEnabled = isEnabled;
                }
            }
        }

        private IEnumerable<View> GetAllInteractiveElements(Element parent)
        {
            // Include Button, Picker, Entry, or any other interactive element types
            if (parent is Button || parent is Picker || parent is Entry || parent is Switch || parent is Slider)
            {
                yield return (View)parent;
            }

            if (parent is IViewContainer<VisualElement> container)
            {
                foreach (var child in container.Children)
                {
                    foreach (var nestedElement in GetAllInteractiveElements(child))
                    {
                        yield return nestedElement;
                    }
                }
            }
            else if (parent is VisualElement visualElement)
            {
                foreach (var child in visualElement.LogicalChildren)
                {
                    foreach (var nestedElement in GetAllInteractiveElements(child))
                    {
                        yield return nestedElement;
                    }
                }
            }
        }

        private async Task DisplayRandomWordAsync(Image tappedRectangle, Label gameLabel, GameState gameState)
        {
            KeyValuePair<string, string> selectedTranslation;

            if (AppConfiguration.Pickers.CATEGORIES_TO_NOT_RANDOMISE.Contains(gameState.CategoryFile))
            {
                selectedTranslation = _translationsService.GetNextTranslationKeyByQueue(gameState.SelectedLanguageCode, gameState.CategoryFile);
            }
            else
            {
                selectedTranslation = _translationsService.GetRandomTranslationKey(gameState.SelectedLanguageCode);
            }

            var wordKey = selectedTranslation.Key;
            var wordValue = selectedTranslation.Value;

            var wordData = new WordData
            {
                Image = GameAssetHelper.GetImageFileName(wordKey),
                Sound = GameAssetHelper.GetSoundFileName(wordKey, gameState.SelectedLanguageCode),
                Word = GameAssetHelper.GetDisplayName(wordValue),
                Emoji = _translationsService.GetEmoji(wordKey)
            };

            await PresentWordWithImageAndSoundAsync(tappedRectangle, gameLabel, gameState, wordData);
        }

        private async Task PresentWordWithImageAndSoundAsync(Image tappedRectangle, Label gameLabel, GameState gameState, WordData wordData)
        {
            gameLabel.Text = string.Empty;

            // Check if the image is a GIF
            bool isGif = Path.GetExtension(wordData.Image).Equals(".gif", StringComparison.OrdinalIgnoreCase);

            if (isGif)
            {
                // Display the GIF and play the sound simultaneously
                var displayGifTask = PresentContent(tappedRectangle, wordData.Image, wordData.Emoji);
                var playSoundTask = PlaySound(wordData.Sound, gameState);

                // Wait for the GIF display duration
                //await DelayHelper.DelayAfterDisplayImage(gameState.DelayOption, AppConfiguration.GameSettings.DefaultDelaySeconds);

                var gifAndSoundDuration = Task.WhenAll(displayGifTask, playSoundTask);
                var delayTask = Task.Delay((gameState.DelayOption > 0 ? gameState.DelayOption : AppConfiguration.GameSettings.DEFAULT_DELAY_SECONDS) * 1000);
                //await Task.WhenAll(gifAndSoundDuration, delayTask);

                var timeout = Task.Delay(10000); // 10-second timeout
                await Task.WhenAny(Task.WhenAll(gifAndSoundDuration, delayTask), timeout);

                // No word display for GIF case
                gameLabel.Text = string.Empty;
            }
            else
            {
                // Normal flow for non-GIF images
                await PresentContent(tappedRectangle, wordData.Image, wordData.Emoji);
                await DelayHelper.DelayAfterDisplayImage(gameState.DelayOption, AppConfiguration.GameSettings.DEFAULT_DELAY_SECONDS);
                gameLabel.Text = wordData.Word.Replace(" ", "\n");
                await PlaySound(wordData.Sound, gameState);
            }
        }

        private async Task PresentContent(Image tappedRectangle, string image, string word)
        {
            if (AssetHelper.AssetFileExists(image))
            {
                // Check if the image is a GIF
                if (Path.GetExtension(image).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    // Navigate to the dedicated GIF display page
                    await Application.Current.MainPage.Navigation.PushModalAsync(new GifDisplayPage(image));

                    // Wait for 5 seconds
                    //await Task.Delay(2000);

                    var gifDuration = MediaHelper.GetGifDuration(image);
                    await Task.Delay(gifDuration);

                    // Pop the GIF display page
                    await Application.Current.MainPage.Navigation.PopModalAsync();

                    // Reset the game
                    var parentLayout = tappedRectangle.Parent as Microsoft.Maui.Controls.Grid;
                    MediaHelper.ResetImages(parentLayout?.Children.OfType<Image>().ToArray());
                }
                else
                {
                    // Normal behavior for non-GIF images
                    tappedRectangle.IsVisible = true;
                    tappedRectangle.Source = ImageSource.FromFile(image);
                }
            }
            else
            {
                // If the image doesn't exist, display the word as a label
                tappedRectangle.IsVisible = false;
                MediaHelper.DisplayLabelInPlaceOfImage(tappedRectangle, word);
            }
        }


        private async Task PlaySound(string soundFile, GameState gameState)
        {
            if (gameState.IsSoundPlaying) return;

            gameState.IsSoundPlaying = true;

            var (success, errorMessage) = await _soundPlayerService.PlaySoundAsync(soundFile);

            if (!success)
            {
                Log.Error($"Error playing sound: {soundFile}, {errorMessage}");
            }

            gameState.IsSoundPlaying = false;
        }



        public async Task RestoreOriginalUI(Image[] rectangles, Label gameLabel)
        {
            // Hide the cat GIF
            var catGif = Application.Current.MainPage.FindByName<Image>("CatGifImage");
            catGif.IsVisible = false;

            // Show the rectangles and the game label again
            foreach (var rectangle in rectangles)
            {
                rectangle.IsVisible = true;
            }
            gameLabel.IsVisible = true;
        }

    }
}