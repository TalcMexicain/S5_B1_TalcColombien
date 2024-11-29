using System.Diagnostics;
using Model;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

public partial class YourStories : ContentPage
{
    #region Fields

    private StoryViewModel _viewModel;
    private SaveViewModel _saveViewModel;

    #endregion

    #region Constructor

    public YourStories()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        _saveViewModel = new SaveViewModel();
        BindingContext = _viewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Adjusts UI sizes when the page size changes.
    /// </summary>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private async void OnNewGameButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story selectedStory)
        {
            Event? firstEvent = selectedStory.FirstEvent;

            if (firstEvent != null)
            {
                await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={selectedStory.IdStory}&eventId={firstEvent.IdEvent}");
            }
            else
            {
                await UIHelper.ShowErrorDialog(this, "Aucun événement de départ trouvé.");
            }
        }
        else
        {
            await UIHelper.ShowErrorDialog(this, "Veuillez sélectionner une histoire à jouer.");
        }
    }

    private async void OnImportButtonClicked(object sender, EventArgs e)
    {
        await ImportStory();
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story story)
        {
            bool confirmed = await UIHelper.ShowDeleteConfirmationDialog(this, story.Title);

            if (confirmed)
            {
                await DeleteStory(story.IdStory);
            }
        }
    }

    private async void OnContinueButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story selectedStory)
        {
            try
            {
                await _saveViewModel.LoadGameAsync(selectedStory.Title);

                Event? savedEvent = _saveViewModel.CurrentSave?.CurrentEvent;

                if (savedEvent != null)
                {
                    await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={selectedStory.IdStory}&eventId={savedEvent.IdEvent}");
                }
                else
                {
                    await UIHelper.ShowErrorDialog(this, AppResources.NoValidSaveFound);
                }
            }
            catch (Exception ex)
            {
                await UIHelper.ShowErrorDialog(this, AppResources.SaveLoadError);
            }
        }
        else
        {
            await UIHelper.ShowErrorDialog(this, "Veuillez sélectionner une histoire.");
        }
    }

    private void OnRepeatButtonClicked(object sender, EventArgs e)
    {

    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPlayerPage));
    }

    #endregion

    #region Story Operations

    private async Task DeleteStory(int storyId)
    {
        try
        {
            await _viewModel.DeleteStoryAsync(storyId);
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, ex.Message);
            Debug.WriteLine($"Error deleting story: {ex.Message}");
        }
    }

    private async Task ImportStory()
    {
        try
        {
            bool success = await _viewModel.ImportStoryAsync();
            if (success)
            {
                await UIHelper.ShowSuccessDialog(this, AppResources.ImportSuccess);
            }
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, ex.Message);
            Debug.WriteLine($"Error importing story: {ex.Message}");
        }
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
            UIHelper.SetButtonSize(this, ImportButton, false);
            UIHelper.SetButtonSize(this, RepeatVoiceButton, false);
            UIHelper.SetButtonSize(this, BackButton, true);

            StoriesList.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            StoriesList.HeightRequest = pageHeight * UIHelper.Sizes.LIST_HEIGHT_FACTOR;
        }
    }

    #endregion
}