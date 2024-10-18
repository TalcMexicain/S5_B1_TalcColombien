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

    /// <summary>
    /// Applies query parameters passed to the page, such as storyId, eventId, and optionId.
    /// Loads the appropriate story, event, and option data based on these parameters.
    /// </summary>
    /// <param name="query">The dictionary of query parameters.</param>
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
                await LoadExistingOption(_optionId);
                UpdateWordsDisplay();
            }
        }

        Debug.WriteLine($"Opened OptionCreation Page with story (id = {_storyId}) , event (id = {_eventId}) and option (id = {_optionId})");
    }


    /// <summary>
    /// Loads the details of an existing option for editing, populating the input fields with the option data.
    /// </summary>
    /// <param name="optionId">The ID of the option to load.</param>
    private async Task LoadExistingOption(int optionId)
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


    /// <summary>
    /// Sets the selected event in the EventPicker based on the linked event ID of the option.
    /// Selects "None" if the option is not linked to any event.
    /// </summary>
    /// <param name="linkedEventId">The ID of the event linked to the option, or null for no link.</param>
    private void SetSelectedEvent(int? linkedEventId)
    {
        if (linkedEventId == null || linkedEventId == 0)
        {
            EventPicker.SelectedItem = _storyViewModel.SelectedStory.Events.FirstOrDefault(e => e.IdEvent == 0); // Select "None"
        }
        else
        {
            EventPicker.SelectedItem = _storyViewModel.SelectedStory.Events.FirstOrDefault(e => e.IdEvent == linkedEventId);
        }
    }


    /// <summary>
    /// Populates the EventPicker with all events from the selected story, excluding the current event.
    /// Adds an option for "None" to represent no linked event.
    /// </summary>
    private void PopulateEventPicker()
    {
        var filteredEvents = _storyViewModel.SelectedStory.Events
            .Where(e => e.IdEvent != _eventId)
            .ToList();

        filteredEvents.Insert(0, new Event { IdEvent = 0, Name = AppResources.None }); // Non existing event 

        EventPicker.ItemsSource = null;
        EventPicker.ItemsSource = filteredEvents;

        Debug.WriteLine($"Populated Event Picker with {filteredEvents.Count} events excluding current event (id = {_eventId})");
    }



    /// <summary>
    /// Event handler triggered when the page size changes.
    /// Adjusts the UI elements dynamically to fit the new page size.
    /// </summary>
    /// <param name="sender">The source of the event (the page).</param>
    /// <param name="e">Event arguments.</param>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    /// <summary>
    /// Dynamically adjusts the sizes of the UI elements based on the current dimensions of the page.
    /// Ensures elements like buttons, frames, and text editors do not become too small or too large.
    /// </summary>
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
        double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth);
        double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);
        double frameWidth = Math.Max(pageWidth * 0.8, minFrameWidth);
        double editorHeight = Math.Max(pageHeight * 0.15, minEditorHeight);

        // Adjust OptionWordsFrame size
        OptionWordsFrame.WidthRequest = frameWidth;

        // Adjust WordsDisplayLabel font size
        WordsDisplayLabel.FontSize = Math.Min(frameWidth * 0.05, 20); // Adjust based on frame width

        // Adjust Entry size (OptionWordsEntry)
        OptionWordsEntry.WidthRequest = frameWidth * 0.9;
        OptionWordsEntry.FontSize = Math.Min(frameWidth * 0.05, 18); // Adjust based on frame width

        // Adjust Add Button size
        AddWordButton.WidthRequest = buttonWidth;
        AddWordButton.HeightRequest = buttonHeight;
        AddWordButton.FontSize = Math.Min(buttonWidth * 0.08, 18); // Adjust font size based on button width

        // Set frame widths for other frames (if any)
        OptionNameFrame.WidthRequest = frameWidth;
        OptionTextFrame.WidthRequest = frameWidth;
        EventPickerFrame.WidthRequest = frameWidth;

        // Set editor height
        OptionTextWord.HeightRequest = editorHeight;

        // Adjust button sizes for Save and Back buttons
        SaveButton.WidthRequest = buttonWidth;
        SaveButton.HeightRequest = buttonHeight;

        BackButton.WidthRequest = buttonWidth * 0.8;
        BackButton.HeightRequest = buttonHeight;

        // Adjust font sizes based on button width
        double buttonFontSize = Math.Min(buttonWidth * 0.08, 18);

        SaveButton.FontSize = buttonFontSize;
        BackButton.FontSize = buttonFontSize;
    }


    private async Task UpdateWordsDisplay()
    {
        // Retrieve the option object
        var existingOption = await _storyViewModel.GetOptionByIdAsync(_storyId, _eventId, _optionId);

        // Check if the option has words and update the display
        if (existingOption != null && existingOption.Words != null && existingOption.Words.Any())
        {
            WordsDisplayLabel.Text = string.Join(" - ", existingOption.Words);
        }
        else
        {
            WordsDisplayLabel.Text = string.Empty; // Clear if no words
        }
    }

    private async void OnAddWordClicked(object sender, EventArgs e)
    {
        // Get the text from the Entry
        var newWord = OptionWordsEntry.Text;

        if (!string.IsNullOrEmpty(newWord))
        {
            // Assuming you have _storyId, _eventId, and optionId already defined
            var existingOption = await _storyViewModel.GetOptionByIdAsync(_storyId, _eventId, _optionId);

            // Add the new word to the option's word list
            if (existingOption.Words == null)
            {
                existingOption.Words = new List<string>(); // Initialize if it's null
            }
            existingOption.Words.Add(newWord);

            // Update the option in the event
            await _storyViewModel.UpdateOptionInEvent(_storyId, _eventId, existingOption);

            // Update the word list display
            UpdateWordsDisplay();

            // Clear the Entry field after adding
            OptionWordsEntry.Text = string.Empty;

            // Dismiss the keyboard
            OptionWordsEntry.Unfocus();
        }
    }

    /// <summary>
    /// Handles the click event for the Save button.
    /// Validates input and creates or updates an option in the story's event.
    /// If successful, navigates back to the EventCreationPage.
    /// </summary>
    /// <param name="sender">The source of the event (the Save button).</param>
    /// <param name="e">Event arguments.</param>
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(OptionNameEntry.Text) || !string.IsNullOrWhiteSpace(OptionTextWord.Text))
        {
            var selectedEvent = (Event)EventPicker.SelectedItem;
            if (selectedEvent.IdEvent == 0) { selectedEvent = null; }

            if (_optionId == 0)
            {
                Debug.WriteLine($"Option is new: Creating new option..");
                await _storyViewModel.AddOptionToEvent(_storyId, _eventId, new Option
                {
                    IdOption = await _storyViewModel.GenerateNewOptionId(_storyId, _eventId),
                    NameOption = OptionNameEntry.Text,
                    Text = OptionTextWord.Text,
                    LinkedEvent = selectedEvent
                });
            }
            else
            {
                Debug.WriteLine($"Option is not new: Updating option..");
                await _storyViewModel.UpdateOptionInEvent(_storyId, _eventId, new Option
                {
                    IdOption = _optionId,
                    NameOption = OptionNameEntry.Text,
                    Text = OptionTextWord.Text,
                    LinkedEvent = selectedEvent
                });
            }

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
