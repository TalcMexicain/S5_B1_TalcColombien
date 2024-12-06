namespace View.Pages;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Model;
using System.Diagnostics;
using View;
using ViewModel;
using View.Resources.Localization;

public partial class MainPlayerPage : ContentPage
{
    #region Fields 

    private SpeechSynthesizerViewModel _speechViewModel;
    private SpeechRecognitionViewModel _recognitionViewModel;

    // Identify the context for the recognition system
    private const string PageContext = "MainPlayerPage";

    #endregion

    #region Constructor

    public MainPlayerPage(ISpeechSynthesizer speechSynthesizer, ISpeechRecognition speechRecognition)
    {
        InitializeComponent();
        SetResponsiveSizes();

        this.SizeChanged += OnSizeChanged;

        
        _speechViewModel = new SpeechSynthesizerViewModel(speechSynthesizer);
        BindingContext = _speechViewModel;

        _recognitionViewModel = new SpeechRecognitionViewModel(speechRecognition);

        _recognitionViewModel.RepeatSpeech += async () => await RepeatSpeech();
        _recognitionViewModel.NavigateNext += async () => await NavigateNext();
        _recognitionViewModel.NavigatePrevious += async () => await NavigatePrevious();
    }

    #endregion

    #region Event Handlers

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        var keywords = new HashSet<string> { AppResources.Repeat, AppResources.StoryList, AppResources.Back };
        _recognitionViewModel.StartRecognition(keywords, PageContext);


        _speechViewModel.TextToSynthesize = this.RulesPlayerLabel.Text;
        _speechViewModel.SynthesizeText();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _speechViewModel.StopSynthesis();
        _recognitionViewModel.UnloadGrammars();
    }

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

    /// <summary>
    /// Navigate to next page.
    /// </summary>
    private async Task NavigateNext()
    {
        await Shell.Current.GoToAsync(nameof(YourStories));
    }

    private async void OnToYourStoriesButtonClicked(object sender, EventArgs e)
    {
        await NavigateNext();
    }

    /// <summary>
    /// Repeat vocal synthesis.
    /// </summary>
    private async Task RepeatSpeech()
    {
        _speechViewModel.TextToSynthesize = this.RulesPlayerLabel.Text;
        _speechViewModel.StopSynthesis();
        _speechViewModel.SynthesizeText();
    }

    private async void OnRepeatButtonClicked(object sender, EventArgs e)
    {
        await RepeatSpeech();
    }

    /// <summary>
    /// Navigate on previous page.
    /// </summary>
    private async Task NavigatePrevious()
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await NavigatePrevious();
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
            UIHelper.SetButtonSize(this, BackButton, true);

            RulesPlayerScrollView.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            RulesPlayerScrollView.HeightRequest = Math.Max(pageHeight * UIHelper.Sizes.FRAME_HEIGHT_FACTOR, UIHelper.Sizes.MIN_FRAME_HEIGHT);
        }
    }

    #endregion
}
