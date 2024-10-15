using ViewModel;
using Model;

namespace View.Pages;

public partial class OptionCreationPage : ContentPage
{
    private readonly EventViewModel eventViewModel;
    private readonly OptionViewModel optionViewModel;

    public OptionCreationPage(EventViewModel eventVM)
    {
        InitializeComponent();
        eventViewModel = eventVM;
        optionViewModel = new OptionViewModel();
        BindingContext = eventViewModel;

        // Handle dynamic resizing
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private void SetResponsiveSizes()
    {
        // Get the current page size
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth <= 0 || pageHeight <= 0)
            return;

        // Define minimum sizes to prevent elements from becoming too small
        double minButtonWidth = 150;
        double minButtonHeight = 50;

        double minFrameWidth = 300;
        double minEditorHeight = 200;

        // Calculate dynamic sizes based on page dimensions
        double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth); // 60% of page width or min size
        double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight); // 8% of page height or min size

        double frameWidth = Math.Max(pageWidth * 0.8, minFrameWidth); // 80% of page width or min size
        double editorHeight = Math.Max(pageHeight * 0.15, minEditorHeight); // 15% of page height or min size

        // Adjust sizes for individual elements

        // Set frame widths
        OptionNameFrame.WidthRequest = frameWidth;
        OptionTextFrame.WidthRequest = frameWidth;
        EventPickerFrame.WidthRequest = frameWidth;

        // Set editor height
        OptionTextWord.HeightRequest = editorHeight;

        // Adjust button sizes
        SaveButton.WidthRequest = buttonWidth;
        SaveButton.HeightRequest = buttonHeight;

        BackButton.WidthRequest = buttonWidth;
        BackButton.HeightRequest = buttonHeight;

        // Adjust font sizes based on button width
        double buttonFontSize = Math.Min(buttonWidth * 0.08, 18); // Scale font size based on button width

        SaveButton.FontSize = buttonFontSize;
        BackButton.FontSize = buttonFontSize;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        eventViewModel.LoadEvent();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        Option option = new Option
        {
            NameOption = this.OptionNameEntry.Text,
            Text = this.OptionTextWord.Text
        };

        // Assign the selected event to the LinkedEvent property of the option
        if (EventPicker.SelectedItem is Event selectedEvent)
        {
            option.LinkedEvent = selectedEvent;
        }

        eventViewModel.AddOption(option);
        await Shell.Current.GoToAsync(nameof(EventCreationPage));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EventCreationPage));
    }
}
