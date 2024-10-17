using Microsoft.Extensions.Logging;
using Model;
using System.Diagnostics;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

public partial class EventCreationPage : ContentPage, IQueryAttributable
{
    private readonly StoryViewModel _storyViewModel;
    private int _storyId;
    private int _eventId;

    public EventCreationPage()
    {
        InitializeComponent();

        _storyViewModel = new StoryViewModel();
        BindingContext = _storyViewModel;

        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged; // Handle dynamic resizing
    }

    /// <summary>
    /// To receive storyId and eventId from the query - Called when navigating to this page
    /// </summary>
    /// <param name="query"></param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _storyViewModel.SelectedStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
        }
        if (query.ContainsKey("eventId"))
        {
            _eventId = int.Parse(query["eventId"].ToString());

            // Load the event if it exists
            if (_eventId != 0)
            {
                var existingEvent = await LoadEventData(_eventId);

                // Fill the page with event data
                PopulateEventDetails(existingEvent); 
                RefreshOptionsList(existingEvent);    
            }
        }
        Debug.WriteLine($"Opened EventCreation Page with story (id = {_storyId}) and event (id = {_eventId})");
    }

    /// <summary>
    /// Load event data
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    private async Task<Event> LoadEventData(int eventId)
    {
        return await _storyViewModel.GetEventByIdAsync(_storyId, eventId);
    }

    /// <summary>
    /// Populate event details (title and description) into UI
    /// </summary>
    /// <param name="existingEvent"></param>
    private void PopulateEventDetails(Event existingEvent)
    {
        if (existingEvent != null)
        {
            EventTitleEntry.Text = existingEvent.Name;
            EventContentEditor.Text = existingEvent.Description;
        }
    }

    /// <summary>
    /// Refreshes the OptionsList
    /// </summary>
    /// <param name="existingEvent"></param>
    private void RefreshOptionsList(Event existingEvent)
    {
        if (existingEvent != null)
        {
            OptionsList.ItemsSource = null; // Clear the existing items - forces refresh on rebind
            OptionsList.ItemsSource = existingEvent.Options; // Bind updated options
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes(); // Adjust sizes when screen size changes
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

            EventTitleEntry.WidthRequest = Math.Max(pageWidth * 0.8, 250);
            EventContentEditor.WidthRequest = Math.Max(pageWidth * 0.8, 250);

            SaveButton.WidthRequest = buttonWidth;
            SaveButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;
        }
    }

    private async void OnAddOptionClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId=0");
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EventTitleEntry.Text) || !string.IsNullOrWhiteSpace(EventContentEditor.Text))
        {
            // Create or update the event in the StoryViewModel
            if (_eventId == 0)
            {
                // New event
                await _storyViewModel.AddEventToStory(_storyId, new Event
                {
                    IdEvent = _storyViewModel.GenerateNewEventId(),
                    Name = EventTitleEntry.Text,
                    Description = EventContentEditor.Text
                });
            }
            else
            {
                // Update existing event
                await _storyViewModel.UpdateEventInStory(_storyId, new Event
                {
                    IdEvent = _eventId,
                    Name = EventTitleEntry.Text,
                    Description = EventContentEditor.Text
                });
            }
            // Go back to StoryMap after saving
            await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
        }
        else
        {
            await DisplayAlert(AppResources.Error, AppResources.ErrorEventTitleDesc, "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Navigate back to StoryMap
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
    }

    private async void OnEditOptionClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId={selectedOption.IdOption}");
        }
    }

    private async void OnDeleteOptionClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DeleteOptionConfirmationText, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                // Remove the option from the ViewModel
                await _storyViewModel.RemoveOptionFromEvent(_storyId, _eventId, selectedOption);

                // Refresh the option List
                var existingEvent = await LoadEventData(_eventId);
                RefreshOptionsList(existingEvent);
            }
        }
    }
}
