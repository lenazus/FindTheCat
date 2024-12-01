using System;
using System.Globalization;
using Microsoft.Maui.Controls;
//using Microsoft.Maui.Essentials;

namespace FindDeCat
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Use the main display information to determine the size
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            var width = displayInfo.Width / displayInfo.Density; // Actual width in pixels

            // Calculate size based on a fraction of the screen width
            return width / 6; // Adjust the divisor as needed
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}