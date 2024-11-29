namespace View.Pages;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Model;
using System.Diagnostics;
using View;
using ViewModel;

public partial class MainPlayerPage : ContentPage
{
    #region Fields 

    private SpeechSynthesizerViewModel _speechViewModel;

    #endregion

    #region Constructor

    public MainPlayerPage(ISpeechSynthesizer speechSynthesizer)
    {
        InitializeComponent();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
        _speechViewModel = new SpeechSynthesizerViewModel(speechSynthesizer);
        BindingContext = _speechViewModel;
    }

    #endregion

    #region Event Handlers

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Pass the label's text to the ViewModel for TTS synthesis
        _speechViewModel.TextToSynthesize = this.RulesPlayerLabel.Text;
        _speechViewModel.SynthesizeText();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop TTS when the page is disappearing
        _speechViewModel.StopSynthesis();
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

    private async void OnRepeatButtonClicked(object sender, EventArgs e)
    {
        _speechViewModel.TextToSynthesize = this.RulesPlayerLabel.Text;
        _speechViewModel.StopSynthesis();
        _speechViewModel.SynthesizeText();
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
            UIHelper.SetButtonSize(this, BackButton, true);

            RulesPlayerScrollView.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            RulesPlayerScrollView.HeightRequest = Math.Max(pageHeight * UIHelper.Sizes.FRAME_HEIGHT_FACTOR, UIHelper.Sizes.MIN_FRAME_HEIGHT);
        }
    }

    #endregion
}