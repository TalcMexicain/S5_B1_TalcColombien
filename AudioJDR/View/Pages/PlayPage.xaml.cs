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

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        double minButtonWidth = 150;
        double minButtonHeight = 50;

        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

            EventTitleLabel.WidthRequest = Math.Max(pageWidth * 0.8, 250);
            EventDescriptionLabel.WidthRequest = Math.Max(pageWidth * 0.8, 250);

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;
        }
    }

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
            await DisplayAlert("Error", "Event not found.", "OK");
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
                        await DisplayAlert("Error", "Linked event not found.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No linked event found for this option.", "OK");
                }
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
    }
}
