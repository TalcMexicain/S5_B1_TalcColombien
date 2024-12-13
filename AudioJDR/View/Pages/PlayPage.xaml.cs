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
        public PlayPage(ISpeechSynthesizer speechSynthesizer, ISpeechRecognition speechRecognition)
        {
            InitializeComponent();

            _storyViewModel = new StoryViewModel();
            BindingContext = _recognitionViewModel;

            SetResponsiveSizes();
            this.SizeChanged += OnSizeChanged;
            MessagingCenter.Subscribe<SettingsPage>(this, "LanguageChanged", (sender) => { UpdateAllText(); });

            _recognitionViewModel = new SpeechRecognitionViewModel(speechRecognition);
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

        private async Task LoadEvent(int storyId, int eventId)
        {
            PageContext = "PlayPage" + eventId.ToString();
            HashSet<string> keywords = new HashSet<string> { AppResources.Repeat, AppResources.Back, AppResources.Validate, AppResources.Cancel, "ok" };

            _currentStory = await _storyViewModel.GetStoryByIdAsync(storyId);
            Event? eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);

            if (eventToShow != null)
            {
                EventTitleLabel.Text = eventToShow.Name;
                EventDescriptionLabel.Text = eventToShow.Description;

                foreach (Option option in eventToShow.GetOptions())
                {
                    string[] optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();
                    foreach (string word in optionWords)
                    {
                        keywords.Add(word);
                    }
                }

                await UpdateRecognitionGrammar(keywords);
            }
        }

        private async Task OnOptionSubmitted()
        {
            string? userInput = OptionEntry?.Text?.Trim().ToLower();

            if (!string.IsNullOrEmpty(userInput))
            {
                Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

                // Check if the input matches an item
                if (currentEvent?.ItemsToPickUp.Any(item =>
                    string.Equals(item.Name, userInput, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    bool clearEntry = await HandleItemPickup(userInput);
                    if (clearEntry)
                    {
                        OptionEntry!.Text = string.Empty;
                    }
                }
                else
                {
                    var result = GetBestMatchingOptionForCurrentEvent(userInput.Split(' '));

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
                        _synthesizerViewModel.StopSynthesis();
                        _synthesizerViewModel.TextToSynthesize = AppResources.NoLinkedOption;
                        _synthesizerViewModel.SynthesizeText();
                    }
                }
            }
            else
            {
                _synthesizerViewModel.StopSynthesis();
                _synthesizerViewModel.TextToSynthesize = AppResources.EnterOption;
                _synthesizerViewModel.SynthesizeText();
            }
        }

        private (Option? BestMatchingOption, Event? LinkedEvent, List<Option> TiedOptions) GetBestMatchingOptionForCurrentEvent(string[] userWords)
        {
            Option? bestMatchingOption = null;
            Event? linkedEvent = null;
            double bestScore = 0.0;
            List<Option> tiedOptions = new List<Option>();

            Event? currentEvent = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == _eventId);

            if (currentEvent != null)
            {
                foreach (Option option in currentEvent.GetOptions())
                {
                    string[] optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();

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
            }

            return (bestMatchingOption, linkedEvent, tiedOptions);
        }

        private async Task UpdateRecognitionGrammar(HashSet<string> keywords)
        {
            _recognitionViewModel.UnloadGrammars();
            await Task.Delay(500);
            if (_currentStory != null)
            {
                Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

                if (currentEvent != null)
                {
                    foreach (var item in currentEvent.ItemsToPickUp)
                    {
                        keywords.Add(item.Name.ToLower());
                    }
                }
            }
            _recognitionViewModel.StartRecognition(keywords, PageContext);
        }

        private double CalculateMatchScore(string[] userWords, string[] optionWords)
        {
            int matchCount = 0;

            foreach (string word in userWords)
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
            _synthesizerViewModel.TextToSynthesize = AppResources.MultipleOptionMatch;
            _synthesizerViewModel.SynthesizeText();
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

        #region Item related Methods

        private async Task<bool> HandleItemPickup(string userInput)
        {
            bool success = false;
            Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

            if (currentEvent != null && currentEvent.ItemsToPickUp.Any())
            {
                var matchedItem = currentEvent.ItemsToPickUp.FirstOrDefault(item =>
                    string.Equals(item.Name, userInput, StringComparison.OrdinalIgnoreCase));

                if (matchedItem != null)
                {
                    // Add the item to the player's inventory
                    _currentStory.Player.Inventory.Add(matchedItem);

                    // Remove the item from the event
                    currentEvent.ItemsToPickUp.Remove(matchedItem);

                    // Announce success
                    _synthesizerViewModel.TextToSynthesize = string.Format(AppResources.ItemPickedUpFormat, matchedItem.Name);
                    _synthesizerViewModel.SynthesizeText();

                    // Update the grammar to remove the picked item
                    await UpdateRecognitionGrammar(currentEvent.ItemsToPickUp.Select(item => item.Name.ToLower()).ToHashSet());
                    success = true;
                    
                }
                else
                {
                    // Announce item not found
                    _synthesizerViewModel.TextToSynthesize = AppResources.ItemNotFound;
                    _synthesizerViewModel.SynthesizeText();
                }
            }
            else
            {
                // Announce no items in the event
                _synthesizerViewModel.TextToSynthesize = AppResources.NoItemsInEvent;
                _synthesizerViewModel.SynthesizeText();
            }
            return success;
        }


        #endregion

        #endregion

        #region UI Management

        private void UpdateAllText()
        {
            OptionEntry.Placeholder = AppResources.PlaceholderKeyWord;
            OptionValidation.Text = AppResources.Confirm;
            RepeatButton.Text = AppResources.Repeat;
            BackButton.Text = AppResources.Back;
        }

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

        private async Task ClosePopup()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            SetResponsiveSizes();
        }

        #endregion

        #region Lifecycle Methods

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _storyViewModel.LoadStoriesAsync();
            await LoadEvent(_storyId,_eventId);
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
                Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

                if (currentEvent != null)
                {
                    SaveViewModel saveViewModel = new SaveViewModel();
                    await saveViewModel.SaveGameAsync(_currentStory, currentEvent);
                }
            }
        }

        #endregion
    }
}
