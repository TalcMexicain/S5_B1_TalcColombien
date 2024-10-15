using Model;
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

    // To receive storyId and eventId from the query
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
        }
        if (query.ContainsKey("eventId"))
        {
            _eventId = int.Parse(query["eventId"].ToString());

            // If eventId is 0, it's a new event, otherwise load the existing event
            if (_eventId != 0)
            {
                LoadExistingEvent(_eventId);
            }
        }
    }

    private async void LoadExistingEvent(int eventId)
    {
        var existingEvent = await _storyViewModel.GetEventByIdAsync(_storyId, eventId);
        if (existingEvent != null)
        {
            EventTitleEntry.Text = existingEvent.Name;
            EventContentEditor.Text = existingEvent.Description;
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
        // Go to OptionCreationPage and pass storyId and eventId
        //await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}");
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EventTitleEntry.Text) || !string.IsNullOrWhiteSpace(EventContentEditor.Text))
        {
            // Create or update the event in the StoryViewModel
            if (_eventId == 0)
            {
                // New event
                _storyViewModel.AddEventToStory(_storyId, new Event
                {
                    Name = EventTitleEntry.Text,
                    Description = EventContentEditor.Text
                });
            }
            else
            {
                // Update existing event
                _storyViewModel.UpdateEventInStory(_storyId, new Event
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
            await DisplayAlert("Error", "Please enter both a title and description for the event.", "OK");
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

    private void OnDeleteOptionClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            _storyViewModel.RemoveOptionFromEvent(_storyId, _eventId, selectedOption);
        }
    }
}
