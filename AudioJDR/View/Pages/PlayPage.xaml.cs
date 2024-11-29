using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using View.Pages;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

/// <summary>
/// The PlayPage class displays events and their related options.
/// </summary>
public partial class PlayPage : ContentPage, IQueryAttributable
{
    private readonly SpeechRecognitionViewModel _speechViewModel; // Nouveau ViewModel pour la reconnaissance vocale
    private StoryViewModel _viewModel; // ViewModel to manage story and event data
    private int _storyId; // ID of the current story
    private int _eventId; // ID of the current event
    private Story _currentStory;

    /// <summary>
    /// Initializes a new instance of the PlayPage class.
    /// </summary>
    public PlayPage()
    {
        InitializeComponent();

        _viewModel = new StoryViewModel();
        BindingContext = _speechViewModel;

        // Instanciation du ViewModel pour la reconnaissance vocale
        _speechViewModel = new SpeechRecognitionViewModel();

        // Abonnement aux événements du ViewModel
        _speechViewModel.OptionSubmitted += OnOptionSubmitted;
        _speechViewModel.AddWordsToView += AddWordsToView;
        _speechViewModel.TextCleared += () =>
        {
            OptionEntry.Text = string.Empty; // Efface le champ texte lorsque "annuler" est reconnu
        };


    }

    /// <summary>
    /// Applies query attributes received during navigation.
    /// </summary>
    /// <param name="query">Dictionary of query parameters.</param>
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("storyId") && query.ContainsKey("eventId"))
        {
            _storyId = int.Parse(query["storyId"].ToString());
            _eventId = int.Parse(query["eventId"].ToString());
            await LoadEvent(_storyId, _eventId);
        }
    }

    public async void AddWordsToView()
    {
        OptionEntry.Text = string.Empty;
        OptionEntry.Text += _speechViewModel.RecognizedText;
    }

    /// <summary>
    /// Loads the event and its options by story ID and event ID.
    /// </summary>
    /// <param name="storyId">The ID of the story.</param>
    /// <param name="eventId">The ID of the event.</param>
    private async Task LoadEvent(int storyId, int eventId)
    {
        var keywords = new HashSet<string>();
        _currentStory = await _viewModel.GetStoryByIdAsync(storyId); // Store the story in the private field
        var eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);
        Debug.WriteLine(eventToShow.Name);

        if (eventToShow != null)
        {
            EventTitleLabel.Text = eventToShow.Name;
            EventDescriptionLabel.Text = eventToShow.Description;

            foreach (var option in eventToShow.Options)
            {
                // Retrieve the keywords for the option
                var optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();

                foreach (var word in optionWords)
                {
                    keywords.Add(word); // Add the keyword to the list (no duplicates thanks to the HashSet)
                }
            }

            // Add words to recognition
            keywords.Add("valider");
            keywords.Add("annuler");

            _speechViewModel.StartRecognition(keywords);
        }
    }



    /// <summary>
    /// Handles the submission of the user's input and attempts to find the best matching option for the current event.
    /// Navigates to the corresponding event if a match is found.
    /// </summary>
    private async void OnOptionSubmitted()
    {
        string userInput = OptionEntry?.Text?.Trim().ToLower();

        if (string.IsNullOrEmpty(userInput))
        {
            await DisplayAlert("Error", "Please enter an option.", "OK");
            Debug.WriteLine("test2");
            return;
        }

        var userWords = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = GetBestMatchingOptionForCurrentEvent(userWords);

        if (result.TiedOptions.Count > 1)
        {
            await HandleTiedOptions(result.TiedOptions);
        }
        else if (result.BestMatchingOption != null && result.LinkedEvent != null)
        {
            await NavigateToEvent(result.LinkedEvent.IdEvent);
        }
        else
        {
            await DisplayAlert("Error", "No matching option or linked event found.", "OK");
        }
    }


    private (Option? BestMatchingOption, Event? LinkedEvent, List<Option> TiedOptions) GetBestMatchingOptionForCurrentEvent(string[] userWords)
    {
        Option bestMatchingOption = null;
        Event linkedEvent = null;
        double bestScore = 0.0;
        List<Option> tiedOptions = new List<Option>();
        var currentEvent = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == _eventId);

        if (currentEvent == null)
            return (null, null, tiedOptions);

        foreach (var option in currentEvent.Options)
        {
            var optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();

            if (optionWords.Length > 0)
            {
                double matchScore = CalculateMatchScore(userWords, optionWords);

                if (matchScore > bestScore)
                {
                    bestScore = matchScore;
                    bestMatchingOption = option;
                    tiedOptions.Clear();
                    tiedOptions.Add(option);
                    linkedEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == option.LinkedEvent?.IdEvent);
                }
                else if (matchScore == bestScore && matchScore > 0.0)
                {
                    tiedOptions.Add(option);
                }
            }
        }

        return (bestMatchingOption, linkedEvent, tiedOptions);
    }

    private double CalculateMatchScore(string[] userWords, string[] optionWords)
    {
        int matchCount = 0;

        foreach (var word in userWords)
        {
            if (optionWords.Contains(word))
            {
                matchCount++;
            }
        }

        return (double)matchCount / optionWords.Length;
    }

    private async Task HandleTiedOptions(List<Option> tiedOptions)
    {
        string optionsList = string.Join("\n", tiedOptions.Select(o => $"- {o.NameOption}"));
        await DisplayAlert(
            "Clarification Needed",
            $"Multiple options match your input:\n{optionsList}\nPlease be more specific.",
            "OK"
        );
    }

    private async Task NavigateToEvent(int eventId)
    {
        await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={_storyId}&eventId={eventId}");
    }

    /// <summary>
    /// Overrides the OnDisappearing method to perform automatic saving of the current game state.
    /// This ensures that when the user navigates away from the current page, the current event
    /// of the story is saved, allowing the player to resume from where they left off.
    /// </summary>
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        if (_currentStory != null && _eventId > 0)
        {
            var currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

            if (currentEvent != null)
            {
                var saveViewModel = new SaveViewModel();

                await saveViewModel.SaveGameAsync(_currentStory, currentEvent);
            }
        }
    }


    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(YourStories)}?storyId={_storyId}");
    }



    #region UI Management

    private void SetResponsiveSizes()
    {
        double pageWidth = this.Width;
        double pageHeight = this.Height;
        if (pageWidth > 0 && pageHeight > 0)
        {
            EventTitleLabel.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
            EventDescriptionLabel.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.FRAME_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);

            UIHelper.SetButtonSize(this, BackButton, true);
        }
    }

    #endregion
}