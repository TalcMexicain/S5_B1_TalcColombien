
namespace Model.Storage
{
    /// <summary>
    /// Defines the contract for managing the storage and retrieval of stories
    /// Provides methods for saving, loading, deleting, and retrieving lists of stories
    /// </summary>
    public interface IStoryManager
    {
        /// <summary>
        /// Deletes a story with the specified ID from storage
        /// </summary>
        /// <param name="storyId">The ID of the story to delete</param>
        void DeleteStory(int storyId);

        /// <summary>
        /// Retrieves all saved stories asynchronously
        /// </summary>
        /// <returns>The list of all saved Story objects</returns>
        Task<List<Story>> GetSavedStoriesAsync();

        /// <summary>
        /// Loads a story by its ID asynchronously
        /// </summary>
        /// <param name="storyId">The ID of the story to load</param>
        /// <returns>The loaded story</returns>
        Task<Story> LoadStory(int storyId);

        /// <summary>
        /// Saves the specified story asynchronously
        /// </summary>
        /// <param name="story">The story to save</param>
        /// <returns>A task representing the asynchronous save operation</returns>
        Task SaveCurrentStoryAsync(Story story);
    }
}