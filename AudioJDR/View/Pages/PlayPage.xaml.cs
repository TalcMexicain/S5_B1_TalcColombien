using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using View.Pages;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages;

/// <summary>
/// The PlayPage class displays events and their related options.
/// </summary>
public partial class PlayPage : ContentPage, IQueryAttributable
{
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
        BindingContext = _viewModel;
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

    /// <summary>
    /// Loads the event and its options by story ID and event ID.
    /// </summary>
    /// <param name="storyId">The ID of the story.</param>
    /// <param name="eventId">The ID of the event.</param>
    private async Task LoadEvent(int storyId, int eventId)
    {
        _currentStory = await _viewModel.GetStoryByIdAsync(storyId); // Store the story in the private field
        var eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);

        if (eventToShow != null)
        {
            EventTitleLabel.Text = eventToShow.Name;
            EventDescriptionLabel.Text = eventToShow.Description;
        }
        else
        {
            await UIHelper.ShowErrorDialog(this, "Event not found.");
        }
    }

    /// <summary>
    /// Handles the submission of the user's input and attempts to find the best matching option for the current event.
    /// Navigates to the corresponding event if a match is found.
    /// </summary>
    private async void OnOptionSubmitted(object sender, EventArgs e)
    {
        // Retrieve the user's input and normalize it to lowercase
        string userInput = OptionEntry?.Text?.Trim().ToLower();

        if (string.IsNullOrEmpty(userInput))
        {
            // Display an error if no input was provided
            await DisplayAlert("Error", "Please enter an option.", "OK");
            return;
        }

        // Convert the user's input into a list of words
        var userWords = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Find the best matching option for the current event
        var result = GetBestMatchingOptionForCurrentEvent(userWords);

        if (result.TiedOptions.Count > 1)
        {
            // If multiple options have the same score, prompt the user to be more specific
            await HandleTiedOptions(result.TiedOptions);
        }
        else if (result.BestMatchingOption != null && result.LinkedEvent != null)
        {
            // Navigate to the linked event of the best matching option
            await NavigateToEvent(result.LinkedEvent.IdEvent);
        }
        else
        {
            // Display an error if no matching option was found
            await DisplayAlert("Error", "No matching option or linked event found.", "OK");
        }
    }

    /// <summary>
    /// Finds the best matching option for the current event based on the user's input words.
    /// </summary>
    /// <param name="userWords">The words entered by the user.</param>
    /// <returns>
    /// A tuple containing the best matching option, the linked event, and a list of tied options if multiple options have the same score.
    /// </returns>
    private (Option? BestMatchingOption, Event? LinkedEvent, List<Option> TiedOptions) GetBestMatchingOptionForCurrentEvent(string[] userWords)
    {
        Option bestMatchingOption = null;
        Event linkedEvent = null;
        double bestScore = 0.0;
        List<Option> tiedOptions = new List<Option>();
        var currentEvent = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == _eventId);

        // Ensure the current event is not null
        if (currentEvent == null)
            return (null, null, tiedOptions);

        // Iterate over the options of the current event only
        foreach (var option in currentEvent.Options)
        {
            // Retrieve the keywords for the option
            var optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();

            if (optionWords.Length > 0)
            {
                // Calculate the match score between user input and option keywords
                double matchScore = CalculateMatchScore(userWords, optionWords);

                if (matchScore > bestScore)
                {
                    // Update the best match
                    bestScore = matchScore;
                    bestMatchingOption = option;
                    tiedOptions.Clear();
                    tiedOptions.Add(option);
                    linkedEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == option.LinkedEvent?.IdEvent);
                }
                else if (matchScore == bestScore && matchScore > 0.0)
                {
                    // Handle tied options with the same score
                    tiedOptions.Add(option);
                }
            }
        }

        return (bestMatchingOption, linkedEvent, tiedOptions);
    }

    /// <summary>
    /// Calculates the match score between the user's input and an option's keywords.
    /// </summary>
    /// <param name="userWords">The words entered by the user.</param>
    /// <param name="optionWords">The keywords associated with an option.</param>
    /// <returns>The match score as a proportion of matching words.</returns>
    private double CalculateMatchScore(string[] userWords, string[] optionWords)
    {
        int matchCount = 0;

        // Count the number of matching words between user input and option keywords
        foreach (var word in userWords)
        {
            if (optionWords.Contains(word))
            {
                matchCount++;
            }
        }

        // Return the match score as the proportion of matching words
        return (double)matchCount / optionWords.Length;
    }

    /// <summary>
    /// Displays a prompt to the user when multiple options have the same match score, asking them to provide more precise input.
    /// </summary>
    /// <param name="tiedOptions">The list of options that have the same match score.</param>
    private async Task HandleTiedOptions(List<Option> tiedOptions)
    {
        // Generate a list of tied options to display to the user
        string optionsList = string.Join("\n", tiedOptions.Select(o => $"- {o.NameOption}"));
        await DisplayAlert(
            "Clarification Needed",
            $"Multiple options match your input:\n{optionsList}\nPlease be more specific.",
            "OK"
        );
    }

    /// <summary>
    /// Navigates to the event associated with a specific event ID.
    /// </summary>
    /// <param name="eventId">The ID of the event to navigate to.</param>
    private async Task NavigateToEvent(int eventId)
    {
        await Shell.Current.GoToAsync($"{nameof(PlayPage)}?storyId={_storyId}&eventId={eventId}");
    }



    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(YourStories)}?storyId={_storyId}");
    }
}
