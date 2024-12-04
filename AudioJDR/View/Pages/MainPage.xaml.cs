using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using View.Pages;
using View.Resources.Localization;
using ViewModel;
namespace View;


/// <summary>
/// Represents the MainPage or main menu, provides access to both main parts of the app as well as the parameters
/// </summary>
public partial class MainPage : ContentPage
{
    #region Constructor
    private SpeechRecognitionViewModel _recognitionViewModel;

    private const string PageContext = "MainPage";

    /// <summary>
    /// Initializes a new instance of the MainPage class, sets up the UI, 
    /// and subscribes to the SizeChanged event to adjust button sizes dynamically.
    /// </summary>
    public MainPage(ISpeechRecognition speechRecognition)
    {

        InitializeComponent();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;

        BindingContext = _recognitionViewModel;
        _recognitionViewModel = new SpeechRecognitionViewModel(speechRecognition);
        _recognitionViewModel.NavigateToPlay += async () => await NavigateToPlay();
    }

    #endregion

    #region Event Handlers

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var keywords = new[] { "jouer" };
        _recognitionViewModel.StartRecognition(keywords, PageContext);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _recognitionViewModel.UnloadGrammars();
    }


    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private async Task NavigateToPlay()
    {
        await Shell.Current.GoToAsync(nameof(MainPlayerPage));
    }

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        await NavigateToPlay();
    }

    private async void OnCreateStoryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainCreatorPage));
    }

    private async void OnSettingsButtonClicked(object sender, EventArgs e)
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

