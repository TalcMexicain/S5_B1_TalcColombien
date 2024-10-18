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
    /// Receives and applies the storyId and eventId from the query parameters when navigating to this page.
    /// It loads the appropriate story and event data into the view model.
    /// </summary>
    /// <param name="query">A dictionary containing the navigation parameters (storyId, eventId).</param>
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

                // Populate the UI with the event details
                PopulateEventDetails(existingEvent);
                RefreshOptionsList(existingEvent);
            }
        }
        Debug.WriteLine($"Opened EventCreation Page with story (id = {_storyId}) and event (id = {_eventId})");
    }


    /// <summary>
    /// Retrieves the event data associated with the given event ID from the ViewModel.
    /// </summary>
    /// <param name="eventId">The ID of the event to load.</param>
    /// <returns>The event associated with the given ID, or null if not found.</returns>
    private async Task<Event> LoadEventData(int eventId)
    {
        return await _storyViewModel.GetEventByIdAsync(_storyId, eventId);
    }


    /// <summary>
    /// Fills the page's UI elements with the details of the provided event (title, description).
    /// </summary>
    /// <param name="existingEvent">The event whose details should be displayed in the UI.</param>
    private void PopulateEventDetails(Event existingEvent)
    {
        if (existingEvent != null)
        {
            EventTitleEntry.Text = existingEvent.Name;
            EventContentEditor.Text = existingEvent.Description;
        }
    }


    /// <summary>
    /// Refreshes the list of options for the provided event by re-binding the event's options
    /// to the OptionsList view.
    /// </summary>
    /// <param name="existingEvent">The event whose options should be displayed.</param>
    private void RefreshOptionsList(Event existingEvent)
    {
        if (existingEvent != null)
        {
            OptionsList.ItemsSource = null; // Clear the existing items - forces refresh on rebind
            OptionsList.ItemsSource = existingEvent.Options; // Bind updated options
        }
    }


    /// <summary>
    /// Event handler for dynamically adjusting UI element sizes when the page is resized.
    /// This ensures the page layout remains responsive to different screen sizes.
    /// </summary>
    /// <param name="sender">The source of the event (typically the page itself).</param>
    /// <param name="e">Event arguments.</param>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes(); // Adjust sizes when screen size changes
    }


    /// <summary>
    /// Dynamically adjusts the sizes of various UI elements (buttons, text inputs) based on
    /// the current page dimensions to ensure a responsive layout.
    /// </summary>
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


    /// <summary>
    /// Handles the logic for creating or updating an event and navigating to the OptionCreationPage 
    /// to add options related to the event.
    /// </summary>
    /// <param name="sender">The object that triggered the event (typically a button).</param>
    /// <param name="e">Event arguments associated with the button click.</param>
    private async void OnAddOptionClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EventTitleEntry.Text) || !string.IsNullOrWhiteSpace(EventContentEditor.Text))
        {
            // Create or update event
            if (_eventId == 0)
            {
                // New event
                var newEvent = new Event
                {
                    IdEvent = _storyViewModel.GenerateNewEventId(),
                    Name = EventTitleEntry.Text,
                    Description = EventContentEditor.Text
                };

                // Add new event to the story
                await _storyViewModel.AddEventToStory(_storyId, newEvent);

                _eventId = newEvent.IdEvent;
            }
            else
            {
                // Update existing event
                var updatedEvent = new Event
                {
                    IdEvent = _eventId,
                    Name = EventTitleEntry.Text,
                    Description = EventContentEditor.Text
                };

                await _storyViewModel.UpdateEventInStory(_storyId, updatedEvent);
            }

            // Navigate to the OptionCreationPage for adding options to the event
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId=0");
        }
        else
        {
            await DisplayAlert(AppResources.Error, AppResources.ErrorEventTitleDesc, "OK");
        }
    }


    /// <summary>
    /// Handles the save button click event by either creating or updating an event
    /// in the story and navigating back to the StoryMap page after saving.
    /// </summary>
    /// <param name="sender">The source of the event (the save button).</param>
    /// <param name="e">Event arguments.</param>
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
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
    }

    private async void OnEditOptionClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId={selectedOption.IdOption}");
        }
    }

    /// <summary>
    /// Handles the click event for deleting an option from the event by confirming with the user
    /// and removing the option from the ViewModel if confirmed.
    /// </summary>
    /// <param name="sender">The source of the event (the delete button).</param>
    /// <param name="e">Event arguments.</param>
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
