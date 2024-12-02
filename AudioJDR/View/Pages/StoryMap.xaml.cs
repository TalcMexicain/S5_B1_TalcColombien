using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Pages;
using View.Resources.Localization;
using ViewModel;
using System.Globalization;

namespace View;

/// <summary>
/// Represents the StoryMap page that allows users to view and edit a specific story,
/// including its title, description, and associated events.
/// The page also handles saving changes, navigating to event creation, and event editing.
/// </summary>
public partial class StoryMap : ContentPage, IQueryAttributable
{
    #region Fields

    private StoryViewModel _storyViewModel;
    private int _storyId;
    private string? _currentTitle;
    private string? _currentDescription;
    private bool _hasUnsavedChanges;
    private bool _isNewStory;
    private Event? _currentFirstEvent; // Track the current first event

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the MainPage class. 
    /// </summary>
    public StoryMap()
    {
        InitializeComponent();
        SetResponsiveSizes();
        InitializeEventHandlers();

        _storyViewModel = new StoryViewModel();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Applies query attributes, setting the story ID and loading the appropriate data.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId"))
        {
            _storyId = (int)query["storyId"];
            _isNewStory = (_storyId == 0);
            await InitializeViewModel();
            InitializeStoryTracking();
        }
        else
        {
            Debug.WriteLine("Missing required navigation parameters");
        }
    }

    private async Task InitializeViewModel()
    {  
        if (_isNewStory)
        {
            Debug.WriteLine($"Initializing new story");
            await _storyViewModel.InitializeNewStoryAsync();
        }
        else
        {
            Debug.WriteLine($"Loading story with ID: {_storyId}");
            await _storyViewModel.LoadStoriesAsync();
            _storyViewModel.CurrentStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
            
            // Populate Events for the current story
            _storyViewModel.Events.Clear(); // Clear existing events
            foreach (var evt in _storyViewModel.CurrentStory.Events)
            {
                Debug.WriteLine($"Loaded event: {evt.Name} (ID: {evt.IdEvent})");
                _storyViewModel.Events.Add(new EventViewModel(_storyViewModel, evt));
            }
        }

        BindingContext = _storyViewModel;
    }

    private void InitializeEventHandlers()
    {
        this.SizeChanged += OnSizeChanged;
        StoryNameEntry.TextChanged += OnStoryPropertyChanged;
        StoryDescriptionEditor.TextChanged += OnStoryPropertyChanged;
    }

    private void InitializeStoryTracking()
    {
        if (_isNewStory)
        {
            _currentTitle = string.Empty;
            _currentDescription = string.Empty;
            _currentFirstEvent = null; // No first event for a new story
        }
        else
        {
            _currentTitle = _storyViewModel.CurrentStory.Title;
            _currentDescription = _storyViewModel.CurrentStory.Description;
            _currentFirstEvent = _storyViewModel.CurrentStory.FirstEvent; // Track the current first event
        }
        _hasUnsavedChanges = false;
    }

    #endregion

    #region Event Handlers

    private async void OnCreateNewEventButtonClicked(object sender, EventArgs e)
    {
        bool canProceed = await CheckUnsavedChanges();
        
        if (canProceed)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "storyId", _storyViewModel.CurrentStory.IdStory },
                { "eventId", 0 }
            };
            await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}", navigationParameter);
        }
    }

    private async void OnEditEventButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Event selectedEvent)
        {
            bool canProceed = await CheckUnsavedChanges();
            
            if (canProceed)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "storyId", _storyId },
                    { "eventId", selectedEvent.IdEvent }
                };
                await Shell.Current.GoToAsync($"{nameof(EventCreationPage)}", navigationParameter);
            }
        }
    }

    private async void OnDeleteEventButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Event selectedEvent)
        {
            bool confirmed = await UIHelper.ShowDeleteConfirmationDialog(this, selectedEvent.Name);
            
            if (confirmed)
            {
                try
                {
                    await _storyViewModel.DeleteEventAsync(selectedEvent.IdEvent);
                    await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.DeleteSuccessFormat, selectedEvent.Name));
                    RefreshEventList();
                }
                catch (Exception ex)
                {
                    await UIHelper.ShowErrorDialog(this, ex.Message);
                    Debug.WriteLine($"Error deleting event: {ex.Message}");
                }
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        bool canProceed = await CheckUnsavedChanges();
        
        if (canProceed)
        {
            await Shell.Current.GoToAsync($"{nameof(StoryList)}");
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await SaveStoryChanges();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private void OnStoryPropertyChanged(object? sender, TextChangedEventArgs e)
    {
        _hasUnsavedChanges = HasStoryChanged();
    }

    private async void OnSetAsFirstEventButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Event selectedEvent)
        {
            try
            {
                // Check if the first event is changing
                if (_currentFirstEvent != selectedEvent)
                {
                    _hasUnsavedChanges = true; // Mark as unsaved changes
                }

                _storyViewModel.SetFirstEvent(selectedEvent.IdEvent);
                _currentFirstEvent = selectedEvent; // Update the current first event
                await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.SetFirstEventSuccessMessage, selectedEvent.Name));
                RefreshEventList();
            }
            catch (Exception ex)
            {
                await UIHelper.ShowErrorDialog(this, ex.Message);
                Debug.WriteLine($"Error setting first event: {ex.Message}");
            }
        }
    }

    #endregion

    #region Story Operations

    private bool HasStoryChanged()
    {
        bool hasChanged = StoryNameEntry.Text != _currentTitle || 
                         StoryDescriptionEditor.Text != _currentDescription;
        return hasChanged;
    }

    private async Task<bool> CheckUnsavedChanges()
    {
        bool canProceed = true;
        
        if (_hasUnsavedChanges)
        {
            bool saveChanges = await UIHelper.ShowUnsavedChangesDialog(this);

            if (saveChanges)
            {
                canProceed = await SaveStoryChanges();
            }
        }
        
        return canProceed;
    }

    private async Task<bool> SaveStoryChanges()
    {
        bool success = false;
        
        try
        {
            _storyViewModel.CurrentStory.Title = StoryNameEntry.Text;
            _storyViewModel.CurrentStory.Description = StoryDescriptionEditor.Text;
            
            await _storyViewModel.UpdateStoryAsync(
                _storyViewModel.CurrentStory.IdStory, 
                _storyViewModel.CurrentStory
            );
            
            _currentTitle = StoryNameEntry.Text;
            _currentDescription = StoryDescriptionEditor.Text;
            _hasUnsavedChanges = false;
            success = true;
            
            await UIHelper.ShowSuccessDialog(this, AppResources.SaveSuccessMessage);
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, ex.Message);
            Debug.WriteLine($"Error saving story changes: {ex.Message}");
        }
        
        return success;
    }

    private void RefreshEventList()
    {
        EventList.ItemsSource = null;
        EventList.ItemsSource = _storyViewModel.CurrentStory.Events;
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth > 0 && pageHeight > 0)
        {

            // Set button sizes dynamically using UIHelper
            UIHelper.SetButtonSize(this, SaveButton, false);
            UIHelper.SetButtonSize(this, CreateNewEventButton, false);
            UIHelper.SetButtonSize(this, BackButton, true);

            // Set frame widths
            double frameWidth = UIHelper.GetResponsiveFrameWidth(pageWidth);
            StoryNameEntry.WidthRequest = frameWidth;
            StoryDescriptionEditor.WidthRequest = frameWidth;
            EventList.WidthRequest = frameWidth;
        }
    }

    #endregion
}
