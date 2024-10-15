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

        if (pageWidth > 0 && pageHeight > 0)
        {
            // Adjust frames and buttons
            EventTitleEntry.WidthRequest = Math.Max(pageWidth * 0.8, 250);
            EventContentEditor.WidthRequest = Math.Max(pageWidth * 0.8, 250);

            SaveButton.WidthRequest = Math.Max(pageWidth * 0.25, 150);
            SaveButton.HeightRequest = Math.Max(pageHeight * 0.08, 60);

            BackButton.WidthRequest = Math.Max(pageWidth * 0.25, 150);
            BackButton.HeightRequest = Math.Max(pageHeight * 0.08, 60);
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
