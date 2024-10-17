using ViewModel;
using Model;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using View.Resources.Localization;

namespace View.Pages;

public partial class OptionCreationPage : ContentPage, IQueryAttributable
{
    private readonly StoryViewModel _storyViewModel;
    private int _storyId;
    private int _eventId;
    private int _optionId;

    public OptionCreationPage()
    {
        InitializeComponent();

        _storyViewModel = new StoryViewModel();
        BindingContext = _storyViewModel;

        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged; // Handle dynamic resizing
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _storyViewModel.SelectedStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
        }

        if (query.ContainsKey("eventId"))
        {
            _eventId = int.Parse(query["eventId"].ToString());
        }
        PopulateEventPicker();

        if (query.ContainsKey("optionId"))
        {
            _optionId = int.Parse(query["optionId"].ToString());

            // Load the option if it exists
            if (_optionId != 0)
            {
                LoadExistingOption(_optionId);
            }
        }
        
        Debug.WriteLine($"Opened OptionCreation Page with story (id = {_storyId}) , event (id = {_eventId}) and option (id = {_optionId})");
    }

    private async void LoadExistingOption(int optionId)
    {
        var existingOption = await _storyViewModel.GetOptionByIdAsync(_storyId, _eventId, optionId);
        if (existingOption != null)
        {
            OptionNameEntry.Text = existingOption.NameOption;
            OptionTextWord.Text = existingOption.Text;

            SetSelectedEvent(existingOption.LinkedEvent?.IdEvent);

            Debug.WriteLine($"Option found : {existingOption.IdOption}, Title: {existingOption.NameOption}");
        }
    }

    private void SetSelectedEvent(int? linkedEventId)
    {
        // If linkedEventId is null or 0 (for "None"), select "None" in the picker
        if (linkedEventId == null || linkedEventId == 0)
        {
            EventPicker.SelectedItem = _storyViewModel.SelectedStory.Events.FirstOrDefault(e => e.IdEvent == 0); // Select "None"
        }
        else
        {
            // Select the event with the linkedEventId
            EventPicker.SelectedItem = _storyViewModel.SelectedStory.Events.FirstOrDefault(e => e.IdEvent == linkedEventId);
        }
    }

    private void PopulateEventPicker()
    {
        // Get all events from the current story and exclude the current event
        var filteredEvents = _storyViewModel.SelectedStory.Events
            .Where(e => e.IdEvent != _eventId)
            .ToList();

        filteredEvents.Insert(0, new Event { IdEvent = 0, Name = AppResources.None }); // Non existing event 

        EventPicker.ItemsSource = null;
        EventPicker.ItemsSource = filteredEvents;

        Debug.WriteLine($"Populated Event Picker with {filteredEvents.Count} events excluding current event (id = {_eventId})");
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

        BackButton.WidthRequest = buttonWidth * 0.8;
        BackButton.HeightRequest = buttonHeight;

        // Adjust font sizes based on button width
        double buttonFontSize = Math.Min(buttonWidth * 0.08, 18); // Scale font size based on button width

        SaveButton.FontSize = buttonFontSize;
        BackButton.FontSize = buttonFontSize;
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(OptionNameEntry.Text) || !string.IsNullOrWhiteSpace(OptionTextWord.Text))
        {
            // Get the selected event from the picker, set to null if "None" is selected
            var selectedEvent = (Event)EventPicker.SelectedItem;
            if (selectedEvent.IdEvent == 0) { selectedEvent = null; }

            // Create or update the option in the StoryViewModel
            if (_optionId == 0)
            {
                Debug.WriteLine($"Option is new: Creating new option..");
                // New option
                await _storyViewModel.AddOptionToEvent(_storyId, _eventId, new Option
                {
                    IdOption = await _storyViewModel.GenerateNewOptionId(_storyId, _eventId),
                    NameOption = OptionNameEntry.Text,
                    Text = OptionTextWord.Text,
                    LinkedEvent = selectedEvent
                }) ;
            }
            else
            {
                Debug.WriteLine($"Option is not new: Updating option..");
                // Update existing option
                await _storyViewModel.UpdateOptionInEvent(_storyId, _eventId, new Option
                {
                    IdOption = _optionId,
                    NameOption = OptionNameEntry.Text,
                    Text = OptionTextWord.Text,
                    LinkedEvent = selectedEvent
                });
            }

            // Go back to EventCreationPage after saving
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId={_eventId}");
        }
        else
        {
            await DisplayAlert(AppResources.Error, AppResources.ErrorOptionTitleDesc, "OK");
        }
    }


    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId={_eventId}");
    }
}
