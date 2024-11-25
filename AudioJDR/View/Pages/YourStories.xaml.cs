using System.Diagnostics;
using Model;
using ViewModel;


namespace View.Pages;

public partial class YourStories : ContentPage
{
    private StoryViewModel _viewModel;
    public YourStories()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// Adjusts UI sizes when the page size changes.
    /// </summary>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    /// <summary>
    /// Adjusts the sizes of buttons and other UI elements dynamically based on the current page dimensions.
    /// Ensures that elements do not shrink or grow beyond reasonable limits.
    /// </summary>
    private void SetResponsiveSizes()
    {
        // Use the current page size to set button sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set minimum button sizes to prevent them from becoming too small
        double minButtonWidth = 350;
        double minButtonHeight = 50;

        // Set button sizes dynamically as a percentage of the current page size
        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.24, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

            ImportButton.WidthRequest = buttonWidth;
            ImportButton.HeightRequest = buttonHeight;

            RepeatVoiceButton.WidthRequest = buttonWidth;
            RepeatVoiceButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;

            StoriesList.WidthRequest = pageWidth * 0.85;

            double buttonFontSize = Math.Min(buttonWidth * 0.08, 16);

            ImportButton.FontSize = buttonFontSize;
            RepeatVoiceButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;

            // Adjust button padding to ensure text fits well
            ImportButton.Padding = new Thickness(20, 5);
            RepeatVoiceButton.Padding = new Thickness(20, 5);
            BackButton.Padding = new Thickness(20, 5);
        }
    }

    private async void OnNewGameButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story selectedStory)
        {
            // Load the first event of the selected story
            var firstEvent = selectedStory.Events?.FirstOrDefault(e => e.IdEvent == 1);

            if (firstEvent != null)
            {
                Debug.WriteLine($"Starting game with Story ID: {selectedStory.IdStory}, Event ID: {firstEvent.IdEvent}");
                await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={selectedStory.IdStory}&eventId={firstEvent.IdEvent}");
            }
            else
            {
                Debug.WriteLine("No event found with IdEvent = 1");
                await DisplayAlert("Erreur", "Aucun événement de départ trouvé.", "OK");
            }
        }
        else
        {
            Debug.WriteLine("Erreur: Aucune histoire sélectionnée.");
            await DisplayAlert("Erreur", "Veuillez sélectionner une histoire à jouer.", "OK");
        }
    }

    private async void OnRepeatButtonClicked(object sender, EventArgs e)
    {
    }

    private async void OnImportButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.ImportStoryAsync();
    }   

    private void OnContinueButtonClicked(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Handler for the Delete button click event.
    /// Deletes the selected story from the ViewModel.
    /// </summary>
    /// <param name="sender">The Delete button clicked.</param>
    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        var storyObjet = button?.CommandParameter as Story;

        if (storyObjet != null)
        {
            _viewModel?.DeleteStoryAsync(storyObjet.IdStory);
        }
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPlayerPage));
    }
}