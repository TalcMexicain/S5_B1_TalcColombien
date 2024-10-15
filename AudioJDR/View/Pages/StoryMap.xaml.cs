using ViewModel;
using Model;
using System.Collections.ObjectModel;
using View.Resources.Localization;
using View.Pages;

namespace View;

public partial class StoryMap : ContentPage, IQueryAttributable
{
    private StoryViewModel _viewModel;
    private EventViewModel _eventViewModel;

    public StoryMap()
    {
        InitializeComponent();
        _viewModel = StoryViewModel.Instance;
        _eventViewModel = new EventViewModel();
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
            int storyId = int.Parse(query["storyId"].ToString());
            System.Diagnostics.Debug.WriteLine($"Received storyId: {storyId}");
            LoadStory(storyId); // Load the story by ID
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("No storyId received");
        }
    }


    private void LoadStory(int storyId)
    {
        var selectedStory = _viewModel.GetStoryById(storyId);

        if (selectedStory != null)
        {
            _viewModel.SelectedStory = selectedStory;
            System.Diagnostics.Debug.WriteLine($"Story loaded: {selectedStory.Title}, Events count: {selectedStory.Events.Count}");

            OnPropertyChanged(nameof(_viewModel.SelectedStory));  // Notify the UI of the change
            OnPropertyChanged(nameof(_viewModel.Events));  // Notify the UI that the event list has changed
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Story not found, creating new story.");

            // Create a new story if it doesn't exist
            var newStory = new Story
            {
                IdStory = _viewModel.GenerateNewStoryId(),
                Title = AppResources.NewStoryPlaceholder,
                Description = AppResources.NewStoryDescPlaceholder,
                Events = new ObservableCollection<Event>()
            };

            _viewModel.SelectedStory = newStory;
            _viewModel.Stories.Add(newStory);

            System.Diagnostics.Debug.WriteLine($"New story created: {newStory.Title}, Events count: {newStory.Events.Count}");

            OnPropertyChanged(nameof(_viewModel.SelectedStory));  // Notify the UI of the change
            OnPropertyChanged(nameof(_viewModel.Events));  // Notify the UI that the event list has changed
        }
    }


    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EventCreationPage(_eventViewModel));
    }
    private async void OnCreateNewEventButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EventCreationPage(_eventViewModel));
    }
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }
}
