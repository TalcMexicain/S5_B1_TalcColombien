using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

/// <summary>
/// Represents the Option Creation/Edition page that allows users to create or edit an option,
/// including its name, text, words list, and linked event.
/// The page also handles saving changes and navigation.
/// </summary>
public partial class OptionCreationPage : ContentPage, IQueryAttributable
{
    #region Fields

    private readonly StoryViewModel _storyViewModel;
    private EventViewModel _eventViewModel;
    private OptionViewModel _optionViewModel;
    private int _storyId;
    private int _eventId;
    private int _optionId;
    private string _initialName;
    private string _initialText;
    private Event _initialLinkedEvent;
    private ObservableCollection<string> _initialWords;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the OptionCreationPage.
    /// </summary>
    public OptionCreationPage()
    {
        InitializeComponent();
        _storyViewModel = new StoryViewModel();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Applies query attributes, setting the story, event, and option IDs and loading the appropriate data.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId") && query.ContainsKey("eventId") && query.ContainsKey("optionId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _eventId = int.Parse(query["eventId"].ToString());
            _optionId = int.Parse(query["optionId"].ToString());
            Debug.WriteLine($"Received storyId: {_storyId}, eventId: {_eventId}, optionId: {_optionId}");

            try
            {
                _storyViewModel.CurrentStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
                _eventViewModel = await _storyViewModel.GetEventViewModelAsync(_eventId);
                
                _optionViewModel = _optionId == 0
                    ? new OptionViewModel(_eventViewModel)
                    : new OptionViewModel(_eventViewModel, _eventViewModel.CurrentEvent.Options.First(o => o.IdOption == _optionId));

                BindingContext = _optionViewModel;
                InitializeOptionState();
                PopulateEventPicker();
                
                Debug.WriteLine(_optionId == 0 
                    ? "Initialized new option creation." 
                    : $"Option loaded: {_optionViewModel.CurrentOption.NameOption}");
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine($"Error loading story, event, or option: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Missing required navigation parameters");
        }
    }

    private void InitializeOptionState()
    {
        OptionNameEntry.Text = _optionViewModel.CurrentOption.NameOption;
        OptionTextWord.Text = _optionViewModel.CurrentOption.Text;
        UpdateWordsDisplay();

        _initialName = _optionViewModel.CurrentOption.NameOption;
        _initialText = _optionViewModel.CurrentOption.Text;
        _initialLinkedEvent = _optionViewModel.CurrentOption.LinkedEvent;
        _initialWords = new ObservableCollection<string>(_optionViewModel.Words);
    }

    private void PopulateEventPicker()
    {
        var currentEventId = _eventViewModel.CurrentEvent.IdEvent;
        var filteredEvents = _storyViewModel.CurrentStory.Events
            .Where(e => e.IdEvent != currentEventId)
            .ToList();

        filteredEvents.Insert(0, new Event { IdEvent = 0, Name = AppResources.None });
        EventPicker.ItemsSource = filteredEvents;
        
        SetSelectedEvent(_optionViewModel.CurrentOption.LinkedEvent?.IdEvent);
    }

    private void SetSelectedEvent(int? linkedEventId)
    {
        var events = (List<Event>)EventPicker.ItemsSource;
        EventPicker.SelectedItem = events.FirstOrDefault(e => e.IdEvent == (linkedEventId ?? 0));
    }

    #endregion

    #region Event Handlers

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        bool canSave = ValidateOptionInput();
        
