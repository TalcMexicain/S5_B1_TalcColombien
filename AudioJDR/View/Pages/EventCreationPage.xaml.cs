using Model;
using ViewModel;

namespace View.Pages;

public partial class EventCreationPage : ContentPage
{
    private readonly EventViewModel _eventViewModel;

    public EventCreationPage(EventViewModel _viewModel)
    {
        InitializeComponent();

        _eventViewModel = _viewModel;
        BindingContext = _eventViewModel;

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
        await Navigation.PushAsync(new OptionCreationPage(_eventViewModel));
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        string eventTitle = EventTitleEntry.Text;
        string eventContent = EventContentEditor.Text;

        if (!string.IsNullOrWhiteSpace(eventTitle) || !string.IsNullOrWhiteSpace(eventContent)){

            _eventViewModel.AddEvent(eventTitle, eventContent);

            await Shell.Current.GoToAsync(nameof(StoryMap));
        }
        else
        {
            await DisplayAlert("Error", "Please enter both a title and description for the event.","OK");
        }
        

        await Shell.Current.GoToAsync(nameof(StoryMap));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }

    private async void OnEditOptionClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var optionObject = button.CommandParameter as Option;

        if (optionObject != null) 
        {
            await Navigation.PushAsync(new OptionCreationPage(_eventViewModel,optionObject));
        }
    }

    private void OnDeleteOptionClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var optionObject = button.CommandParameter as Option;

        if (optionObject != null)
        {
            _eventViewModel.RemoveOption(optionObject);
        }
    }
}
