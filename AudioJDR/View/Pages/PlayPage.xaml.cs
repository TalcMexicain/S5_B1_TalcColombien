using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using View.Pages;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages
{
    /// <summary>
    /// The PlayPage class displays events and their related options.
    /// </summary>
    public partial class PlayPage : ContentPage, IQueryAttributable
    {
        #region Fields 

        private SpeechRecognitionViewModel _recognitionViewModel;
        private SpeechSynthesizerViewModel _synthesizerViewModel;
        private StoryViewModel _storyViewModel;
        private Story _currentStory;
        private int _storyId;
        private int _eventId;
        private string PageContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PlayPage class.
        /// </summary>
        public PlayPage(ISpeechSynthesizer speechSynthesizer)
        {
            InitializeComponent();

            _storyViewModel = new StoryViewModel();
            BindingContext = _recognitionViewModel;

            _recognitionViewModel = new SpeechRecognitionViewModel();
            _synthesizerViewModel = new SpeechSynthesizerViewModel(speechSynthesizer);

            // Subscriptions to recognition view model events
            _recognitionViewModel.RepeatSpeech += async () => await RepeatSpeech();
            _recognitionViewModel.NavigatePrevious += async () => await NavigatePrevious();
            _recognitionViewModel.OptionSubmitted += async () => await OnOptionSubmitted();
            _recognitionViewModel.AddWordsToView += AddWordsToView;
            _recognitionViewModel.TextCleared += () =>
            {
                OptionEntry.Text = string.Empty;
            };
        }

        #endregion

        #region Public Methods

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

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the event and its options by story ID and event ID.
        /// </summary>
        private async Task LoadEvent(int storyId, int eventId)
        {
            PageContext = "PlayPage" + eventId.ToString();
            var keywords = new HashSet<string> { "repeter", "retour", "valider", "annuler" };

            _currentStory = await _storyViewModel.GetStoryByIdAsync(storyId);
            Event? eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);

            if (eventToShow != null)
            {
                EventTitleLabel.Text = eventToShow.Name;
                EventDescriptionLabel.Text = eventToShow.Description;

                foreach (var option in eventToShow.Options)
                {
                    var optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();
                    foreach (var word in optionWords)
                    {
                        keywords.Add(word);
                    }
                }

                await UpdateRecognitionGrammar(keywords);
            }
        }

        /// <summary>
        /// Handles the submission of the user's input and attempts to find the best matching option for the current event.
        /// Navigates to the corresponding event if a match is found.
        /// </summary>
        private async Task OnOptionSubmitted()
        {
            string userInput = OptionEntry?.Text?.Trim().ToLower();

            if (string.IsNullOrEmpty(userInput))
            {
                await DisplayAlert("Error", "Please enter an option.", "OK");
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

        /// <summary>
        /// Finds the best matching option for the current event based on user input.
        /// </summary>
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

        private async Task UpdateRecognitionGrammar(HashSet<string> keywords)
        {
            _recognitionViewModel.UnloadGrammars();
            await Task.Delay(500);
            _recognitionViewModel.StartRecognition(keywords, PageContext);
        }

        /// <summary>
        /// Calculates the match score between user input words and option words.
        /// </summary>
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

        /// <summary>
        /// Handles scenarios where multiple options tie for the best match.
        /// </summary>
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

        private async Task RepeatSpeech()
        {
            _synthesizerViewModel.TextToSynthesize = this.EventDescriptionLabel.Text;
            _synthesizerViewModel.StopSynthesis();
            _synthesizerViewModel.SynthesizeText();
        }

        private async Task NavigatePrevious()
        {
            await Shell.Current.GoToAsync($"{nameof(YourStories)}?storyId={_storyId}");
        }

        #endregion

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

        #region Event Handlers

        private async void OnRepeatButtonClicked(object sender, EventArgs e)
        {
            await RepeatSpeech();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await NavigatePrevious();
        }

        private async void OnOptionSubmittedFromButton(object sender, EventArgs e)
        {
            await OnOptionSubmitted();
        }

        private async void AddWordsToView()
        {
            OptionEntry.Text = string.Empty;
            OptionEntry.Text += _recognitionViewModel.RecognizedText;
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Pass the label's text to the ViewModel for TTS synthesis
            _synthesizerViewModel.TextToSynthesize = this.EventDescriptionLabel.Text;
            _synthesizerViewModel.SynthesizeText();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            _synthesizerViewModel.StopSynthesis();
            _recognitionViewModel.UnloadGrammars();

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

        #endregion
    }
}