        if (canSave)
        {
            await UpdateOption();
            await NavigateBack();
        }
    }

    private async void OnAddWordClicked(object sender, EventArgs e)
    {
        string newWord = OptionWordsEntry.Text?.Trim();
        bool canAddWord = !string.IsNullOrEmpty(newWord);
        
        if (canAddWord)
        {
            bool added = await _optionViewModel.AddWordAsync(newWord);
            HandleWordAddResult(added);
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        bool shouldProceed = await ConfirmAndSaveIfNecessary();
        if (shouldProceed)
        {
            await NavigateBack();
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    #endregion

    #region Data Operations

    private async Task UpdateOption()
    {
        if (_optionViewModel.CurrentOption != null)
        {
            _optionViewModel.CurrentOption.NameOption = OptionNameEntry.Text;
            _optionViewModel.CurrentOption.Text = OptionTextWord.Text;
            _optionViewModel.CurrentOption.LinkedEvent = (Event)EventPicker.SelectedItem;
            await _optionViewModel.UpdateOptionAsync(_optionViewModel.CurrentOption);
            Debug.WriteLine("Option updated successfully.");
        }
    }

    private bool ValidateOptionInput()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(OptionNameEntry.Text) || string.IsNullOrWhiteSpace(OptionTextWord.Text))
        {
            DisplayAlert(AppResources.Error, AppResources.ErrorOptionTitleDesc, "OK");
            isValid = false;
        }

        return isValid;
    }

    private async void HandleWordAddResult(bool added)
    {
        if (added)
        {
            UpdateWordsDisplay();
            OptionWordsEntry.Text = string.Empty;
        }
        else
        {
            await DisplayAlert(AppResources.Error, AppResources.WordAlreadyExists, "OK");
        }
    }

    private async Task<bool> ConfirmAndSaveIfNecessary()
    {
        bool result = true;
        bool hasChanges = CheckForChanges();

        if (hasChanges)
        {
            result = await HandleUnsavedChanges();
        }

        return result;
    }

    private bool CheckForChanges()
    {
        return _initialName != OptionNameEntry.Text ||
               _initialText != OptionTextWord.Text ||
               _initialLinkedEvent != (Event)EventPicker.SelectedItem ||
               !_initialWords.SequenceEqual(_optionViewModel.Words);
    }

    private async Task<bool> HandleUnsavedChanges()
    {
        bool result = true;
        bool confirm = await DisplayAlert(AppResources.Confirm, 
                                        AppResources.DiscardChangesMessage, 
                                        AppResources.Yes, 
                                        AppResources.No);
        
        if (confirm)
        {
            await UpdateOption();
        }
        else
        {
            result = false;
        }

        return result;
    }

    private void UpdateWordsDisplay()
    {
        WordsDisplayLabel.Text = _optionViewModel.Words.Any()
            ? string.Join(" - ", _optionViewModel.Words)
            : string.Empty;
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth <= 0 || pageHeight <= 0) return;

        double minButtonWidth = 150;
        double minButtonHeight = 50;
        double minFrameWidth = 300;
        double minEditorHeight = 200;

        double buttonWidth = Math.Max(pageWidth * 0.25, minButtonWidth);
        double buttonHeight = Math.Max(pageHeight * 0.08, minButtonHeight);
        double frameWidth = Math.Max(pageWidth * 0.8, minFrameWidth);
        double editorHeight = Math.Max(pageHeight * 0.15, minEditorHeight);

        OptionWordsFrame.WidthRequest = frameWidth;
        WordsDisplayLabel.FontSize = Math.Min(frameWidth * 0.05, 20);
        OptionWordsEntry.WidthRequest = frameWidth * 0.9;
        OptionWordsEntry.FontSize = Math.Min(frameWidth * 0.05, 18);

        AddWordButton.WidthRequest = buttonWidth;
        AddWordButton.HeightRequest = buttonHeight;
        AddWordButton.FontSize = Math.Min(buttonWidth * 0.08, 18);

        OptionNameFrame.WidthRequest = frameWidth;
        OptionTextFrame.WidthRequest = frameWidth;
        EventPickerFrame.WidthRequest = frameWidth;

        OptionTextWord.HeightRequest = editorHeight;

        SaveButton.WidthRequest = buttonWidth;
        SaveButton.HeightRequest = buttonHeight;

        BackButton.WidthRequest = buttonWidth * 0.8;
        BackButton.HeightRequest = buttonHeight;

        double buttonFontSize = Math.Min(buttonWidth * 0.08, 18);
        SaveButton.FontSize = buttonFontSize;
        BackButton.FontSize = buttonFontSize;
    }

    #endregion

    #region Transitions

    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}?storyId={_storyId}&eventId={_eventId}");
    }

    #endregion
}
