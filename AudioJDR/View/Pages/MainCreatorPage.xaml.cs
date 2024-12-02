namespace View.Pages;
using View;

/// <summary>
/// Represents the MainCreatorPage or creator menu, provides access to the Creator area
/// </summary>
public partial class MainCreatorPage : ContentPage
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the MainCreatorPage class, sets up the UI, 
    /// and subscribes to the SizeChanged event to adjust button sizes dynamically.
    /// </summary>
    public MainCreatorPage()
    {
        InitializeComponent();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Event Handlers

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private async void OnGoToStoryListButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        // Use the current page size to set button sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set button sizes dynamically using UIHelper
        if (pageWidth > 0 && pageHeight > 0)
        {
            UIHelper.SetButtonSize(this, ToStoryListButton, false); 
            UIHelper.SetButtonSize(this, BackButton, true); 
        }
    }

    #endregion
}
