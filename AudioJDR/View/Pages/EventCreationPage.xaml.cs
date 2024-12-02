using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

/// <summary>
/// Represents the Event Creation/Edition page that allows users to create or edit an event,
/// including its name, description, and associated options.
/// The page also handles saving changes and navigation.
/// </summary>
public partial class EventCreationPage : ContentPage, IQueryAttributable
{
    #region Fields

    private readonly StoryViewModel _storyViewModel;
    private EventViewModel _eventViewModel;
    private int _storyId;
    private int _eventId;
    private string? _initialName;
    private string? _initialDescription;
    private bool _hasUnsavedChanges;
    private bool _isNewEvent;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the EventCreationPage class.
    /// </summary>
    public EventCreationPage()
    {
        InitializeComponent();
        SetResponsiveSizes();
        InitializeEventHandlers();

        _storyViewModel = new StoryViewModel();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Applies query attributes, setting the story and event IDs and loading the appropriate data.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId") && query.ContainsKey("eventId"))
        {
            _storyId = (int)query["storyId"];
            _eventId = (int)query["eventId"];
            _isNewEvent = (_eventId == 0);
            await InitializeViewModel();
            InitializeEventTracking();
        }
        else
        {
            Debug.WriteLine("Missing required navigation parameters");
        }
    }

    private async Task InitializeViewModel()
    {
        _storyViewModel.CurrentStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
            
        if (_isNewEvent)
        {
            Debug.WriteLine($"Initializing new event");
            _eventViewModel = new EventViewModel(_storyViewModel);
            await _eventViewModel.InitializeNewEventAsync();
        }
        else
        {
            Debug.WriteLine($"Loading event with ID: {_eventId}");
            _eventViewModel = await _storyViewModel.GetEventViewModelAsync(_eventId);
        }

        BindingContext = _eventViewModel;
    }

    private void InitializeEventHandlers()
    {
        this.SizeChanged += OnSizeChanged;
        EventNameEntry.TextChanged += OnEventPropertyChanged;
        EventDescriptionEditor.TextChanged += OnEventPropertyChanged;
    }

    private void InitializeEventTracking()
    {
        if (_isNewEvent)
        {
            _initialName = string.Empty;
            _initialDescription = string.Empty;
        }
        else
        {
            _initialName = _eventViewModel.CurrentEvent.Name;
            _initialDescription = _eventViewModel.CurrentEvent.Description;
        }
        _hasUnsavedChanges = false;
    }

    #endregion

    #region Event Handlers

    private void OnEventPropertyChanged(object? sender, TextChangedEventArgs e)
    {
        _hasUnsavedChanges = HasEventChanged();
    }

    private async void OnCreateNewOptionButtonClicked(object sender, EventArgs e)
    {
        bool canProceed = await CheckUnsavedChanges();
        
        if (canProceed)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "storyId", _storyId },
                { "eventId", _eventViewModel.CurrentEvent.IdEvent },
                { "optionId", 0 }
            };
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}", navigationParameter);
        }
    }

    private async void OnEditOptionButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            bool canProceed = await CheckUnsavedChanges();
            
            if (canProceed)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "storyId", _storyId },
                    { "eventId", _eventId },
                    { "optionId", selectedOption.IdOption }
                };
                await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}", navigationParameter);
            }
        }
    }

    private async void OnDeleteOptionButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option selectedOption)
        {
            bool confirmed = await UIHelper.ShowDeleteConfirmationDialog(this, selectedOption.NameOption);
            
            if (confirmed)
            {
                try 
                {
                    await _eventViewModel.DeleteOptionAsync(selectedOption.IdOption);
                    await UIHelper.ShowSuccessDialog(this, string.Format(AppResources.DeleteSuccessFormat, selectedOption.NameOption));
                    RefreshOptionList();
                }
                catch (Exception ex)
                {
                    await UIHelper.ShowErrorDialog(this, ex.Message);
                    Debug.WriteLine($"Error deleting option: {ex.Message}");
                }
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
                { "storyId", _storyId }
            };
            await Shell.Current.GoToAsync($"{nameof(StoryMap)}", navigationParameter);
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await SaveEventChanges();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    #endregion

    #region Event Operations

    private bool HasEventChanged()
    {
        bool hasChanged = EventNameEntry.Text != _initialName || 
                         EventDescriptionEditor.Text != _initialDescription;
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
                canProceed = await SaveEventChanges();
            }
        }
        
        return canProceed;
    }

    private async Task<bool> SaveEventChanges()
    {
        bool success = false;
        
        try
        {
            _eventViewModel.CurrentEvent.Name = EventNameEntry.Text;
            _eventViewModel.CurrentEvent.Description = EventDescriptionEditor.Text;
            
            await _eventViewModel.UpdateEventAsync(_eventViewModel.CurrentEvent);
            
            _initialName = EventNameEntry.Text;
            _initialDescription = EventDescriptionEditor.Text;
            _hasUnsavedChanges = false;
            success = true;
            await UIHelper.ShowSuccessDialog(this, AppResources.SaveSuccessMessage);
        }
        catch (Exception ex)
        {
            await UIHelper.ShowErrorDialog(this, ex.Message);
            Debug.WriteLine($"Error saving event changes: {ex.Message}");
        }
        
        return success;
    }

    private void RefreshOptionList()
    {
        OptionList.ItemsSource = null;
        OptionList.ItemsSource = _eventViewModel.CurrentEvent.Options;
    }

    #endregion

    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        // Set button sizes dynamically using UIHelper
        if (pageWidth > 0 && pageHeight > 0)
        {
            UIHelper.SetButtonSize(this, SaveButton, false); 
            UIHelper.SetButtonSize(this, CreateNewOptionButton, false); 
            UIHelper.SetButtonSize(this, BackButton, true); 

            // Set frame widths
            double frameWidth = UIHelper.GetResponsiveFrameWidth(pageWidth);
            EventNameEntry.WidthRequest = frameWidth;
            EventDescriptionEditor.WidthRequest = frameWidth;
            OptionList.WidthRequest = frameWidth;
        }
    }

    #endregion
}
