using Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ViewModel.Resources.Localization;

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
        /// <exception cref="ArgumentNullException">Thrown when StoryViewModel is null</exception>
        public EventViewModel(StoryViewModel storyViewModel, Event? eventInstance = null)
        {
            if (storyViewModel == null)
            {
                throw new ArgumentNullException(AppResourcesVM.EventVM_Constructor_Exception);
            }

            _parentStoryViewModel = storyViewModel;
            _options = new ObservableCollection<OptionViewModel>();

            if (eventInstance != null)
            {
                CurrentEvent = eventInstance;
            }
            else
            {
                CurrentEvent = CreateNewEvent();
            }
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
                throw new ArgumentNullException(string.Format(AppResourcesVM.EventVM_NullException, nameof(updatedEvent)));
            }

            CurrentEvent.Name = updatedEvent.Name;
            CurrentEvent.Description = updatedEvent.Description;
            await _parentStoryViewModel.UpdateStoryAsync(_parentStoryViewModel.CurrentStory.IdStory, _parentStoryViewModel.CurrentStory);
        }

        /// <summary>
        /// Initializes a new event with default values.
        /// </summary>
        public async Task InitializeNewEventAsync()
        {
            CurrentEvent = CreateNewEvent();
            await _parentStoryViewModel.AddEventAsync(CurrentEvent);
        }

        /// <summary>
        /// Deletes the current event from its parent story.
        /// </summary>
        public async Task DeleteEventAsync()
        {
            await _parentStoryViewModel.DeleteEventAsync(CurrentEvent.IdEvent);
        }

        #endregion

        #region Option Management

        /// <summary>
        /// Creates or retrieves an OptionViewModel for a specific option.
        /// </summary>
        /// <param name="optionId">The ID of the option.</param>
        /// <returns>An OptionViewModel instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the option with id is not found.</exception>
        public async Task<OptionViewModel> GetOptionViewModelAsync(int optionId)
        {
            Option? option = CurrentEvent.Options.FirstOrDefault(o => o.IdOption == optionId);
            if (option == null)
            {
                throw new ArgumentNullException(string.Format(AppResourcesVM.EventVM_GetOptionVMAsync_NullException), optionId.ToString());
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
                throw new ArgumentNullException(string.Format(AppResourcesVM.EventVM_NullException, nameof(newOption)));
            }

            CurrentEvent.Options.Add(newOption);
            OptionViewModel newViewModel = new OptionViewModel(this, newOption);
            Options.Add(newViewModel);
            await UpdateEventAsync(CurrentEvent);
        }

        /// <summary>
        /// Deletes an option from the current event.
        /// </summary>
        /// <param name="optionId">The ID of the option to delete.</param>
        public async Task DeleteOptionAsync(int optionId)
        {
            OptionViewModel? optionToRemove = Options.FirstOrDefault(o => o.CurrentOption.IdOption == optionId);
            if (optionToRemove != null)
            {
                Options.Remove(optionToRemove);
                CurrentEvent.Options.Remove(optionToRemove.CurrentOption);
                await _parentStoryViewModel.UpdateStoryAsync(_parentStoryViewModel.CurrentStory.IdStory, _parentStoryViewModel.CurrentStory);
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
            ObservableCollection<Event> existingEvents = _parentStoryViewModel.CurrentStory.Events;
            int newId = existingEvents.Count > 0 ? existingEvents.Max(e => e.IdEvent) + 1 : 1;
            return newId;
        }

        private void RefreshOptionViewModels()
        {
            Options.Clear();
            
            if (CurrentEvent?.Options != null)
            {
                foreach (Option option in CurrentEvent.Options)
                {
                    Options.Add(new OptionViewModel(this, option));
                }
            }
        }

        #endregion
    }
}
