namespace View.Pages;
using View;

public partial class MainPlayerPage : ContentPage
{
	public MainPlayerPage()
    {
        InitializeComponent();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }
    /// <summary>
    /// Event handler triggered when the page size changes. 
    /// Calls a method to adjust UI elements based on the new size.
    /// </summary>
    /// <param name="sender">The source of the event (typically the page itself).</param>
    /// <param name="e">Event arguments.</param>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    /// <summary>
    /// Adjusts the sizes and padding of buttons and other UI elements dynamically 
    /// based on the current page dimensions. Ensures that buttons do not become 
    /// too small and that font sizes and padding are properly set for readability.
    /// </summary>
    private void SetResponsiveSizes()
    {
        // Use the current page size to set button sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set minimum button sizes to prevent them from becoming too small
        double minButtonWidth = 250;
        double minButtonHeight = 70;

        // Set button sizes dynamically as a percentage of the current page size
        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.24, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

            // Adjust the width and height of buttons
            RepeatButton.WidthRequest = buttonWidth;
            RepeatButton.HeightRequest = buttonHeight;

            ToYourStoriesListButton.WidthRequest = buttonWidth;
            ToYourStoriesListButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;

            // Adjust font size based on button width, with a maximum size to avoid overflow
            double buttonFontSize = Math.Min(buttonWidth * 0.06, 20);

            // Set font sizes for buttons
            RepeatButton.FontSize = buttonFontSize;
            ToYourStoriesListButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;

            // Adjust button padding to ensure text fits well
            RepeatButton.Padding = new Thickness(20, 5);
            ToYourStoriesListButton.Padding = new Thickness(20, 5);
            BackButton.Padding = new Thickness(20, 5);
        }
    }

    private async void OnRepeatButtonClicked(object sender, EventArgs e)
    {
        
    }
    private async void OnToYourStoriesButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(YourStories));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}