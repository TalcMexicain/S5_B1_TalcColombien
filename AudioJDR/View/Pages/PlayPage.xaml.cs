using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Pages;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

/// <summary>
/// The PlayPage class displays events and their related options.
/// </summary>
public partial class PlayPage : ContentPage, IQueryAttributable
{
    private StoryViewModel _viewModel; // ViewModel to manage story and event data
    private int _storyId; // ID of the current story
    private int _eventId; // ID of the current event
    private Story _currentStory;

    /// <summary>
    /// Initializes a new instance of the PlayPage class.
    /// </summary>
    public PlayPage()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
    }

    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth > 0 && pageHeight > 0)
        {
            EventTitleLabel.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            EventDescriptionLabel.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);

            UIHelper.SetButtonSize(this, BackButton, true);
        }
    }

    #endregion

    /// <summary>
    /// Applies query attributes received during navigation.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId") && query.ContainsKey("eventId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _eventId = int.Parse(query["eventId"].ToString());
            await LoadEvent(_storyId, _eventId);
        }
    }

    /// <summary>
    /// Loads the event and its options by story ID and event ID.
    /// </summary>
    /// <param name="storyId">The ID of the story.</param>
    /// <param name="eventId">The ID of the event.</param>
    private async Task LoadEvent(int storyId, int eventId)
    {
        _currentStory = await _viewModel.GetStoryByIdAsync(storyId); // Store the story in the private field
        var eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);

        if (eventToShow != null)
        {
            EventTitleLabel.Text = eventToShow.Name;
            EventDescriptionLabel.Text = eventToShow.Description;
            OptionsListView.ItemsSource = eventToShow.Options;
        }
        else
        {
            await UIHelper.ShowErrorDialog(this, "Event not found.");
        }
    }

    /// <summary>
    /// Handles the selection of an option by navigating to the next event.
    /// </summary>
    /// <param name="sender">The sender object (CollectionView).</param>
    /// <param name="e">Event arguments containing the current selection.</param>
    private async void OnOptionSelected(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            var selectedOption = button.CommandParameter as Option;
            if (selectedOption != null)
            {
                // Check if the selected option has a linked event
                if (selectedOption.LinkedEvent != null)
                {
                    var linkedEvent = _currentStory?.Events.FirstOrDefault(ev => ev.IdEvent == selectedOption.LinkedEvent.IdEvent);

                    if (linkedEvent != null)
                    {
                        // Navigate to the event
                        await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={_storyId}&eventId={linkedEvent.IdEvent}");
                    }
                    else
                    {
                        await UIHelper.ShowErrorDialog(this, "Linked event not found.");
                    }
                }
                else
                {
                    await UIHelper.ShowErrorDialog(this, "No linked event found for this option.");
                }
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(YourStories)}?storyId={_storyId}");
    }
}
