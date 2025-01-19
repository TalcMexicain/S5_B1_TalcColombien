using System.Diagnostics;
using System.Text;
using Model;
using Model.Items;
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
        private bool _isInventoryOpen = false;
        private List<Option>? _optionsRequiringItem;
        private Item? _currentItemForOption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PlayPage class.
        /// </summary>
        public PlayPage(ISpeechSynthesizer speechSynthesizer, ISpeechRecognition speechRecognition)
        {
            InitializeComponent();

            Debug.WriteLine("Initializing PlayPage...");

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
            _recognitionViewModel.OpenInventory += async () => await OpenInventory();
            _recognitionViewModel.CloseInventory += async () => await CloseInventory();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies query attributes received during navigation.
        /// </summary>
        /// <param name="query">Dictionary of query parameters.</param>
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Debug.WriteLine("Applying query attributes...");

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
            HashSet<string> keywords = new HashSet<string> { AppResources.Repeat, AppResources.Back, AppResources.Validate, AppResources.Cancel, AppResources.OpenInventory, AppResources.CloseInventory };

            _currentStory = await _storyViewModel.GetStoryByIdAsync(storyId);
            Event? eventToShow = _currentStory?.Events.FirstOrDefault(e => e.IdEvent == eventId);

            if (eventToShow != null)
            {
                EventTitleLabel.Text = eventToShow.Name;
                EventDescriptionLabel.Text = eventToShow.Description;

                foreach (Option option in eventToShow.GetOptions())
                {
                    keywords.Add(option.NameOption);
                    string[] optionWords = option.GetWords()?.Select(word => word.ToLower()).ToArray() ?? Array.Empty<string>();
                    foreach (string word in optionWords)
                    {
                        keywords.Add(word);
                    }
                }
                foreach(Item item in eventToShow.GetItemsToPickUp())
                {
                    keywords.Add(item.Name); 
                }
                await UpdateRecognitionGrammar(keywords);
            }           
        }

        private async Task OnOptionSubmitted()
        {
            string? userInput = OptionEntry?.Text?.Trim().ToLower();

            if (!string.IsNullOrEmpty(userInput))
            {
                // Check if we are expecting an option selection
                if (_optionsRequiringItem != null && _currentItemForOption != null)
                {
                    await HandleOptionSelection(userInput);
                }
                else if(userInput == AppResources.OpenInventory.ToLower() && !_isInventoryOpen)
                {
                    await OpenInventory();
                } 

                else if (userInput == AppResources.CloseInventory.ToLower() && _isInventoryOpen)
                {
                    await CloseInventory();
                }
                else if (_isInventoryOpen)
                {
                    await HandleItemUsage(userInput);
                }
                else
                {
                    await HandleOptionOrItem(userInput);
                }
            }
            else
            {
                _synthesizerViewModel.StopSynthesis();
                _synthesizerViewModel.TextToSynthesize = AppResources.EnterOption;
                _synthesizerViewModel.SynthesizeText();
            }
        }

        private async Task HandleOptionSelection(string userInput)
        {
            try
            {
                if (_optionsRequiringItem != null && _currentItemForOption != null)
                {
                    string normalizedInput = userInput.Trim().ToLower();

                    var selectedOption = _optionsRequiringItem.FirstOrDefault(option =>
                        string.Equals(option.NameOption.Trim().ToLower(), normalizedInput, StringComparison.OrdinalIgnoreCase));

                    if (selectedOption != null)
                    {
                        await UseItemForOption(_currentItemForOption, selectedOption);

                        _optionsRequiringItem = null;
                        _currentItemForOption = null;
                    }
                    else
                    {
                        _synthesizerViewModel.TextToSynthesize = AppResources.InvalidOptionSelection;
                        _synthesizerViewModel.SynthesizeText();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleOptionSelection: {ex.Message}");
            }
        }


        private async Task HandleOptionOrItem(string userInput)
        {
            Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

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
                else if (result.BestMatchingOption != null)
                {
                    // Refresh the option state to ensure it reflects the latest data
                    var requiredItems = result.BestMatchingOption.GetRequiredItems();

                    if (requiredItems.Any())
                    {
                        _synthesizerViewModel.TextToSynthesize = AppResources.OptionRequiresItem;
                        _synthesizerViewModel.SynthesizeText();
                    }
                    else if (result.LinkedEvent != null)
                    {
                        await NavigateToEvent(result.LinkedEvent.IdEvent);
                    }
                }
                else
                {
                    _synthesizerViewModel.StopSynthesis();
                    _synthesizerViewModel.TextToSynthesize = AppResources.NoLinkedOption;
                    _synthesizerViewModel.SynthesizeText();
                }
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
            try
            {
                // Validate keywords to ensure they are not null or empty
                if (keywords != null && keywords.Count > 0)
                {
                    keywords.RemoveWhere(string.IsNullOrWhiteSpace);

                    if (keywords.Count > 0)
                    {
                        _recognitionViewModel.UnloadGrammars();
                        await Task.Delay(500);
                        _recognitionViewModel.StartRecognition(keywords, PageContext);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateRecognitionGrammar: {ex.Message}");
            }
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

        #endregion

        #region Item related Methods

        private async Task OpenInventory()
        {
            _isInventoryOpen = true;
            StringBuilder inventoryText = new StringBuilder(AppResources.InventoryHeader);

            if (_currentStory.Player.Inventory.Any())
            {
                foreach (var item in _currentStory.Player.Inventory)
                {
                    inventoryText.AppendLine($"- {item.Name}");
                }
            }
            else
            {
                inventoryText.AppendLine(AppResources.NoItemsInInventory);
            }

            EventDescriptionLabel.Text = inventoryText.ToString();
            _synthesizerViewModel.TextToSynthesize = inventoryText.ToString();
            _synthesizerViewModel.SynthesizeText();
        }

        private async Task CloseInventory()
        {
            _isInventoryOpen = false;
            Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

            if (currentEvent != null)
            {
                EventDescriptionLabel.Text = currentEvent.Description;
                _synthesizerViewModel.TextToSynthesize = currentEvent.Description;
                _synthesizerViewModel.SynthesizeText();
            }
        }

        private async Task<bool> HandleItemPickup(string userInput)
        {
            bool success = false;

            try
            {
                Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

                if (currentEvent != null && currentEvent.ItemsToPickUp.Any())
                {
                    var matchedItem = currentEvent.ItemsToPickUp.FirstOrDefault(item =>
                        string.Equals(item.Name, userInput, StringComparison.OrdinalIgnoreCase));

                    if (matchedItem != null)
                    {
                        // Add the item to the player's inventory
                        _currentStory.Player.Inventory.Add(matchedItem);

                        // Announce success
                        _synthesizerViewModel.TextToSynthesize = string.Format(AppResources.ItemPickedUpFormat, matchedItem.Name);
                        _synthesizerViewModel.SynthesizeText();

                        // After grammar update, remove the item from the event
                        currentEvent.ItemsToPickUp.Remove(matchedItem);

                        success = true;
                    }
                    else
                    {
                        _synthesizerViewModel.TextToSynthesize = AppResources.ItemNotFound;
                        _synthesizerViewModel.SynthesizeText();
                    }
                }
                else
                {
                    _synthesizerViewModel.TextToSynthesize = AppResources.NoItemsInEvent;
                    _synthesizerViewModel.SynthesizeText();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleItemPickup: {ex.Message}");
                success = false;
            }

            return success;
        }

        private async Task HandleItemUsage(string userInput)
        {
            Event? currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);

            if (currentEvent != null && _currentStory.Player.Inventory.Any())
            {
                // Check if the item exists in the inventory
                var usedItem = _currentStory.Player.Inventory.FirstOrDefault(item =>
                    string.Equals(item.Name, userInput, StringComparison.OrdinalIgnoreCase));

                if (usedItem != null)
                {
                    var optionsRequiringItem = currentEvent.GetOptions()
                        .Where(option => option.GetRequiredItems()?.Contains((KeyItem)usedItem) == true)
                        .ToList();

                    if (optionsRequiringItem.Count == 0)
                    {
                        _synthesizerViewModel.TextToSynthesize = AppResources.ItemNotUsableHere;
                        _synthesizerViewModel.SynthesizeText();
                    }
                    else if (optionsRequiringItem.Count == 1)
                    {
                        await UseItemForOption(usedItem, optionsRequiringItem.First());
                    }
                    else
                    {
                        await PromptPlayerToChooseOption(usedItem, optionsRequiringItem);
                    }
                }
                else
                {
                    _synthesizerViewModel.TextToSynthesize = AppResources.ItemNotFound;
                    _synthesizerViewModel.SynthesizeText();
                }
            }
        }


        private async Task UseItemForOption(Item usedItem, Option option)
        {
            _currentStory.Player.Inventory.Remove(usedItem);

            // Remove the item from the option's required items
            var requiredItems = option.GetRequiredItems();
            if (requiredItems.Contains((KeyItem)usedItem))
            {
                requiredItems.Remove((KeyItem)usedItem);               
            }

            // Update the option in the game state
            var currentEvent = _currentStory.Events.FirstOrDefault(e => e.IdEvent == _eventId);
            if (currentEvent != null)
            {
                var globalOption = currentEvent.GetOptions().FirstOrDefault(o => o.IdOption == option.IdOption);
                if (globalOption != null)
                {
                    globalOption.SetRequiredItems(requiredItems);
                }
            }

            // Announce success
            _synthesizerViewModel.TextToSynthesize = string.Format(AppResources.ItemUsedSuccessfully, usedItem.Name);
            _synthesizerViewModel.SynthesizeText();
            RefreshInventoryText();

            // Update recognition grammar to reflect inventory changes
            await UpdateRecognitionGrammar(new HashSet<string>(_currentStory.Player.Inventory.Select(item => item.Name.ToLower())));
        }

        private void RefreshInventoryText()
        {
            StringBuilder inventoryText = new StringBuilder(AppResources.InventoryHeader);

            if (_currentStory.Player.Inventory.Any())
            {
                foreach (var item in _currentStory.Player.Inventory)
                {
                    inventoryText.AppendLine($"- {item.Name}");
                }
            }
            else
            {
                inventoryText.AppendLine(AppResources.NoItemsInInventory);
            }
            EventDescriptionLabel.Text = inventoryText.ToString();
            _synthesizerViewModel.TextToSynthesize = inventoryText.ToString();
        }

        private async Task PromptPlayerToChooseOption(Item usedItem, List<Option> optionsRequiringItem)
        {
            try
            {
                // Set the context for option selection
                _optionsRequiringItem = optionsRequiringItem;
                _currentItemForOption = usedItem;

                // Build the options list text
                StringBuilder optionsText = new StringBuilder(string.Format(AppResources.MultipleOptionsRequireItemHeader, usedItem.Name));
                for (int i = 0; i < optionsRequiringItem.Count; i++)
                {
                    optionsText.AppendLine($"{i + 1}. {optionsRequiringItem[i].NameOption}");
                }
                // Announce the options requiring the item
                string optionsTextToAnnounce = optionsText.ToString();
                _synthesizerViewModel.TextToSynthesize = optionsTextToAnnounce;
                _synthesizerViewModel.SynthesizeText();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in PromptPlayerToChooseOption: {ex.Message}");
            }
        }

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
            await LoadEvent(_storyId, _eventId);

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
