using View.Pages;
using ViewModel;
using Model;

namespace View;

public partial class StoryList : ContentPage
{
    private StoryViewModel _viewModel;

    public StoryList()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged; // To handle resizing
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes(); // Adjust sizes when screen size changes
    }

    private void SetResponsiveSizes()
    {
        // Use the current page size to set button sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set minimum button sizes to prevent them from becoming too small
        double minButtonWidth = 300; 
        double minButtonHeight = 50; 

        // Set button sizes dynamically as a percentage of the current page size
        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.35, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.1, minButtonHeight);

            CreateNewStoryButton.WidthRequest = buttonWidth;
            CreateNewStoryButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.75;
            BackButton.HeightRequest = buttonHeight;

            StoriesList.WidthRequest = pageWidth * 0.85;

            double buttonFontSize = Math.Min(buttonWidth * 0.08, 16); // Limit font size to avoid overflow

            CreateNewStoryButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;

            // Adjust button padding to ensure text fits well
            CreateNewStoryButton.Padding = new Thickness(20, 5); 
            BackButton.Padding = new Thickness(20, 5);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadStories(); // Load the stories when the page appears
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Story selectedStory)
        {
            await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={selectedStory.IdStory}");
        }
    }

    private async void OnCreateNewStoryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId=0");
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainCreatorPage)); // Navigate back to MainCreatorPage
    }
}
