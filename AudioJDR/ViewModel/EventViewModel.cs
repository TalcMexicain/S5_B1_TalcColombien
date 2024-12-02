using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing Event objects and their operations.
    /// Handles event creation, modification, deletion, and option management.
    /// </summary>
    public class EventViewModel : BaseViewModel
    {
        #region Fields

        private readonly StoryViewModel _parentStoryViewModel;
        private Event _currentEvent;
        private ObservableCollection<OptionViewModel> _options;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current Event being worked with.
        /// </summary>
        public Event CurrentEvent
        {
            get => _currentEvent;
            set
            {
                if (SetProperty(ref _currentEvent, value))
                {
                    RefreshOptionViewModels();
                }
            }
        }

        /// <summary>
        /// Gets the collection of OptionViewModels for the current Event.
        /// </summary>
        public ObservableCollection<OptionViewModel> Options
        {
            get => _options;
            private set => SetProperty(ref _options, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EventViewModel class.
        /// </summary>
        /// <param name="storyViewModel">The parent StoryViewModel.</param>
        /// <param name="eventInstance">The Event instance to manage, creates new if null.</param>
        public EventViewModel(StoryViewModel storyViewModel, Event? eventInstance = null)
        {
            _parentStoryViewModel = storyViewModel ?? throw new ArgumentNullException(nameof(storyViewModel));
            _options = new ObservableCollection<OptionViewModel>();
            CurrentEvent = eventInstance ?? CreateNewEvent();
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Updates the current event with new information.
        /// </summary>
        /// <param name="updatedEvent">The Event with updated details.</param>
        /// <exception cref="ArgumentNullException">Thrown when updatedEvent is null.</exception>
        public async Task UpdateEventAsync(Event updatedEvent)
        {
            if (updatedEvent == null)
            {
                throw new ArgumentNullException(nameof(updatedEvent));
            }

            CurrentEvent.Name = updatedEvent.Name;
            CurrentEvent.Description = updatedEvent.Description;
            await _parentStoryViewModel.UpdateStoryAsync(_parentStoryViewModel.CurrentStory.IdStory, _parentStoryViewModel.CurrentStory);
            Debug.WriteLine($"Updated event: {CurrentEvent.Name} (ID: {CurrentEvent.IdEvent})");
        }

        /// <summary>
        /// Initializes a new event with default values.
        /// </summary>
        public async Task InitializeNewEventAsync()
        {
            CurrentEvent = CreateNewEvent();
            await _parentStoryViewModel.AddEventAsync(CurrentEvent);
            Debug.WriteLine("Initialized new event");
        }

        /// <summary>
        /// Deletes the current event from its parent story.
        /// </summary>
        public async Task DeleteEventAsync()
        {
            await _parentStoryViewModel.DeleteEventAsync(CurrentEvent.IdEvent);
            Debug.WriteLine($"Deleted event with ID: {CurrentEvent.IdEvent}");
        }

        #endregion

        #region Option Management

        /// <summary>
        /// Creates or retrieves an OptionViewModel for a specific option.
        /// </summary>
        /// <param name="optionId">The ID of the option.</param>
        /// <returns>An OptionViewModel instance.</returns>
        public async Task<OptionViewModel> GetOptionViewModelAsync(int optionId)
        {
            var option = CurrentEvent.Options.FirstOrDefault(o => o.IdOption == optionId);
            if (option == null)
            {
                throw new ArgumentException($"Option with ID {optionId} not found");
            }
            return new OptionViewModel(this, option);
        }

        /// <summary>
        /// Adds a new option to the current event.
        /// </summary>
        /// <param name="newOption">The option to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when newOption is null.</exception>
        public async Task AddOptionAsync(Option newOption)
        {
            if (newOption == null)
            {
                throw new ArgumentNullException(nameof(newOption));
            }

            CurrentEvent.Options.Add(newOption);
            var newViewModel = new OptionViewModel(this, newOption);
            Options.Add(newViewModel);
            await UpdateEventAsync(CurrentEvent);
            Debug.WriteLine($"Added new option: {newOption.NameOption} to event {CurrentEvent.Name}");
        }

        /// <summary>
        /// Deletes an option from the current event.
        /// </summary>
        /// <param name="optionId">The ID of the option to delete.</param>
        public async Task DeleteOptionAsync(int optionId)
        {
            var optionToRemove = Options.FirstOrDefault(o => o.CurrentOption.IdOption == optionId);
            if (optionToRemove != null)
            {
                Options.Remove(optionToRemove);
                CurrentEvent.Options.Remove(optionToRemove.CurrentOption);
                await _parentStoryViewModel.UpdateStoryAsync(_parentStoryViewModel.CurrentStory.IdStory, _parentStoryViewModel.CurrentStory);
                Debug.WriteLine($"Deleted option with ID: {optionId}");
            }
        }

        #endregion

        #region Utility Methods

        private Event CreateNewEvent()
        {
            return new Event
            {
                IdEvent = GenerateNewEventId(),
                Name = string.Empty,
                Description = string.Empty,
                Options = new List<Option>()
            };
        }

        /// <summary>
        /// Generates a new unique ID for an event.
        /// </summary>
        /// <returns>A new unique event ID.</returns>
        public int GenerateNewEventId()
        {
            var existingEvents = _parentStoryViewModel.CurrentStory.Events;
            int newId = existingEvents.Count > 0 ? existingEvents.Max(e => e.IdEvent) + 1 : 1;
            return newId;
        }

        private void RefreshOptionViewModels()
        {
            Options.Clear();
            
            if (CurrentEvent?.Options != null)
            {
                foreach (var option in CurrentEvent.Options)
                {
                    Options.Add(new OptionViewModel(this, option));
                }
            }
        }

        #endregion
    }
}
