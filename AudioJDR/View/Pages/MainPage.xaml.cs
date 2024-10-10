using View.Pages;

namespace View
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            
        }

        private async void OnCreateButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the NewPage
            await Shell.Current.GoToAsync(nameof(MainCreatorPage));
        }

        private async void OnOptionButtonClicked(object sender, EventArgs e)
        {
            
        }
    }

}
