using Model;
using Model.Storage;
using System.Collections.ObjectModel;

namespace ViewModel
{
    public class StoryViewModel : BaseViewModel
    {
        private readonly StoryManager _storyManager;
        private Story _selectedStory;

        public Story SelectedStory
        {
            get => _selectedStory;
            set
            {
                _selectedStory = value;
                OnPropertyChanged(nameof(SelectedStory));  // Notify UI to update when the selected story changes
                OnPropertyChanged(nameof(Events)); // Notify UI to update when the events change
            }
        }

        public ObservableCollection<Event> Events
        {
            get
            {
                ObservableCollection<Event> tempEvents;
                if (_selectedStory.Events == null)
                {
                    tempEvents = new ObservableCollection<Event>();
                }
                else { tempEvents = _selectedStory.Events; }
                return tempEvents;

            }
            private set
            {
                if (_selectedStory != null)
                {
                    _selectedStory.Events = value;
                    OnPropertyChanged(nameof(Events));
                }
            }
        }


        private ObservableCollection<Story> _stories;
        public ObservableCollection<Story> Stories
        {
            get
            {
                if (_stories == null)
                {
                    _stories = new ObservableCollection<Story>();
                }
                return _stories;
            }
            private set
            {
                _stories = value;
                OnPropertyChanged(nameof(Stories));
            }
        }

        public StoryViewModel()
        {
            _storyManager = new StoryManager();
            _stories = new ObservableCollection<Story>();
            LoadStories();
        }

        /// <summary>
        /// Load stories from the StorySaveSystem and populate the Stories collection.
        /// </summary>
        public async Task LoadStories()
        {
            var savedStories = await _storyManager.GetSavedStoriesAsync();
            if (savedStories != null)
            {
                Stories.Clear();
                foreach (var story in savedStories)
                {
                    Stories.Add(story); // Populate Stories list
                }
            }
        }

        /// <summary>
        /// Adds a story
        /// </summary>
        /// <param name="newStory"></param>
        /// <returns></returns>
        public async Task AddStory(Story newStory)
        {
            Stories.Add(newStory);
            await _storyManager.SaveCurrentStory(newStory);
        }


        /// <summary>
        /// Updates the story with the given with the given story
        /// </summary>
        /// <param name="storyId">the id of the story to update</param>
        /// <param name="updatedStory">the update</param>
        /// <returns></returns>
        public async Task UpdateStory(int storyId, Story updatedStory)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                story.Title = updatedStory.Title;
                story.Description = updatedStory.Description;
                story.Events = updatedStory.Events;

                await _storyManager.SaveCurrentStory(story);
            }
        }

        /// <summary>
        /// Get a story by its ID.
        /// </summary>
        public async Task<Story> GetStoryByIdAsync(int storyId)
        {
            if (Stories.Count == 0)
            {
                // Ensure stories are loaded before trying to retrieve a story
                await LoadStories();
            }
            return Stories.FirstOrDefault(s => s.IdStory == storyId);
        }

        /// <summary>
        /// Deletes a story from the Stories list and from storage.
        /// </summary>
        public async Task DeleteStory(int storyId)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                Stories.Remove(story);
                _storyManager.DeleteStory(story.IdStory);
            }
        }

        /// <summary>
        /// Generate a new unique ID for the story.
        /// </summary>
        public int GenerateNewStoryId()
        {
            if (Stories.Count == 0)
                return 1; // Start from 1 if no stories

            return Stories.Max(s => s.IdStory) + 1;
        }

        #region Event Management

        /// <summary>
        /// Add a new event to a story.
        /// </summary>
        public void AddEventToStory(int storyId, Event newEvent)
        {
            var story = Stories.FirstOrDefault(s => s.IdStory == storyId);
            if (story != null)
            {
                story.Events.Add(newEvent);
            }
        }

        /// <summary>
        /// Update an existing event in a story.
        /// </summary>
        public void UpdateEventInStory(int storyId, Event updatedEvent)
        {
            var story = Stories.FirstOrDefault(s => s.IdStory == storyId);
            if (story != null)
            {
                var eventToUpdate = story.Events.FirstOrDefault(e => e.IdEvent == updatedEvent.IdEvent);
                if (eventToUpdate != null)
                {
                    eventToUpdate.Name = updatedEvent.Name;
                    eventToUpdate.Description = updatedEvent.Description;
                }
            }
        }

        /// <summary>
        /// Delete an event from a story.
        /// </summary>
        public void DeleteEventFromStory(int storyId, int eventId)
        {
            var story = Stories.FirstOrDefault(s => s.IdStory == storyId);
            if (story != null)
            {
                var eventToDelete = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
                if (eventToDelete != null)
                {
                    story.Events.Remove(eventToDelete);
                }
            }
        }

        /// <summary>
        /// Remove an option from a specific event in a story.
        /// </summary>
        public void RemoveOptionFromEvent(int storyId, int eventId, Option option)
        {
            var story = Stories.FirstOrDefault(s => s.IdStory == storyId);
            if (story != null)
            {
                var eventToUpdate = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
                if (eventToUpdate != null)
                {
                    eventToUpdate.Options.Remove(option);
                }
            }
        }

        private int GenerateNewEventId()
        {
            if (_selectedStory == null || _selectedStory.Events.Count == 0)
            {
                return 1;
            }
            return _selectedStory.Events.Max(e => e.IdEvent) + 1;
        }

        /// <summary>
        /// Get an event by its ID from a specific story.
        /// </summary>
        public async Task<Event> GetEventByIdAsync(int storyId, int eventId)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                return story.Events.FirstOrDefault(e => e.IdEvent == eventId);
            }
            return null;
        }

        #endregion
    }
}
