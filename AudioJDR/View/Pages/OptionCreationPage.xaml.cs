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
    private Event _initialLinkedEvent;
    private bool _hasUnsavedChanges;
    private bool _isNewOption;
    private bool _isInitializing = true;

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
        InitializeEventHandlers();
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
            _storyId = (int)query["storyId"];
            _eventId = (int)query["eventId"];
            _optionId = (int)query["optionId"];
            _isNewOption = (_optionId == 0);
            
            await InitializeViewModel();
            InitializeOptionState();
            PopulateEventPicker();
        }
        else
        {
            Debug.WriteLine("Missing required navigation parameters");
        }
    }

    private async Task InitializeViewModel()
    {
        try
        {
            _storyViewModel.CurrentStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
            _eventViewModel = await _storyViewModel.GetEventViewModelAsync(_eventId);
            
            if (_isNewOption)
            {
                _optionViewModel = new OptionViewModel(_eventViewModel);
                await _optionViewModel.InitializeNewOptionAsync();
            }
            else
            {
                _optionViewModel = await _eventViewModel.GetOptionViewModelAsync(_optionId);
                Debug.WriteLine($"Loaded option with ID: {_optionId}");
            }

            BindingContext = _optionViewModel;
            UpdateWordsDisplay();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing view model: {ex.Message}");
            await UIHelper.ShowErrorDialog(this, ex.Message);
        }
    }

    private void InitializeEventHandlers()
    {
        this.SizeChanged += OnSizeChanged;
        OptionNameEntry.TextChanged += OnOptionPropertyChanged;
        EventPicker.SelectedIndexChanged += OnOptionPropertyChanged;
    }

    private void InitializeOptionState()
    {
        _isInitializing = true;
        
        OptionNameEntry.Text = _optionViewModel.CurrentOption.NameOption;
        UpdateWordsDisplay();

        _initialName = _optionViewModel.CurrentOption.NameOption;
        _initialLinkedEvent = _optionViewModel.CurrentOption.LinkedEvent;
        _hasUnsavedChanges = false;

        _isInitializing = false;
    }

    private void PopulateEventPicker()
    {
        int? currentEventId;

        // Check if the option is new
        if (_isNewOption)
        {
            // If the option is new, use the CurrentEvent from the EventViewModel
            currentEventId = _eventViewModel.CurrentEvent?.IdEvent;
        }
        else
        {
            // If the option is not new, use the LinkedEvent from the CurrentOption
            currentEventId = _optionViewModel.CurrentOption.LinkedEvent?.IdEvent;
        }

        var currentEvents = _storyViewModel.CurrentStory.Events;
        EventPicker.ItemsSource = currentEvents;

        // Set the selected event based on the currentEventId
        SetSelectedEvent(currentEventId);
    }

    private void SetSelectedEvent(int? linkedEventId = null)
    {
        var events = _storyViewModel.CurrentStory.Events;

        if (linkedEventId == null)
        {
            // If linkedEventId is null, select the CurrentEvent from the EventViewModel
            EventPicker.SelectedItem = _eventViewModel.CurrentEvent;
        }
        else
        {
            // Try to find the event with the matching ID in the ItemsSource
            var selectedEvent = events.FirstOrDefault(e => e.IdEvent == linkedEventId);

            // If no matching event is found, fall back to the CurrentEvent
            EventPicker.SelectedItem = selectedEvent ?? _eventViewModel.CurrentEvent;
        }
    }

    #endregion

    #region Event Handlers

    private void OnOptionPropertyChanged(object sender, EventArgs e)
    {
        if (!_isInitializing && _optionViewModel?.CurrentOption != null && _initialName != null)
        {
            var currentEvent = EventPicker.SelectedItem as Event;
            bool nameChanged = OptionNameEntry.Text != _initialName;
            bool eventChanged = currentEvent?.IdEvent != _initialLinkedEvent?.IdEvent;

            _hasUnsavedChanges = nameChanged || eventChanged;
            Debug.WriteLine($"Changes detected - Name changed: {nameChanged}, Event changed: {eventChanged}");
        }
    }


    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        bool canSave = await ValidateOptionInput();
        
        if (canSave)
        {
            await SaveOptionChanges();
        }
    }

    private async void OnAddWordClicked(object sender, EventArgs e)
    {
        string newWord = OptionWordsEntry.Text.Trim();

        if (!string.IsNullOrWhiteSpace(newWord))
        {
            try
            {
                // Check if the word already exists
                bool exists = _optionViewModel.Words.Contains(newWord);

                if (exists)
                {
                    // If it exists, remove the word
                    await _optionViewModel.RemoveWordAsync(newWord);
                    await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.DeletedFormat, newWord));
                }
                else
                {
                    // If it doesn't exist, add the word
                    await _optionViewModel.AddWordAsync(newWord);
                    await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.AddedFormat, newWord));
                }

                // Update the display and clear the input
                UpdateWordsDisplay();
                OptionWordsEntry.Text = string.Empty;
                _hasUnsavedChanges = true;
            }
            catch (Exception ex)
            {
                await UIHelper.ShowErrorDialog(this, string.Format(AppResources.WordChangeErrorFormat, newWord));
                Debug.WriteLine($"Error processing word: {ex.Message}");
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        bool canProceed = await CheckUnsavedChanges();
        
        if (canProceed)
        {
            var navigationParameter = new Dictionary<string, object>
        {
            { "storyId", _storyId },
            { "eventId", _eventId }
        };
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}", navigationParameter);
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    #endregion

    #region Option Operations

    private async Task<bool> CheckUnsavedChanges()
    {
        bool canProceed = true;
        
        if (_hasUnsavedChanges)
        {
            bool saveChanges = await UIHelper.ShowUnsavedChangesDialog(this);

            if (saveChanges)
            {
                canProceed = await SaveOptionChanges();
            }
        }
        
        return canProceed;
    }

    private async Task<bool> SaveOptionChanges()
    {
        bool success = false;
        
        try
        {
            _optionViewModel.CurrentOption.NameOption = OptionNameEntry.Text;
            _optionViewModel.CurrentOption.LinkedEvent = (Event)EventPicker.SelectedItem;
            
            await _optionViewModel.UpdateOptionAsync(_optionViewModel.CurrentOption);
            
            _initialName = OptionNameEntry.Text;
            _initialLinkedEvent = (Event)EventPicker.SelectedItem;
            _hasUnsavedChanges = false;
            
            success = true;
            await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.SaveSuccessFormat, OptionNameEntry.Text));
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, string.Format(AppResources.SaveErrorFormat, OptionNameEntry.Text));
            Debug.WriteLine($"Error saving option changes: {ex.Message}");
        }
        
        return success;
    }

    private async Task<bool> ValidateOptionInput()
    {
        bool isValid = !string.IsNullOrWhiteSpace(OptionNameEntry.Text);
        
        if (!isValid)
        {
            await UIHelper.ShowErrorDialog(this, AppResources.ErrorOptionTitle);
        }
        
        return isValid;
    }

    private void UpdateWordsDisplay()
    {
        var words = _optionViewModel?.Words;
        WordsDisplayLabel.Text = words != null && words.Any() 
            ? string.Join(" - ", words)
            : AppResources.NoWordsAdded;
        Debug.WriteLine($"Updated words display: {WordsDisplayLabel.Text}");
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        // Use the current page size to set button sizes dynamically
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set button sizes dynamically using UIHelper
        if (pageWidth > 0 && pageHeight > 0)
        {
            UIHelper.SetButtonSize(this, SaveButton, false);
            UIHelper.SetButtonSize(this, BackButton, true);
            UIHelper.SetButtonSize(this, AddWordButton, false); 

            double frameWidth = UIHelper.GetResponsiveFrameWidth(pageWidth);
            OptionNameEntry.WidthRequest = frameWidth;
            EventPicker.WidthRequest = frameWidth;
            OptionWordsEntry.WidthRequest = frameWidth;
        }
    }

    #endregion
}
