using Model;
using System.Diagnostics;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

public partial class OptionCreationPage : ContentPage, IQueryAttributable
{
    private readonly StoryViewModel _storyViewModel;
    private int _storyId;
    private int _eventId;
    private int _optionId;

    private Option _optionObject;

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
            await OnLoadOptionPage(_optionId);
        }
    }

    private async Task OnLoadOptionPage(int optionId)
    {
        if (optionId != 0)
        {
            var existingOption = await _storyViewModel.GetOptionByIdAsync(_storyId, _eventId, optionId);

            if (existingOption != null)
            {
                OptionNameEntry.Text = existingOption.NameOption;
                OptionTextWord.Text = existingOption.Text;

                SetSelectedEvent(existingOption.LinkedEvent?.IdEvent);

                _optionObject = existingOption;

                UpdateWordsDisplay();
            }
        }
        else
        {
            this._optionObject = new Option();
        }
    }

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

        // Check if the option has words and update the display
        if (this._optionObject.IsWordsListNotEmpty())
        {
            this.WordsDisplayLabel.Text = string.Join(" - ", this._optionObject.GetWords());
        }
        else
        {
            this.WordsDisplayLabel.Text = string.Empty; // Clear if no words
        }
    }

    private async void OnAddWordClicked(object sender, EventArgs e)
    {
        // Get the text from the Entry
        var newWord = OptionWordsEntry.Text;

        if (!string.IsNullOrEmpty(newWord))
        {

            this._optionObject.AddWordInList(newWord);

            await _storyViewModel.UpdateOptionInEvent(_storyId, _eventId, _optionObject);

            UpdateWordsDisplay();

            // Clear the Entry field after adding
            this.OptionWordsEntry.Text = string.Empty;

            // Dismiss the keyboard
            this.OptionWordsEntry.Unfocus();
        }
    }


    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(OptionNameEntry.Text) || !string.IsNullOrWhiteSpace(OptionTextWord.Text))
        {
            var selectedEvent = (Event)EventPicker.SelectedItem;
            if (selectedEvent != null && selectedEvent.IdEvent == 0) 
            { 
                selectedEvent = null; 
            }

            this._optionObject.IdOption = await _storyViewModel.GenerateNewOptionId(_storyId, _eventId);
            this._optionObject.NameOption = OptionNameEntry.Text;
            this._optionObject.Text = OptionTextWord.Text;
            this._optionObject.LinkedEvent = selectedEvent;

            if (_optionId == 0)
            {
                await _storyViewModel.AddOptionToEvent(_storyId, _eventId, _optionObject);
            }
            else
            {
                await _storyViewModel.UpdateOptionInEvent(_storyId, _eventId, _optionObject);
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
