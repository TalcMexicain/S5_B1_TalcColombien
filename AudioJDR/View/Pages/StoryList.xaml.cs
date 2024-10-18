using View.Pages;
using ViewModel;
using Model;
using System.Diagnostics;

namespace View;

public partial class StoryList : ContentPage
{
    private StoryViewModel _viewModel;

    public StoryList()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged; // To handle resizing
    }

    /// <summary>
    /// Adjusts UI sizes when the page size changes.
    /// </summary>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes(); // Adjust sizes when screen size changes
    }

    /// <summary>
    /// Adjusts the sizes of buttons and the story list dynamically based on the current page dimensions.
    /// Ensures that buttons do not shrink or grow beyond reasonable limits.
    /// </summary>
    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        double minButtonWidth = 350;
        double minButtonHeight = 50;

        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.24, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

            CreateNewStoryButton.WidthRequest = buttonWidth;
            CreateNewStoryButton.HeightRequest = buttonHeight;

            ImportStoryButton.WidthRequest = buttonWidth;
            ImportStoryButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;

            StoriesList.WidthRequest = pageWidth * 0.85;

            double buttonFontSize = Math.Min(buttonWidth * 0.08, 16);

            CreateNewStoryButton.FontSize = buttonFontSize;
            ImportStoryButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;

            CreateNewStoryButton.Padding = new Thickness(20, 5);
            ImportStoryButton.Padding = new Thickness(20, 5);
            BackButton.Padding = new Thickness(20, 5);
        }
    }


    /// <summary>
    /// Opens the StoryMap page to edit the selected story.
    /// </summary>
    /// <param name="sender">The Edit button clicked.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story selectedStory)
        {
            Debug.WriteLine($"Edit Button Clicked: Story ID: {selectedStory.IdStory}, Title: {selectedStory.Title}");
            await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={selectedStory.IdStory}");
        }
        else
        {
            Debug.WriteLine("Error: Could not retrieve the selected story.");
        }
    }


    private async void OnCreateNewStoryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId=0");
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainCreatorPage)); // Navigate back to MainCreatorPage
    }

    private async void OnImportButtonClicked(object sender, EventArgs e)
    {
        await _viewModel.ImportStoryAsync();
    }

    /// <summary>
    /// Exports the selected story by triggering the ExportStoryAsync method in the ViewModel.
    /// </summary>
    /// <param name="sender">The Export button clicked.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnExportButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story story)
        {
            Debug.WriteLine($"Exportation of story with id = {story.IdStory} initiated");
            await _viewModel?.ExportStoryAsync(story);
        }
        else
        {
            Debug.WriteLine($"Exportation : story was not found");
        }
    }


    /// <summary>
    /// Deletes the selected story by triggering the DeleteStory method in the ViewModel.
    /// </summary>
    /// <param name="sender">The Delete button clicked.</param>
    /// <param name="e">Event arguments.</param>
    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var storyObjet = button?.CommandParameter as Story;

        if (storyObjet != null)
        {
            _viewModel?.DeleteStory(storyObjet.IdStory);
        }
    }

}
