using Microsoft.Maui.Controls;

namespace FindDeCat.Pages
{
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage(string errorMessage)
        {
            InitializeComponent();

            ErrorMessageLabel.Text = errorMessage;
        }
    }
}