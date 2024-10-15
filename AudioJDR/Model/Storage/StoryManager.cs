using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Storage
{
    public class StoryManager
    {
        private readonly StorySaveSystem _storySaveSystem;

        public StoryManager()
        {
            _storySaveSystem = new StorySaveSystem();
        }

        /// <summary>
        /// Saves the given story
        /// </summary>
        /// <param name="story">a story</param>
        /// <returns></returns>
        public async Task SaveCurrentStory(Story story)
        {
            await _storySaveSystem.SaveStoryAsync(story);
        }

        /// <summary>
        /// Loads a story from the given id
        /// </summary>
        /// <param name="storyId">The id of the seeked story</param>
        /// <returns></returns>
        public async Task<Story> LoadStory(int storyId)
        {
            return await _storySaveSystem.LoadStoryAsync(storyId);
        }

        /// <summary>
        /// Deletes a story with the given id
        /// </summary>
        /// <param name="storyId"></param>
        public void DeleteStory(int storyId)
        {
            _storySaveSystem.DeleteStory(storyId);
        }

        /// <summary>
        /// Loads and returns all available stories from the storage.
        /// </summary>
        /// <returns>List of all saved Story objects.</returns>
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
