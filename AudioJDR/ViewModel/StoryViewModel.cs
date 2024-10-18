using Microsoft.Extensions.Logging;
using Model;
using Model.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace ViewModel
{
    public class StoryViewModel : BaseViewModel
    {
        #region Fields

        private readonly StoryManager _storyManager;
        private Story _selectedStory;

        private ObservableCollection<Story> _stories;

        #endregion

        #region Properties

        public Story? SelectedStory
        {
            get => _selectedStory;
            set
            {
                _selectedStory = value;
                OnPropertyChanged(nameof(SelectedStory));  // Notify UI to update when the selected story changes
            }
        }

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

        #endregion

        public StoryViewModel()
        {
            _storyManager = new StoryManager();
            _stories = new ObservableCollection<Story>();
            LoadStories();
        }

        #region Story Management

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
                Debug.WriteLine($"Story Updated : Updating files..");
                await _storyManager.SaveCurrentStory(story);
            }
        }

        /// <summary>
        /// Get a story by its ID.
        /// </summary>
        public async Task<Story> GetStoryByIdAsync(int storyId)
        {
            Debug.WriteLine($"Fetching story with id = {storyId} ..");
            if (Stories.Count == 0)
            {
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
        /// Generate a new unique ID for the story
        /// </summary>
        public int GenerateNewStoryId()
        {
            int newStoryId = 1; // Start from 1 if no stories

            if (Stories.Count > 0)
            {
                newStoryId = Stories.Max(s => s.IdStory) + 1;
            }
                
            return newStoryId;
        }

        public async Task ExportStoryAsync(Story story)
        {
            await FileServiceManager.ExportStoryAsync(story);
        }

        public async Task ImportStoryAsync()
        {
            try
            {
                // Import the story
                var importedStory = await FileServiceManager.ImportStoryAsync();
                if (importedStory != null)
                {
                    // Generate a new ID for the imported story to prevent overwriting existing stories
                    importedStory.IdStory = GenerateNewStoryId();

                    // Add the imported story to the collection
                    AddStory(importedStory);
                }
                else
                {
                    // Handle the case where no valid story was imported
                    throw new InvalidDataException("Invalid story or file format.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                Debug.WriteLine($"Error importing story: {ex.Message}");
            }
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Add a new event to a story.
        /// </summary>
        public async Task AddEventToStory(int storyId, Event newEvent)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                story.Events.Add(newEvent);
                await UpdateStory(storyId,story);
            }
        }

        /// <summary>
        /// Update an existing event in a story.
        /// </summary>
        public async Task UpdateEventInStory(int storyId, Event updatedEvent)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                var eventToUpdate = story.Events.FirstOrDefault(e => e.IdEvent == updatedEvent.IdEvent);
                if (eventToUpdate != null)
                {
                    eventToUpdate.Name = updatedEvent.Name;
                    eventToUpdate.Description = updatedEvent.Description;
                    Debug.WriteLine($"Event added to Story : Updating Lists..");
                    await UpdateStory(storyId, story);
                }
            }
        }

        /// <summary>
        /// Delete an event from a story.
        /// </summary>
        public async Task DeleteEventFromStory(int storyId, int eventId)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                var eventToDelete = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
                if (eventToDelete != null)
                {
                    story.Events.Remove(eventToDelete);
                    await UpdateStory(storyId, story);
                }
            }
        }

        /// <summary>
        /// Remove an option from a specific event in a story.
        /// </summary>
        public async Task RemoveOptionFromEvent(int storyId, int eventId, Option option)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                var eventToUpdate = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
                if (eventToUpdate != null)
                {
                    eventToUpdate.Options.Remove(option);
                    await UpdateStory(storyId, story);
                }
            }
        }

        /// <summary>
        /// Generate a new unique ID for the event
        /// </summary>
        /// <returns></returns>
        public int GenerateNewEventId()
        {
            int newEventId = 1; // Start from 1 if no events

            if (_selectedStory != null && _selectedStory.Events.Count > 0)
            {
                newEventId = _selectedStory.Events.Max(e => e.IdEvent) + 1;
            }

            return newEventId;
        }

        /// <summary>
        /// Get an event by its ID from a specific story.
        /// </summary>
        public async Task<Event> GetEventByIdAsync(int storyId, int eventId)
        {
            Event? result = null;

            var story = await GetStoryByIdAsync(storyId);

            if (story != null)
            {
                Debug.WriteLine($"Story Found : Fetching event with id = {eventId} ..");
                result =  story.Events.FirstOrDefault(e => e.IdEvent == eventId);
            }

            return result;
        }

        #endregion

        #region Option Management

        /// <summary>
        /// Add an option to an event within a story.
        /// </summary>
        public async Task AddOptionToEvent(int storyId, int eventId, Option newOption)
        {
            var eventToUpdate = await GetEventByIdAsync(storyId,eventId);

            if (eventToUpdate != null)
            {
                eventToUpdate.Options.Add(newOption);
                Debug.WriteLine($"Option added to event : Updating lists..");
                await UpdateEventInStory(storyId, eventToUpdate);
            }
            else
            {
                Debug.WriteLine($"Event is null : Option was not added");
            }
        }

        /// <summary>
        /// Update an option in an event within a story.
        /// </summary>
        public async Task UpdateOptionInEvent(int storyId, int eventId, Option updatedOption)
        {
            var story = await GetStoryByIdAsync(storyId);
            var eventToUpdate = story?.Events.FirstOrDefault(e => e.IdEvent == eventId);

            if (eventToUpdate != null)
            {
                var optionToUpdate = eventToUpdate.Options.FirstOrDefault(o => o.IdOption == updatedOption.IdOption);
                if (optionToUpdate != null)
                {
                    optionToUpdate.NameOption = updatedOption.NameOption;
                    optionToUpdate.Text = updatedOption.Text;
                    optionToUpdate.LinkedEvent = updatedOption.LinkedEvent;

                    await UpdateEventInStory(storyId, eventToUpdate);
                }
            }
        }

        /// <summary>
        /// Delete an option from an event within a story.
        /// </summary>
        public async Task DeleteOptionFromEvent(int storyId, int eventId, int optionId)
        {
            var story = await GetStoryByIdAsync(storyId);
            var eventToUpdate = story?.Events.FirstOrDefault(e => e.IdEvent == eventId);

            if (eventToUpdate != null)
            {
                var optionToDelete = eventToUpdate.Options.FirstOrDefault(o => o.IdOption == optionId);
                if (optionToDelete != null)
                {
                    eventToUpdate.Options.Remove(optionToDelete);
                    await UpdateEventInStory(storyId, eventToUpdate);
                }
            }
        }

        public async Task<int> GenerateNewOptionId(int storyId, int eventId)
        {
            int newOptionId = 1;  // Default value for new option ID

            // Retrieve the event using the provided storyId and eventId
            Event selectedEvent = await GetEventByIdAsync(storyId, eventId);

            if (selectedEvent != null && selectedEvent.Options != null && selectedEvent.Options.Count > 0)
            {
                // Find the maximum existing option ID and calculate the next available ID
                newOptionId = selectedEvent.Options.Max(o => o.IdOption) + 1;
            }

            return newOptionId;
        }

        /// <summary>
        /// Get an option by its ID from a specific event in a story.
        /// </summary>
        public async Task<Option> GetOptionByIdAsync(int storyId, int eventId, int optionId)
        {
            Option selectedOption = null;  // Default to null if option is not found

            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                var selectedEvent = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
                if (selectedEvent != null)
                {
                    selectedOption = selectedEvent.Options.FirstOrDefault(o => o.IdOption == optionId);
                }
            }

            return selectedOption;
        }


        #endregion
    }
}
