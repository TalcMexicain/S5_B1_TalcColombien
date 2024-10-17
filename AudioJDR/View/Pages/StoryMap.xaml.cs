using ViewModel;
using Model;
using System.Collections.ObjectModel;
using View.Resources.Localization;
using View.Pages;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace View;
public partial class StoryMap : ContentPage, IQueryAttributable
{
    private StoryViewModel _viewModel;
    private int _storyId;

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
        // Example of dynamic size logic
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            Debug.WriteLine($"Received storyId: {_storyId}");
            LoadStory(_storyId); // Load the story by ID
        }
        else
        {
            Debug.WriteLine("No storyId received");
        }
    }

    private async void LoadStory(int storyId)
    {
        var story = await _viewModel.GetStoryByIdAsync(storyId);
        _viewModel.SelectedStory = story;

        if (story != null)
        {
            StoryNameEntry.Text = story.Title;
            StoryDescriptionEditor.Text = story.Description;
            EventList.ItemsSource = story.Events;
            Debug.WriteLine($"Story loaded: {story.Title}, Events count: {story.Events.Count}");
        }
        else
        {
            // Don't add a new story yet, wait for the user to save
            StoryNameEntry.Text = AppResources.NewStoryPlaceholder;
            StoryDescriptionEditor.Text = AppResources.NewStoryPlaceholder;
            EventList.ItemsSource = new ObservableCollection<Event>();
        }
    }

    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event selectedEvent)
        {
            // Navigating to EventCreationPage with both storyId and eventId
            Debug.WriteLine($"Edit Event Clicked: Story ID: {_storyId}, Event ID: {selectedEvent.IdEvent}");
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId={selectedEvent.IdEvent}");
        }
        else
        {
            Debug.WriteLine("Error: Could not retrieve the selected event.");
        }
    }

    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        if(sender is Button button && button.BindingContext is Event selectedEvent)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DeleteOptionConfirmationText, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                // Remove the event from the ViewModel
                await _viewModel.DeleteEventFromStory(_storyId, selectedEvent.IdEvent);

                
            }
        }
    }

    private async void OnCreateNewEventButtonClicked(object sender, EventArgs e)
    {
        // Navigating to EventCreationPage with just the storyId and a new eventId (set as 0 for new event creation)
        Debug.WriteLine($"Create New Event Clicked: Story ID: {_storyId}");
        await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId=0");
    }


    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // If the storyId is 0, treat it as a new story; otherwise, update the existing story
        if (_storyId == 0)
        {
            var newStory = new Story
            {
                IdStory = _viewModel.GenerateNewStoryId(),
                Title = StoryNameEntry.Text,
                Description = StoryDescriptionEditor.Text,
                Events = (ObservableCollection<Event>)EventList.ItemsSource
            };

            await _viewModel.AddStory(newStory); // Add and save the new story
            _storyId = newStory.IdStory; // Update the ID after saving
            _viewModel.SelectedStory = newStory;
        }
        else
        {
            var updatedStory = new Story
            {
                IdStory = _storyId,
                Title = StoryNameEntry.Text,
                Description = StoryDescriptionEditor.Text,
                Events = (ObservableCollection<Event>)EventList.ItemsSource
            };

            await _viewModel.UpdateStory(_storyId, updatedStory); // Save changes
        }

        await Shell.Current.GoToAsync(nameof(StoryList)); // Navigate back to the story list
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }
}
