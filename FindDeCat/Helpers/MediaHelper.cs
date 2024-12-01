using System;
using System.Collections.Generic;
using System.IO;
using FindDeCat.Configuration;
using Microsoft.Maui.Graphics;

namespace FindDeCat.Services
{
    public static class MediaHelper
    {
        private static Random _random = new();

        public static void ResetImages(Image[] rectangles)
        {
            var randomizedImages = AppConfiguration.GameSettings.RECTANGLE_IMAGES
                .OrderBy(_ => _random.Next())
                .ToArray(); 

            for (int i = 0; i < rectangles.Length; i++)
            {
                if (i >= randomizedImages.Length) break; // Avoid out-of-bounds issues

                var rectangle = rectangles[i];
                rectangle.Source = randomizedImages[i];
                rectangle.IsVisible = true;

                RemoveLabelInRectanglePosition(rectangle);
            }
        }

        private static void RemoveLabelInRectanglePosition(Image rectangle)
        {
            // Assuming the images and labels are within a Grid
            var parentLayout = rectangle.Parent as Microsoft.Maui.Controls.Grid;
            if (parentLayout == null) return;

            // Get the row and column of the rectangle
            var row = Microsoft.Maui.Controls.Grid.GetRow(rectangle);
            var column = Microsoft.Maui.Controls.Grid.GetColumn(rectangle);

            // Find all labels in the same position and remove them
            var labelsToRemove = parentLayout.Children
                .OfType<Label>()
                .Where(label => Microsoft.Maui.Controls.Grid.GetRow(label) == row &&
                                Microsoft.Maui.Controls.Grid.GetColumn(label) == column)
                .ToList();

            foreach (var label in labelsToRemove)
            {
                parentLayout.Children.Remove(label);
            }
        }

        public static string GetRandomCatImage()
        {
            int randomCatGifNum = _random.Next(1, AppConfiguration.GameSettings.NUM_OF_CAT_GIFS_IN_RESOURCES + 1);
            return $"cat{randomCatGifNum}.gif";
        }

        public static string GetRandomCatSound()
        {
            int randomCatSoundNum = _random.Next(1, AppConfiguration.GameSettings.NUM_OF_CAT_SOUNDS_IN_RESOURCES + 1);
            return $"meow{randomCatSoundNum}.m4a";
        }

        public static void DisplayLabelInPlaceOfImage(Image tappedRectangle, string word)
        {
            var parentLayout = (Microsoft.Maui.Controls.Grid)tappedRectangle.Parent;
            var row = Microsoft.Maui.Controls.Grid.GetRow(tappedRectangle);
            var column = Microsoft.Maui.Controls.Grid.GetColumn(tappedRectangle);

            // Check if label already exists; if not, create one
            var wordLabel = parentLayout.Children
                .OfType<Label>()
                .FirstOrDefault(label => Microsoft.Maui.Controls.Grid.GetRow(label) == row &&
                                         Microsoft.Maui.Controls.Grid.GetColumn(label) == column);

            if (wordLabel == null)
            {
                wordLabel = new Label
                {
                    Text = word,
                    FontSize = AppConfiguration.GameSettings.DISPLAY_WORD_FONT_SIZE,
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                // Place the label in the same grid position as tappedRectangle
                Microsoft.Maui.Controls.Grid.SetRow(wordLabel, row);
                Microsoft.Maui.Controls.Grid.SetColumn(wordLabel, column);

                // Add the label to the grid
                parentLayout.Children.Add(wordLabel);
            }
        }

        public static int GetGifDuration(string gifPath)
        {
            return AppConfiguration.GameSettings.DEFAULT_CAT_GIF_DISPLAY_DURATION;
        }
    }
}