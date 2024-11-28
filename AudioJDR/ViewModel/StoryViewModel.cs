using Model;
using Model.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing Story objects and their operations.
    /// Handles story creation, modification, deletion, and persistence.
    /// </summary>
    public class StoryViewModel : BaseViewModel
    {
        #region Fields

        private readonly StoryManager _storyManager;
        private Story _currentStory;
        private ObservableCollection<Story> _stories;
        private ObservableCollection<EventViewModel> _events;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current Story being worked with.
        /// </summary>
        public Story CurrentStory
        {
            get => _currentStory;
            set => SetProperty(ref _currentStory, value);
        }

        /// <summary>
        /// Gets the collection of all available Stories.
        /// </summary>
        public ObservableCollection<Story> Stories
        {
            get => _stories;
            private set => SetProperty(ref _stories, value);
        }

        /// <summary>
        /// Gets the collection of EventViewModels for the current Story.
        /// </summary>
        public ObservableCollection<EventViewModel> Events
        {
            get => _events;
            private set => SetProperty(ref _events, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the StoryViewModel class.
        /// </summary>
        public StoryViewModel()
        {
            _storyManager = new StoryManager();
            _stories = new ObservableCollection<Story>();
            _events = new ObservableCollection<EventViewModel>();
            LoadStoriesAsync();
        }

        #endregion

        #region Story Management

        /// <summary>
        /// Loads all saved stories from storage and initializes their EventViewModels.
        /// </summary>
        public async Task LoadStoriesAsync()
        {
            var savedStories = await _storyManager.GetSavedStoriesAsync();
            Stories.Clear();
            Events.Clear();
            
            if (savedStories != null)
            {
                foreach (var story in savedStories)
                {
                    Stories.Add(story);
                    if (story == CurrentStory)
                    {
                        RefreshEventViewModels(story);
                    }
                }
            }
            
            Debug.WriteLine($"Loaded {Stories.Count} stories from storage");
        }

        /// <summary>
        /// Adds a new story to the collection and saves it.
        /// </summary>
        /// <param name="newStory">The story to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when newStory is null.</exception>
        public async Task AddStoryAsync(Story newStory)
        {
            if (newStory == null)
            {
                throw new ArgumentNullException(nameof(newStory));
            }

            Stories.Add(newStory);
            await _storyManager.SaveCurrentStoryAsync(newStory);
            Debug.WriteLine($"Added new story: {newStory.Title} (ID: {newStory.IdStory})");
        }

        /// <summary>
        /// Updates an existing story with new information.
        /// </summary>
        /// <param name="storyId">The ID of the story to update.</param>
        /// <param name="updatedStory">The updated story information.</param>
        /// <exception cref="ArgumentNullException">Thrown when updatedStory is null.</exception>
        public async Task UpdateStoryAsync(int storyId, Story updatedStory)
        {
            if (updatedStory == null)
            {
                throw new ArgumentNullException(nameof(updatedStory));
            }

            var existingStory = await GetStoryByIdAsync(storyId);
            
            if (existingStory != null)
            {
                existingStory.Title = updatedStory.Title;
                existingStory.Description = updatedStory.Description;
                existingStory.Events = updatedStory.Events;
                
                RefreshEventViewModels(existingStory);
                OnPropertyChanged(nameof(CurrentStory));
                OnPropertyChanged(nameof(Stories));
                await _storyManager.SaveCurrentStoryAsync(existingStory);
                Debug.WriteLine($"Updated story: {existingStory.Title} (ID: {existingStory.IdStory})");
            }
            else
            {
                await AddStoryAsync(updatedStory);
            }
        }

        /// <summary>
        /// Deletes a story by its ID.
        /// </summary>
        /// <param name="storyId">The ID of the story to delete.</param>
        public async Task DeleteStoryAsync(int storyId)
        {
            var storyToDelete = await GetStoryByIdAsync(storyId);
            
            if (storyToDelete != null)
            {
                Stories.Remove(storyToDelete);
                _storyManager.DeleteStory(storyToDelete.IdStory);
                Debug.WriteLine($"Deleted story with ID: {storyId}");
            }
        }

        /// <summary>
        /// Retrieves a story by its ID.
        /// </summary>
        /// <param name="storyId">The ID of the story to retrieve.</param>
        /// <returns>The story with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the story is not found.</exception>
        public async Task<Story> GetStoryByIdAsync(int storyId)
        {
            if (Stories.Count == 0)
            {
                await LoadStoriesAsync();
            }

            var story = Stories.FirstOrDefault(s => s.IdStory == storyId);
            
            if (story == null)
            {
                Debug.WriteLine($"Story with ID {storyId} not found");
                throw new KeyNotFoundException($"Story with ID {storyId} not found");
            }
            
            return story;
        }

        #endregion

        #region Event Management

        private void RefreshEventViewModels(Story story)
        {
            Events.Clear();
            foreach (var evt in story.Events)
            {
                Events.Add(new EventViewModel(this, evt));
            }
        }

        /// <summary>
        /// Creates or retrieves an EventViewModel for a specific event.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>An EventViewModel instance.</returns>
        public async Task<EventViewModel> GetEventViewModelAsync(int eventId)
        {
            var existingViewModel = Events.FirstOrDefault(evm => evm.CurrentEvent.IdEvent == eventId);
            if (existingViewModel != null)
            {
                return existingViewModel;
            }

            var currentEvent = await GetEventByIdAsync(CurrentStory.IdStory, eventId);
            var newViewModel = new EventViewModel(this, currentEvent);
            Events.Add(newViewModel);
            return newViewModel;
        }

        /// <summary>
        /// Adds a new event to the current story.
        /// </summary>
        /// <param name="newEvent">The event to add.</param>
        /// <exception cref="InvalidOperationException">Thrown when no current story is selected.</exception>
        public async Task AddEventAsync(Event newEvent)
        {
            if (CurrentStory == null)
            {
                throw new InvalidOperationException("No story selected");
            }

            CurrentStory.Events.Add(newEvent);
            var newViewModel = new EventViewModel(this, newEvent);
            Events.Add(newViewModel);
            await UpdateStoryAsync(CurrentStory.IdStory, CurrentStory);
        }

        /// <summary>
        /// Deletes an event from the current story.
        /// </summary>
        /// <param name="eventId">The ID of the event to delete.</param>
        public async Task DeleteEventAsync(int eventId)
        {
            var eventToRemove = Events.FirstOrDefault(e => e.CurrentEvent.IdEvent == eventId);
            if (eventToRemove != null)
            {
                Events.Remove(eventToRemove);
                CurrentStory.Events.Remove(eventToRemove.CurrentEvent);
                await UpdateStoryAsync(CurrentStory.IdStory, CurrentStory);
                Debug.WriteLine($"Deleted event with ID: {eventId}");
            }
        }

        private async Task<Event> GetEventByIdAsync(int storyId, int eventId)
        {
            var story = await GetStoryByIdAsync(storyId);
            var foundEvent = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
            return foundEvent;
        }

        #endregion

        #region Import/Export

        /// <summary>
        /// Exports a story to a file.
        /// </summary>
        /// <param name="story">The story to export.</param>
        /// <exception cref="ArgumentNullException">Thrown when story is null.</exception>
        /// <exception cref="IOException">Thrown when file operations fail.</exception>
        public async Task<bool> ExportStoryAsync(Story story)
        {
            bool success = false;
            
            try
            {
                if (story != null)
                {
                    success = await FileServiceManager.ExportStoryAsync(story);
                    if (success)
                    {
                        Debug.WriteLine($"Story exported: {story.Title} (ID: {story.IdStory})");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error exporting story: {ex.Message}");
            }
            
            return success;
        }

        /// <summary>
        /// Imports a story from a file.
        /// </summary>
        /// <returns>The imported story.</returns>
        /// <exception cref="InvalidDataException">Thrown when the imported data is invalid.</exception>
        /// <exception cref="IOException">Thrown when file operations fail.</exception>
        public async Task<bool> ImportStoryAsync()
        {
            bool success = false;
            
            try
            {
                Story importedStory = await FileServiceManager.ImportStoryAsync();
                if (importedStory != null)
                {
                    // Regenerate a new unique ID for the imported story
                    importedStory.IdStory = GenerateNewStoryId();
                    
                    Stories.Add(importedStory);
                    await _storyManager.SaveCurrentStoryAsync(importedStory); // Save the imported story
                    Debug.WriteLine($"Story imported with new ID and saved: {importedStory.Title} (ID: {importedStory.IdStory})");
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error importing story: {ex.Message}");
            }
            
            return success;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Initializes a new story with default values.
        /// </summary>
        public async Task InitializeNewStoryAsync()
        {
            CurrentStory = new Story
            {
                IdStory = GenerateNewStoryId(),
                Title = string.Empty,
                Description = string.Empty,
                Events = new ObservableCollection<Event>()
            };
            
            await AddStoryAsync(CurrentStory);
            Debug.WriteLine("Initialized new story");
        }

        /// <summary>
        /// Generates a new unique ID for a story.
        /// </summary>
        /// <returns>A new unique story ID.</returns>
        public int GenerateNewStoryId()
        {
            int newId = Stories.Count > 0 ? Stories.Max(s => s.IdStory) + 1 : 1;
            return newId;
        }

        #endregion
    }
}
