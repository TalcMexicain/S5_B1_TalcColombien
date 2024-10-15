using ViewModel;

namespace View.Pages;

public partial class EventCreationPage : ContentPage
{
    private readonly EventViewModel _viewModel;

    public EventCreationPage()
    {
        InitializeComponent();

        _viewModel = new EventViewModel();
        BindingContext = _viewModel;

        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged; // Handle dynamic resizing
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes(); // Adjust sizes when screen size changes
    }

    private void SetResponsiveSizes()
    {
        // Use the current page size to set button and frame sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set minimum button sizes to prevent them from becoming too small
        double minButtonWidth = 150;
        double minButtonHeight = 50;

        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth);
            double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);

            // Adjust frames and buttons
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
        await Navigation.PushAsync(new OptionCreationPage(_viewModel));
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryMap));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }
}
