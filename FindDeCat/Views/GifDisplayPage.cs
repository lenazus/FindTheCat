using FindDeCat.Configuration;
using Microsoft.Maui.Controls;

namespace FindDeCat.Views
{
    public class GifDisplayPage : ContentPage
    {
        public GifDisplayPage(string gifPath)
        {
            Content = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Parse(AppConfiguration.GameSettings.BACKGROUND_COLOR),
                Children =
                {
                    new WebView
                    {
                        WidthRequest = 300, // Set a default width if you need it; can be removed
                        HeightRequest = 300, // Set a default height if you need it; can be removed
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = Color.Parse(AppConfiguration.GameSettings.BACKGROUND_COLOR),
                        Source = new HtmlWebViewSource
                        {
                            Html = $@"
                            <html>
                            <body style='margin:0;padding:0;overflow:hidden;background-color:{Color.Parse(AppConfiguration.GameSettings.BACKGROUND_COLOR)};display:flex;justify-content:center;align-items:center;'>
                                <img src='{gifPath}' style='display:block;' />
                            </body>
                            </html>"
                        }
                    }
                }
            };
        }
    }
}
