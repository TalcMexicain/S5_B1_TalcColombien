namespace View.Pages;
using View;

public partial class MainPlayerPage : ContentPage
{
    #region Constructor

    public MainPlayerPage()
    {
        InitializeComponent();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Event handler triggered when the page size changes. 
    /// Calls a method to adjust UI elements based on the new size.
    /// </summary>
    /// <param name="sender">The source of the event (typically the page itself).</param>
    /// <param name="e">Event arguments.</param>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private async void OnRepeatButtonClicked(object sender, EventArgs e)
    {
        // Implementation for repeat button click
    }

    private async void OnToYourStoriesButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(YourStories));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    #endregion

    #region UI Management

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

        // Set button sizes dynamically using UIHelper
        if (pageWidth > 0 && pageHeight > 0)
        {
            UIHelper.SetButtonSize(this, RepeatButton, false);
            UIHelper.SetButtonSize(this, ToYourStoriesListButton, false);
            UIHelper.SetButtonSize(this, BackButton, true); // Assuming BackButton is a back button
        }
    }

    #endregion
}