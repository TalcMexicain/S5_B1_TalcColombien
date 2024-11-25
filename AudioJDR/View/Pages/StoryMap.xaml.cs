using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Pages;
using View.Resources.Localization;
using ViewModel;

namespace View;

/// <summary>
/// Represents the StoryMap page that allows users to view and edit a specific story,
/// including its title, description, and associated events.
/// The page also handles saving changes, navigating to event creation, and event editing.
/// </summary>
public partial class StoryMap : ContentPage, IQueryAttributable
{
    private readonly StoryViewModel _viewModel;
    private int _storyId;
    private string _initialTitle;
    private string _initialDescription;
    private ObservableCollection<Event> _initialEvents;

    public StoryMap()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.24, 350);
            double buttonHeight = Math.Max(pageHeight * 0.08, 50);

            StoryNameEntry.WidthRequest = Math.Max(pageWidth * 0.7, 300);
            StoryDescriptionEditor.WidthRequest = Math.Max(pageWidth * 0.7, 300);
            EventList.WidthRequest = Math.Max(pageWidth * 0.9, 300);

            SaveButton.WidthRequest = buttonWidth;
            SaveButton.HeightRequest = buttonHeight;

            CreateNewEventButton.WidthRequest = buttonWidth;
            CreateNewEventButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;

            double buttonFontSize = Math.Min(buttonWidth * 0.08, 18);
            SaveButton.FontSize = buttonFontSize;
            CreateNewEventButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;
        }
    }

    /// <summary>
    /// Applies query attributes, setting the story ID and loading the story.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            Debug.WriteLine($"Received storyId: {_storyId}");

            if (_storyId == 0)
            {
                await _viewModel.InitializeNewStoryAsync();
            }
            else
            {
                _viewModel.CurrentStory = await _viewModel.GetStoryByIdAsync(_storyId);
            }

            InitializeStoryState();
            
            Debug.WriteLine(_storyId == 0 
                ? "Initialized new story creation." 
                : $"Story loaded: {_viewModel.CurrentStory.Title}, Events count: {_viewModel.CurrentStory.Events.Count}");
        }
        else
        {
            Debug.WriteLine("No storyId received");
        }
    }

    private void InitializeStoryState()
    {
        StoryNameEntry.Text = _viewModel.CurrentStory.Title;
        StoryDescriptionEditor.Text = _viewModel.CurrentStory.Description;
        EventList.ItemsSource = _viewModel.CurrentStory.Events;

        _initialTitle = _viewModel.CurrentStory.Title;
        _initialDescription = _viewModel.CurrentStory.Description;
        _initialEvents = new ObservableCollection<Event>(_viewModel.CurrentStory.Events);
    }

    private async Task UpdateStory()
    {
        if (_viewModel.CurrentStory != null)
        {
            await _viewModel.UpdateStoryAsync(_viewModel.CurrentStory.IdStory, _viewModel.CurrentStory);
            Debug.WriteLine("Story updated successfully.");
        }
    }

    private async void OnEditEventButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Event currentEvent)
        {
            Debug.WriteLine($"Edit Event Clicked: Event ID: {currentEvent.IdEvent}");
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId={currentEvent.IdEvent}");
        }
    }

    private async void OnDeleteEventButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Event currentEvent)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DeleteEventConfirmationText, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                var eventViewModel = new EventViewModel(_viewModel, currentEvent);
                await eventViewModel.DeleteEventAsync(currentEvent.IdEvent);
                RefreshEventList();
            }
        }
    }

    private void RefreshEventList()
    {
        EventList.ItemsSource = null;
        EventList.ItemsSource = _viewModel.CurrentStory.Events;
    }

    private async void OnCreateNewEventButtonClicked(object sender, EventArgs e)
    {
        bool shouldProceed = await ConfirmAndSaveIfNecessary();

        if (shouldProceed)
        {
            Debug.WriteLine($"Create New Event Clicked: Story ID: {_storyId}");
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId=0");
        }
        else
        {
            Debug.WriteLine("Navigation canceled by user.");
        }
    }

    private async Task<bool> ConfirmAndSaveIfNecessary()
    {
        bool hasChanges = _initialTitle != StoryNameEntry.Text ||
                          _initialDescription != StoryDescriptionEditor.Text ||
                          !_initialEvents.SequenceEqual(_viewModel.CurrentStory.Events);

        bool result = true;
        if (hasChanges)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DiscardChangesMessage, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                await UpdateStory();
            }
            else
            {
                result = false;
            }
        }
        return result;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await UpdateStory();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        bool shouldProceed = await ConfirmAndSaveIfNecessary();
        if (shouldProceed)
        {
            await Shell.Current.GoToAsync(nameof(StoryList));
        }
    }
}
