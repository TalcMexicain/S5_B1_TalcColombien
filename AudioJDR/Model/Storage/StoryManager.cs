using System.Diagnostics;

namespace Model.Storage
{
    /// <summary>
    /// Manages the storage and retrieval of stories using the StorySaveSystem.
    /// Provides methods to save, load, and delete stories.
    /// </summary>
    public class StoryManager
    {
        private readonly StorySaveSystem _storySaveSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoryManager"/> class.
        /// </summary>
        public StoryManager()
        {
            _storySaveSystem = new StorySaveSystem();
        }

        /// <summary>
        /// Saves the given story asynchronously.
        /// </summary>
        /// <param name="story">The story to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveCurrentStoryAsync(Story story)
        {
            await _storySaveSystem.SaveStoryAsync(story);
            Debug.WriteLine($"Story successfully saved.");
        }

        /// <summary>
        /// Loads a story by its ID asynchronously.
        /// </summary>
        /// <param name="storyId">The ID of the story to load.</param>
        /// <returns>A task representing the asynchronous load operation, with the loaded <see cref="Story"/>.</returns>
        public async Task<Story> LoadStory(int storyId)
        {
            return await _storySaveSystem.LoadStoryAsync(storyId);
        }

        /// <summary>
        /// Deletes a story with the given ID.
        /// </summary>
        /// <param name="storyId">The ID of the story to delete.</param>
        public void DeleteStory(int storyId)
        {
            _storySaveSystem.DeleteStory(storyId);
        }

        /// <summary>
        /// Loads and returns all available stories from the storage asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with a list of all saved <see cref="Story"/> objects.</returns>
        public async Task<List<Story>> GetSavedStoriesAsync()
        {
            // Get the list of story titles
            var storyTitles = _storySaveSystem.GetAvailableStories();

            // Load each story by title and return the full list of Story objects
            var stories = new List<Story>();
            foreach (var title in storyTitles)
            {
                var story = await _storySaveSystem.LoadStoryAsync(title);
                if (story != null)
                {
                    stories.Add(story);
                }
            }
            return stories;
        }
    }
}
