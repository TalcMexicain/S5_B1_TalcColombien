using Model;
using System.Diagnostics;
using View.Resources.Localization;
using View.Pages;
using ViewModel;

namespace View;

/// <summary>
/// Creator-Side Page displaying the list of all stories to be managed.
/// </summary>
public partial class StoryList : ContentPage
{
    #region Fields

    private readonly StoryViewModel _storyViewModel;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the StoryList page.
    /// </summary>
    public StoryList()
    {
        InitializeComponent();
        _storyViewModel = new StoryViewModel();
        BindingContext = _storyViewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Event Handlers

    private async void OnCreateNewStoryButtonClicked(object sender, EventArgs e)
    {
        await _storyViewModel.InitializeNewStoryAsync();
        await NavigateToStoryMap(_storyViewModel.CurrentStory.IdStory);
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await NavigateToMainCreator();
    }

    private async void OnEditStoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story story)
        {
            StoriesList.SelectedItem = null;
            await NavigateToStoryMap(story.IdStory);
        }
    }

    private async void OnDeleteStoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story story)
        {
            bool confirmed = await UIHelper.ShowDeleteConfirmationDialog(this, story.Title);

            if (confirmed)
            {
                await DeleteStory(story);
            }
        }
    }

    private async void OnExportStoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story story)
        {
            await ExportStory(story);
        }
    }

    private async void OnImportStoryClicked(object sender, EventArgs e)
    {
        await ImportStory();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    #endregion

    #region Navigation

    private async Task NavigateToStoryMap(int storyId)
    {
        Dictionary<string, object> navigationParameter = new Dictionary<string, object>
        {
            { "storyId", storyId }
        };
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}", navigationParameter);
    }

    private async Task NavigateToMainCreator()
    {
        await Shell.Current.GoToAsync($"{nameof(MainCreatorPage)}");
    }

    #endregion

    #region Story Operations

    private async Task DeleteStory(Story story)
    {
        try
        {
            await _storyViewModel.DeleteStoryAsync(story.IdStory);
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, string.Format(AppResources.DeleteErrorFormat, story.Title));
            Debug.WriteLine($"Error deleting story: {ex.Message}");
        }
    }

    private async Task ExportStory(Story story)
    {
        try
        {
            bool success = await _storyViewModel.ExportStoryAsync(story);
            if (success) await UIHelper.ShowSuccessDialog(this, AppResources.ExportSuccess);
            else await UIHelper.ShowErrorDialog(this, string.Format(AppResources.ExportErrorFormat, story.Title));
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, string.Format(AppResources.ExportErrorFormat, story.Title));
            Debug.WriteLine($"Error exporting story: {ex.Message}");
        }
    }

    private async Task ImportStory()
    {
        try
        {
            bool success = await _storyViewModel.ImportStoryAsync();
            if (success) await UIHelper.ShowSuccessDialog(this, AppResources.ImportSuccess);
            else await UIHelper.ShowErrorDialog(this, AppResources.ImportError);
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, AppResources.ImportError);
            Debug.WriteLine($"Error importing story: {ex.Message}");
        }
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = Width;
        double pageHeight = Height;

        if (pageWidth > 0 && pageHeight > 0)
        {
            // Apply sizes to UI elements using UIHelper
            UIHelper.SetButtonSize(this,CreateNewStoryButton, false); 
            UIHelper.SetButtonSize(this,ImportStoryButton, false); 
            UIHelper.SetButtonSize(this,BackButton, true); 
            StoriesList.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            StoriesList.HeightRequest = pageHeight * UIHelper.Sizes.LIST_HEIGHT_FACTOR;
        }
    }

    #endregion
}

