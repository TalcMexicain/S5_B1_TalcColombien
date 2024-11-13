using View.Pages;

namespace View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SetResponsiveSizes();
            this.SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Event handler triggered when the page size changes. 
        /// Calls a method to adjust the UI elements based on the new size.
        /// </summary>
        /// <param name="sender">The source of the event (typically the page itself).</param>
        /// <param name="e">Event arguments.</param>
        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetResponsiveSizes();
        }


        /// <summary>
        /// Dynamically adjusts the sizes and fonts of the buttons based on the 
        /// current dimensions of the page. Ensures buttons do not become too small 
        /// and that the font size is adjusted to fit the button width properly.
        /// </summary>
        private void SetResponsiveSizes()
        {
            // Use the current page size to set button sizes dynamically
            double pageWidth = this.Width;
            double pageHeight = this.Height;

            // Set minimum button sizes to prevent them from becoming too small
            double minButtonWidth = 150;
            double minButtonHeight = 50;

            // Set button sizes dynamically as a percentage of the current page size
            if (pageWidth > 0 && pageHeight > 0)
            {
                double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth);
                double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

                // Apply dynamic sizes to buttons
                PlayButton.WidthRequest = buttonWidth;
                PlayButton.HeightRequest = buttonHeight;

                CreateButton.WidthRequest = buttonWidth;
                CreateButton.HeightRequest = buttonHeight;

                SettingsButton.WidthRequest = buttonWidth;
                SettingsButton.HeightRequest = buttonHeight;

                // Adjust font size based on button width, with a maximum size to avoid overflow
                double buttonFontSize = Math.Min(buttonWidth * 0.1, 24);

                PlayButton.FontSize = buttonFontSize;
                CreateButton.FontSize = buttonFontSize;
                SettingsButton.FontSize = buttonFontSize;
            }
        }


        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPlayerPage));
        }

        private async void OnCreateButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainCreatorPage));
        }

        private async void OnOptionButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }
    }
}
