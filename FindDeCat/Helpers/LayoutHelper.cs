using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;

namespace FindDeCat.Helpers
{
    public static class LayoutHelper
    {
        public static void AdjustRectangleSizes(Layout<View> parent, params Image[] rectangles)
        {
            if (parent == null)
            {
                return;
            }

            double width = parent.Width > parent.Height ? parent.Width / 100 : parent.Height / 100;

            foreach (var rectangle in rectangles)
            {
                rectangle.WidthRequest = width;
                rectangle.HeightRequest = width;
            }
        }

        public static void UpdateLayout(double width, double height, Image[] rectangles, Label gameLabel)
        {
            var availableWidth = width * 0.9;
            var availableHeight = height * 0.5;
            var rectSize = Math.Min(availableWidth / 2, availableHeight / 2);

            foreach (var rect in rectangles)
            {
                rect.WidthRequest = rectSize;
                rect.HeightRequest = rectSize;
            }

            double fontSize = rectSize / 8;
            gameLabel.FontSize = Math.Max(24, Math.Min(fontSize, 72));
        }
    }
}