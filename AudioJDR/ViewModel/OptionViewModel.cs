using Model;
using Model.Items;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ViewModel.Resources.Localization;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing Option objects and their operations.
    /// Handles option creation, modification, deletion, and word management.
    /// </summary>
    public class OptionViewModel : BaseViewModel
    {
        #region Fields

        private readonly EventViewModel _parentEventViewModel;
        private Option _currentOption;
        private ObservableCollection<string> _words;
        private ObservableCollection<KeyItem> _requiredItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current Option being worked with.
        /// </summary>
        public Option CurrentOption
        {
            get => _currentOption;
            set
            {
                if (SetProperty(ref _currentOption, value))
                {
                    RefreshWords();
                }
            }
        }

        /// <summary>
        /// Gets the collection of words for the current Option.
        /// </summary>
        public ObservableCollection<string> Words
        {
            get => _words;
            private set => SetProperty(ref _words, value);
        }

        /// <summary>
        /// Gets the collection of required items for the current Option.
        /// </summary>
        public ObservableCollection<KeyItem> RequiredItems
        {
            get => _requiredItems;
            private set => SetProperty(ref _requiredItems, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OptionViewModel class.
        /// </summary>
        /// <param name="eventViewModel">The parent EventViewModel.</param>
        /// <param name="optionInstance">The Option instance to manage, creates new if null.</param>
        /// <exception cref="ArgumentNullException">Thrown when eventViewModel is null.</exception>
        public OptionViewModel(EventViewModel eventViewModel, Option? optionInstance = null)
        {
            if (eventViewModel == null)
            {
                throw new ArgumentNullException(string.Format(AppResourcesVM.OptionVM_NullException,nameof(eventViewModel)));
            }

            _parentEventViewModel = eventViewModel;
            _words = new ObservableCollection<string>();
            _requiredItems = new ObservableCollection<KeyItem>();
            CurrentOption = optionInstance ?? CreateNewOption();
            RefreshWords();
            RefreshRequiredItems();
        }

        #endregion

        #region Option Management

        /// <summary>
        /// Updates the current option with new information.
        /// </summary>
        /// <param name="updatedOption">The Option with updated details.</param>
        /// <exception cref="ArgumentNullException">Thrown when updatedOption is null.</exception>
        public async Task UpdateOptionAsync(Option updatedOption)
        {
            if (updatedOption == null)
            {
                throw new ArgumentNullException(string.Format(AppResourcesVM.OptionVM_NullException, nameof(updatedOption)));
            }

            CurrentOption.NameOption = updatedOption.NameOption;
            CurrentOption.LinkedEvent = updatedOption.LinkedEvent;
            await _parentEventViewModel.UpdateEventAsync(_parentEventViewModel.CurrentEvent);
        }

        /// <summary>
        /// Initializes a new option with default values.
        /// </summary>
        public async Task InitializeNewOptionAsync()
        {
            CurrentOption = CreateNewOption();
            await _parentEventViewModel.AddOptionAsync(CurrentOption);
        }

        /// <summary>
        /// Deletes the current option from its parent event.
        /// </summary>
        public async Task DeleteAsync()
        {
            await _parentEventViewModel.DeleteOptionAsync(CurrentOption.IdOption);
        }

        #endregion

        #region Word Management

        /// <summary>
        /// Adds a new word to the option's word list.
        /// </summary>
        /// <param name="word">The word to add.</param>
        /// <returns>True if the word was added successfully, false if it already exists.</returns>
        public async Task<bool> AddWordAsync(string word)
        {
            bool success = false;
            
            if (!Words.Contains(word))
            {
                CurrentOption.AddWordInList(word);
                Words.Add(word);
                await _parentEventViewModel.UpdateEventAsync(_parentEventViewModel.CurrentEvent);
                success = true;
            }
            
            return success;
        }

        /// <summary>
        /// Removes a word from the option's word list.
        /// </summary>
        /// <param name="word">The word to remove.</param>
        /// <returns>True if the word was removed successfully, false if it wasn't found.</returns>
        public async Task<bool> RemoveWordAsync(string word)
        {
            bool success = false;
            
            if (Words.Contains(word))
            {
                CurrentOption.RemoveWordInList(word);
                Words.Remove(word);
                await _parentEventViewModel.UpdateEventAsync(_parentEventViewModel.CurrentEvent);
                success = true;
            }

            return success;
        }
        #endregion

        #region Required Items Management

        /// <summary>
        /// Adds a new required item to the option's item list
        /// </summary>
        /// <param name="item">The KeyItem to add</param>
        /// <returns>True if the item was added successfully, false if it already exists</returns>
        public async Task<bool> AddRequiredItemAsync(KeyItem item)
        {
            bool success = false;

            if (!RequiredItems.Contains(item))
            {
                CurrentOption.AddKeyItem(item);
                RequiredItems.Add(item);
                await _parentEventViewModel.UpdateEventAsync(_parentEventViewModel.CurrentEvent);
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Removes a required item from the option's item list
        /// </summary>
        /// <param name="item">The KeyItem to remove</param>
        /// <returns>True if the item was removed successfully, false if it wasn't found</returns>
        public async Task<bool> RemoveRequiredItemAsync(KeyItem item)
        {
            bool success = false;

            if (RequiredItems.Contains(item))
            {
                CurrentOption.RemoveKeyItem(item);
                RequiredItems.Remove(item);
                await _parentEventViewModel.UpdateEventAsync(_parentEventViewModel.CurrentEvent);
                success = true;
            }

            return success;
        }

        private void RefreshRequiredItems()
        {
            RequiredItems.Clear();

            if (CurrentOption?.GetRequiredItems() != null)
            {
                foreach (KeyItem item in CurrentOption.GetRequiredItems())
                {
                    RequiredItems.Add(item);
                }
            }
        }

        #endregion

        #region Utility Methods

        private Option CreateNewOption()
        {
            return new Option
            {
                IdOption = GenerateNewOptionId(),
                NameOption = string.Empty,
                LinkedEvent = null
            };
        }

        /// <summary>
        /// Generates a new unique ID for an option.
        /// </summary>
        /// <returns>A new unique option ID.</returns>
        public int GenerateNewOptionId()
        {
            List<Option> existingOptions = _parentEventViewModel.CurrentEvent.GetOptions();
            int newId = existingOptions.Count > 0 ? existingOptions.Max(o => o.IdOption) + 1 : 1;
            return newId;
        }

        private void RefreshWords()
        {
            Words.Clear();
            if (CurrentOption?.GetWords() != null)
            {
                foreach (string word in CurrentOption.GetWords())
                {
                    Words.Add(word);
                }
            }
        }

        /// <summary>
        /// Gets the parent Event of the current option.
        /// </summary>
        /// <returns>The parent Event.</returns>
        public Event GetParentEvent()
        {
            return _parentEventViewModel.CurrentEvent;
        }

        #endregion
    }
}

