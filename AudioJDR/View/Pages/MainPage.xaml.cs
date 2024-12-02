using View.Pages;
namespace View;

/// <summary>
/// Represents the MainPage or main menu, provides access to both main parts of the app as well as the parameters
/// </summary>
public partial class MainPage : ContentPage
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the MainPage class, sets up the UI, 
    /// and subscribes to the SizeChanged event to adjust button sizes dynamically.
    /// </summary>
    public MainPage()
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
            UIHelper.SetButtonSize(this, PlayButton, false); 
            UIHelper.SetButtonSize(this, CreateButton, false); 
            UIHelper.SetButtonSize(this, SettingsButton, false); 
        }
    }

    #endregion
}

