using Model;
using Model.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ViewModel
{
    public class StoryViewModel : BaseViewModel
    {
        private readonly StoryManager storyManager;
        private ObservableCollection<Story> stories;
        private Story currentStory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryViewModel"/> class.
        /// </summary>
        public StoryViewModel()
        {
            storyManager = new StoryManager();
            stories = new ObservableCollection<Story>();
            LoadStoriesAsync();
        }

        /// <summary>
        /// Gets the collection of stories.
        /// </summary>
        public ObservableCollection<Story> Stories
        {
            get => stories;
            private set
            {
                stories = value;
                OnPropertyChanged(nameof(Stories));
            }
        }

        /// <summary>
        /// Gets or sets the currently selected story.
        /// </summary>
        public Story CurrentStory
        {
            get => currentStory;
            set
            {
                currentStory = value;
                OnPropertyChanged(nameof(CurrentStory));
            }
        }

        /// <summary>
        /// Loads the stories asynchronously.
        /// </summary>
        public async Task LoadStoriesAsync()
        {
            var savedStories = await storyManager.GetSavedStoriesAsync();
            if (savedStories != null)
            {
                Stories.Clear();
                foreach (var story in savedStories)
                {
                    Stories.Add(story);
                }
            }
        }

        /// <summary>
        /// Adds a new story.
        /// </summary>
        /// <param name="newStory">The new story to add.</param>
        public async Task AddStoryAsync(Story newStory)
        {
            Stories.Add(newStory);
            await storyManager.SaveCurrentStoryAsync(newStory);
        }

        /// <summary>
        /// Updates an existing story.
        /// </summary>
        /// <param name="storyId">The ID of the story to update.</param>
        /// <param name="updatedStory">The updated story details.</param>
        public async Task UpdateStoryAsync(int storyId, Story updatedStory)
        {
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                story.Title = updatedStory.Title;
                story.Description = updatedStory.Description;
                story.Events = updatedStory.Events;
                OnPropertyChanged(nameof(CurrentStory));
                OnPropertyChanged(nameof(Stories));
                await storyManager.SaveCurrentStoryAsync(story);
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
            var story = await GetStoryByIdAsync(storyId);
            if (story != null)
            {
                Stories.Remove(story);
                storyManager.DeleteStory(story.IdStory);
            }
            else
            {
                Debug.WriteLine($"Story with ID {storyId} not found for deletion.");
            }
        }

        /// <summary>
        /// Retrieves a story by its ID asynchronously.
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
                Debug.WriteLine($"Story with ID {storyId} not found.");
                throw new KeyNotFoundException($"Story with ID {storyId} not found.");
            }
            return story;
        }

        /// <summary>
        /// Retrieves an EventViewModel for a specific event ID within the current story.
        /// </summary>
        /// <param name="eventId">The ID of the event to retrieve.</param>
        /// <returns>An instance of EventViewModel.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the event is not found.</exception>
        public async Task<EventViewModel> GetEventViewModelAsync(int eventId)
        {
            var currentEvent = await GetEventByIdAsync(CurrentStory.IdStory, eventId);
            if (currentEvent == null)
            {
                Debug.WriteLine($"Event with ID {eventId} not found.");
                throw new KeyNotFoundException($"Event with ID {eventId} not found.");
            }
            return new EventViewModel(this, currentEvent);
        }

        private async Task<Event> GetEventByIdAsync(int storyId, int eventId)
        {
            var story = await GetStoryByIdAsync(storyId);
            var currentEvent = story.Events.FirstOrDefault(e => e.IdEvent == eventId);
            if (currentEvent == null)
            {
                Debug.WriteLine($"Event with ID {eventId} not found in story ID {storyId}.");
            }
            return currentEvent;
        }

        /// <summary>
        /// Generates a new unique ID for a story.
        /// </summary>
        /// <returns>A new unique story ID.</returns>
        public int GenerateNewStoryId()
        {
            return Stories.Count > 0 ? Stories.Max(s => s.IdStory) + 1 : 1;
        }

        /// <summary>
        /// Exports a story asynchronously.
        /// </summary>
        /// <param name="story">The story to export.</param>
        public async Task ExportStoryAsync(Story story)
        {
            await FileServiceManager.ExportStoryAsync(story);
        }

        /// <summary>
        /// Imports a story asynchronously.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when the imported story is invalid or the file format is incorrect.</exception>
        public async Task ImportStoryAsync()
        {
            try
            {
                var importedStory = await FileServiceManager.ImportStoryAsync();
                if (importedStory != null)
                {
                    importedStory.IdStory = GenerateNewStoryId();
                    await AddStoryAsync(importedStory);
                }
                else
                {
                    throw new InvalidDataException("Invalid story or file format.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error importing story: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Initializes a new story for creation.
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
        }
    }
}
