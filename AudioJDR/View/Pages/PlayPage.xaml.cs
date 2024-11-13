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

    /// <summary>
    /// Initializes a new instance of the PlayPage class.
    /// </summary>
    public PlayPage()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
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
        var story = await _viewModel.GetStoryByIdAsync(storyId);
        var eventToShow = story?.Events.FirstOrDefault(e => e.IdEvent == eventId);

        if (eventToShow != null)
        {
            EventTitleLabel.Text = eventToShow.Name;
            EventDescriptionLabel.Text = eventToShow.Description;


            // Load options dynamically into the options list

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
    /// <param name="sender">The sender object.</param>
    /// <param name="e">Event arguments.</param>
    private async void OnOptionSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Option selectedOption)
        {
            // Navigate to the next event specified by the option's NextEventId
            await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={_storyId}&eventId={selectedOption.LinkedEvent}");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
    }
}
