using ViewModel;
using Model;
using System.Collections.ObjectModel;
using View.Resources.Localization;

namespace View;

public partial class StoryMap : ContentPage, IQueryAttributable
{
    private StoryViewModel _viewModel;

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
            double buttonWidth = Math.Max(pageWidth * 0.45, 150);
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
            LoadStory(storyId); // Load story by ID
        }
    }

    private void LoadStory(int storyId)
    {
        var selectedStory = _viewModel.GetStoryById(storyId);
        if (selectedStory != null)
        {
            _viewModel.SelectedStory = selectedStory;
        }
        else
        {
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
        }
    }

    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        // Event edit logic here
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }
}
