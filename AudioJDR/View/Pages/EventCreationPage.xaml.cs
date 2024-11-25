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
    private readonly StoryViewModel _storyViewModel;
    private EventViewModel _eventViewModel;
    private int _storyId;
    private int _eventId;
    private string _initialName;
    private string _initialDescription;
    private ObservableCollection<Option> _initialOptions;

    /// <summary>
    /// Initializes a new instance of the EventCreationPage.
    /// </summary>
    public EventCreationPage()
    {
        InitializeComponent();
        _storyViewModel = new StoryViewModel();
        SetResponsiveSizes();
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        SetResponsiveSizes();
    }

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;

        if (pageWidth > 0 && pageHeight > 0)
        {
            double buttonWidth = Math.Max(pageWidth * 0.24, 350);
            double buttonHeight = Math.Max(pageHeight * 0.08, 50);

            EventNameEntry.WidthRequest = Math.Max(pageWidth * 0.7, 300);
            EventDescriptionEditor.WidthRequest = Math.Max(pageWidth * 0.7, 300);
            OptionList.WidthRequest = Math.Max(pageWidth * 0.9, 300);

            SaveButton.WidthRequest = buttonWidth;
            SaveButton.HeightRequest = buttonHeight;

            CreateNewOptionButton.WidthRequest = buttonWidth;
            CreateNewOptionButton.HeightRequest = buttonHeight;

            BackButton.WidthRequest = buttonWidth * 0.8;
            BackButton.HeightRequest = buttonHeight;

            double buttonFontSize = Math.Min(buttonWidth * 0.08, 18);
            SaveButton.FontSize = buttonFontSize;
            CreateNewOptionButton.FontSize = buttonFontSize;
            BackButton.FontSize = buttonFontSize;
        }
    }

    /// <summary>
    /// Applies query attributes, setting the story and event IDs and loading the appropriate data.
    /// </summary>
    /// <param name="query">Dictionary of query parameters containing storyId and eventId.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId") && query.ContainsKey("eventId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _eventId = int.Parse(query["eventId"].ToString());
            Debug.WriteLine($"Received storyId: {_storyId}, eventId: {_eventId}");

            try
            {
                _storyViewModel.CurrentStory = await _storyViewModel.GetStoryByIdAsync(_storyId);
                _eventViewModel = _eventId == 0 
                    ? new EventViewModel(_storyViewModel)
                    : await _storyViewModel.GetEventViewModelAsync(_eventId);

                BindingContext = _eventViewModel;
                InitializeEventState();
                
                Debug.WriteLine(_eventId == 0 
                    ? "Initialized new event creation." 
                    : $"Event loaded: {_eventViewModel.CurrentEvent.Name}, Options count: {_eventViewModel.CurrentEvent.Options.Count}");
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine($"Error loading story or event: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Missing required navigation parameters");
        }
    }

    private void InitializeEventState()
    {
        EventNameEntry.Text = _eventViewModel.CurrentEvent.Name;
        EventDescriptionEditor.Text = _eventViewModel.CurrentEvent.Description;
        OptionList.ItemsSource = _eventViewModel.CurrentEvent.Options;

        _initialName = _eventViewModel.CurrentEvent.Name;
        _initialDescription = _eventViewModel.CurrentEvent.Description;
        _initialOptions = new ObservableCollection<Option>(_eventViewModel.CurrentEvent.Options);
    }

    private async Task UpdateEvent()
    {
        if (_eventViewModel.CurrentEvent != null)
        {
            _eventViewModel.CurrentEvent.Name = EventNameEntry.Text;
            _eventViewModel.CurrentEvent.Description = EventDescriptionEditor.Text;
            await _eventViewModel.UpdateEventAsync(_eventViewModel.CurrentEvent);
            Debug.WriteLine("Event updated successfully.");
        }
    }

    private void RefreshOptionList()
    {
        OptionList.ItemsSource = null;
        OptionList.ItemsSource = _eventViewModel.CurrentEvent.Options;
    }

    private async Task<bool> ConfirmAndSaveIfNecessary()
    {
        bool hasChanges = _initialName != EventNameEntry.Text ||
                         _initialDescription != EventDescriptionEditor.Text ||
                         !_initialOptions.SequenceEqual(_eventViewModel.CurrentEvent.Options);

        bool result = true;
        if (hasChanges)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DiscardChangesMessage, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                await UpdateEvent();
            }
            else
            {
                result = false;
            }
        }
        return result;
    }

    private async void OnEditOptionButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option currentOption)
        {
            Debug.WriteLine($"Edit Option Clicked: Option ID: {currentOption.IdOption}");
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId={currentOption.IdOption}");
        }
    }

    private async void OnDeleteOptionButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Option currentOption)
        {
            bool confirm = await DisplayAlert(AppResources.Confirm, AppResources.DeleteOptionConfirmationText, AppResources.Yes, AppResources.No);
            if (confirm)
            {
                var optionViewModel = new OptionViewModel(_eventViewModel, currentOption);
                await optionViewModel.DeleteOptionAsync(currentOption.IdOption);
                RefreshOptionList();
            }
        }
    }

    private async void OnCreateNewOptionButtonClicked(object sender, EventArgs e)
    {
        bool shouldProceed = await ConfirmAndSaveIfNecessary();
        if (shouldProceed)
        {
            Debug.WriteLine($"Create New Option Clicked: Event ID: {_eventId}");
            await Shell.Current.GoToAsync($"{nameof(OptionCreationPage)}?storyId={_storyId}&eventId={_eventId}&optionId=0");
        }
        else
        {
            Debug.WriteLine("Navigation canceled by user.");
        }
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await UpdateEvent();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        bool shouldProceed = await ConfirmAndSaveIfNecessary();
        if (shouldProceed)
        {
            await Shell.Current.GoToAsync($"{nameof(StoryMap)}?storyId={_storyId}");
        }
    }
}
