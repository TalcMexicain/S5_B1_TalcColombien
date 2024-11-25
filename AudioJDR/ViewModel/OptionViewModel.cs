using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// Represents the ViewModel for managing Options within an Event, including CRUD operations.
    /// </summary>
    public class OptionViewModel : BaseViewModel
    {
        private readonly EventViewModel eventViewModel;
        private Option currentOption;
        private ObservableCollection<string> words;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionViewModel"/> class.
        /// </summary>
        /// <param name="eventViewModel">The parent EventViewModel managing the event.</param>
        /// <param name="optionInstance">The Option instance to manage, a new one is created if null.</param>
        public OptionViewModel(EventViewModel eventViewModel, Option? optionInstance = null)
        {
            this.eventViewModel = eventViewModel;
            currentOption = optionInstance ?? CreateNewOption();
            words = new ObservableCollection<string>(currentOption.GetWords());
        }

        private Option CreateNewOption()
        {
            int newOptionId = GenerateNewOptionId();
            return new Option
            {
                IdOption = newOptionId,
                NameOption = string.Empty,
                Text = string.Empty,
                LinkedEvent = null
            };
        }

        private int GenerateNewOptionId()
        {
            return eventViewModel.CurrentEvent.Options.Count > 0
                ? eventViewModel.CurrentEvent.Options.Max(o => o.IdOption) + 1
                : 1;
        }

        /// <summary>
        /// Gets or sets the current Option instance managed by this ViewModel.
        /// </summary>
        public Option CurrentOption
        {
            get => currentOption;
            set
            {
                currentOption = value;
                Words = new ObservableCollection<string>(value.GetWords());
                OnPropertyChanged(nameof(CurrentOption));
            }
        }

        /// <summary>
        /// Gets or sets the collection of words associated with the option.
        /// </summary>
        public ObservableCollection<string> Words
        {
            get => words;
            private set
            {
                words = value;
                OnPropertyChanged(nameof(Words));
            }
        }

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
                Words.Add(word);
                currentOption.AddWordInList(word);
                await UpdateOptionAsync(currentOption);
                Debug.WriteLine($"Added word: {word} to option {currentOption.NameOption}");
                success = true;
            }
            else
            {
                Debug.WriteLine($"Word {word} already exists in option {currentOption.NameOption}");
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
                Words.Remove(word);
                currentOption.RemoveWordInList(word);
                await UpdateOptionAsync(currentOption);
                Debug.WriteLine($"Removed word: {word} from option {currentOption.NameOption}");
                success = true;
            }
            else
            {
                Debug.WriteLine($"Word {word} not found in option {currentOption.NameOption}");
            }
            
            return success;
        }

        /// <summary>
        /// Adds a new option to the current event.
        /// </summary>
        /// <param name="newOption">The new Option to add.</param>
        public async Task AddOptionAsync(Option newOption)
        {
            eventViewModel.CurrentEvent.Options.Add(newOption);
            await eventViewModel.UpdateEventAsync(eventViewModel.CurrentEvent);
            Debug.WriteLine($"Added new option: {newOption.NameOption}");
        }

        /// <summary>
        /// Updates the current option with new information.
        /// </summary>
        /// <param name="updatedOption">The Option with updated details.</param>
        public async Task UpdateOptionAsync(Option updatedOption)
        {
            currentOption.NameOption = updatedOption.NameOption;
            currentOption.Text = updatedOption.Text;
            currentOption.LinkedEvent = updatedOption.LinkedEvent;
            OnPropertyChanged(nameof(CurrentOption));
            await eventViewModel.UpdateEventAsync(eventViewModel.CurrentEvent);
            Debug.WriteLine($"Updated option: {updatedOption.NameOption}");
        }

        /// <summary>
        /// Deletes an option by its ID from the current event.
        /// </summary>
        /// <param name="optionId">The ID of the option to delete.</param>
        public async Task DeleteOptionAsync(int optionId)
        {
            var optionToDelete = eventViewModel.CurrentEvent.Options.FirstOrDefault(o => o.IdOption == optionId);
            if (optionToDelete != null)
            {
                eventViewModel.CurrentEvent.Options.Remove(optionToDelete);
                await eventViewModel.UpdateEventAsync(eventViewModel.CurrentEvent);
                Debug.WriteLine($"Deleted option with ID: {optionId}");
            }
            else
            {
                Debug.WriteLine($"Option with ID {optionId} not found for deletion");
                throw new KeyNotFoundException($"Option with ID {optionId} not found");
            }
        }

        /// <summary>
        /// Gets the parent Event of the current option.
        /// </summary>
        /// <returns>The parent Event.</returns>
        public Event GetParentEvent()
        {
            return eventViewModel.CurrentEvent;
        }

        /// <summary>
        /// Gets the parent EventViewModel.
        /// </summary>
        /// <returns>The parent EventViewModel.</returns>
        public EventViewModel GetParentEventViewModel()
        {
            return eventViewModel;
        }
    }
}

