using Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// Represents the ViewModel for managing Events within a Story, including CRUD operations.
    /// </summary>
    public class EventViewModel : BaseViewModel
    {
        private readonly StoryViewModel storyViewModel;
        private Event currentEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventViewModel"/> class.
        /// </summary>
        /// <param name="storyViewModel">The parent StoryViewModel managing the story.</param>
        /// <param name="eventInstance">The Event instance to manage, a new one is created if null.</param>
        public EventViewModel(StoryViewModel storyViewModel, Event? eventInstance = null)
        {
            this.storyViewModel = storyViewModel;
            currentEvent = eventInstance ?? CreateNewEvent();
            Options = new ObservableCollection<Option>(currentEvent.Options);
        }

        private Event CreateNewEvent()
        {
            int newEventId = GenerateNewEventId();
            return new Event
            {
                IdEvent = newEventId,
                Name = string.Empty,
                Description = string.Empty,
                Options = new List<Option>()
            };
        }

        private int GenerateNewEventId()
        {
            return storyViewModel.CurrentStory.Events.Count > 0
                ? storyViewModel.CurrentStory.Events.Max(e => e.IdEvent) + 1
                : 1;
        }

        /// <summary>
        /// Gets or sets the current Event instance managed by this ViewModel.
        /// </summary>
        public Event CurrentEvent
        {
            get => currentEvent;
            set
            {
                currentEvent = value;
                OnPropertyChanged(nameof(CurrentEvent));
                OnPropertyChanged(nameof(Options));
            }
        }

        /// <summary>
        /// Gets the collection of Options associated with the Event.
        /// </summary>
        public ObservableCollection<Option> Options { get; private set; }

        /// <summary>
        /// Adds a new event to the current story.
        /// </summary>
        /// <param name="newEvent">The new Event to add.</param>
        public async Task AddEventAsync(Event newEvent)
        {
            storyViewModel.CurrentStory.Events.Add(newEvent);
            await storyViewModel.UpdateStoryAsync(storyViewModel.CurrentStory.IdStory, storyViewModel.CurrentStory);
        }

        /// <summary>
        /// Updates the current event with new information.
        /// </summary>
        /// <param name="updatedEvent">The Event with updated details.</param>
        public async Task UpdateEventAsync(Event updatedEvent)
        {
            currentEvent.Name = updatedEvent.Name;
            currentEvent.Description = updatedEvent.Description;
            currentEvent.Options = updatedEvent.Options;
            OnPropertyChanged(nameof(CurrentEvent));
            OnPropertyChanged(nameof(Options));
            await storyViewModel.UpdateStoryAsync(storyViewModel.CurrentStory.IdStory, storyViewModel.CurrentStory);
        }

        /// <summary>
        /// Deletes an event by its ID from the current story.
        /// </summary>
        /// <param name="eventId">The ID of the event to delete.</param>
        public async Task DeleteEventAsync(int eventId)
        {
            var eventToDelete = storyViewModel.CurrentStory.Events.FirstOrDefault(e => e.IdEvent == eventId);
            if (eventToDelete != null)
            {
                storyViewModel.CurrentStory.Events.Remove(eventToDelete);
                await storyViewModel.UpdateStoryAsync(storyViewModel.CurrentStory.IdStory, storyViewModel.CurrentStory);
            }
        }

        /// <summary>
        /// Retrieves an OptionViewModel for a specific option ID within the event.
        /// </summary>
        /// <param name="optionId">The ID of the option to retrieve.</param>
        /// <returns>An instance of OptionViewModel or null if the option is not found.</returns>
        public OptionViewModel GetOptionViewModel(int optionId)
        {
            var selectedOption = currentEvent.Options.FirstOrDefault(o => o.IdOption == optionId);
            return selectedOption != null ? new OptionViewModel(this, selectedOption) : null;
        }

        /// <summary>
        /// Gets the parent Story of the current event.
        /// </summary>
        /// <returns>The parent Story.</returns>
        public Story GetParentStory()
        {
            return storyViewModel.CurrentStory;
        }

        /// <summary>
        /// Gets the parent StoryViewModel.
        /// </summary>
        /// <returns>The parent StoryViewModel.</returns>
        public StoryViewModel GetParentStoryViewModel()
        {
            return storyViewModel;
        }
    }
}
