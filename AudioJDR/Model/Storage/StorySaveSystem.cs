using System.Text.Json;

namespace Model.Storage
{
    /// <summary>
    /// Manages the saving, loading, and deleting of story data in a local storage system
    /// It stores story data in JSON format, using the story's ID as the filename
    /// </summary>
    public class StorySaveSystem
    {
        #region Fields 

        private readonly string _storiesFolderPath;

        #endregion

        #region Constructor 

        /// <summary>
        /// Initializes a new instance of the StorySaveSystem class
        /// Sets the folder path for saving the stories. If no path is provided, the default folder
        /// </summary>
        /// <param name="storiesFolderPath">The path where the stories will be saved. If null, the default path is used</param>
        public StorySaveSystem(string? storiesFolderPath = null)
        {
            if (storiesFolderPath != null)
            {
                _storiesFolderPath = storiesFolderPath;
            }
            else
            {
                _storiesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stories");
            }

            //Debug.WriteLine("Saves folder path: " + _storiesFolderPath);

            // Create the stories directory if it doesn't exist
            if (!Directory.Exists(_storiesFolderPath))
            {
                Directory.CreateDirectory(_storiesFolderPath);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves a story asynchronously to a file, using the story's ID as the filename.
        /// </summary>
        public async Task SaveStoryAsync(Story story)
        {
            // Use the story's ID as the filename to prevent duplicates
            string storyFilePath = Path.Combine(_storiesFolderPath, $"{story.IdStory}.json");

            JsonSerializerOptions optionsJson = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                IncludeFields = true
            };

            string storyJson = JsonSerializer.Serialize(story, optionsJson);
            await File.WriteAllTextAsync(storyFilePath, storyJson);
        }

        /// <summary>
        /// Loads a story by its ID asynchronously.
        /// </summary>
        public async Task<Story> LoadStoryAsync(int storyId)
        {
            // Use the story's ID to locate the file
            string storyFilePath = Path.Combine(_storiesFolderPath, $"{storyId}.json");

            Story? story = null;

            if (File.Exists(storyFilePath))
            {
                string storyJson = await File.ReadAllTextAsync(storyFilePath);
                JsonSerializerOptions optionsJson = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    IncludeFields = true
                };

                story = JsonSerializer.Deserialize<Story>(storyJson, optionsJson);
            }

            return story;
        }

        /// <summary>
        /// Deletes a story by its ID.
        /// </summary>
        public void DeleteStory(int storyId)
        {
            // Use the story's ID to locate the file and delete it
            string storyFilePath = Path.Combine(_storiesFolderPath, $"{storyId}.json");

            if (File.Exists(storyFilePath))
            {
                File.Delete(storyFilePath);
            }
        }

        /// <summary>
        /// Retrieves all saved stories asynchronously.
        /// </summary>
        public async Task<List<Story>> GetSavedStoriesAsync()
        {
            List<Story> stories = new List<Story>();

            if (Directory.Exists(_storiesFolderPath))
            {
                var storyFiles = Directory.GetFiles(_storiesFolderPath, "*.json");

                foreach (var storyFile in storyFiles)
                {
                    string storyJson = await File.ReadAllTextAsync(storyFile);
                    JsonSerializerOptions optionsJson = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        IncludeFields = true
                    };

                    Story? story = JsonSerializer.Deserialize<Story>(storyJson, optionsJson);
                    if (story != null)
                    {
                        stories.Add(story);
                    }
                }
            }

            return stories;
        }

        /// <summary>
        /// Get the list of saved story IDs.
        /// </summary>
        public List<int> GetAvailableStories()
        {
            List<int> storyIds = new List<int>();

            if (Directory.Exists(_storiesFolderPath))
            {
                var storyFiles = Directory.GetFiles(_storiesFolderPath, "*.json");

                foreach (var storyFile in storyFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(storyFile);

                    // Convert the filename (which is the ID) back to an integer
                    if (int.TryParse(fileName, out int storyId))
                    {
                        storyIds.Add(storyId);
                    }
                }
            }

            return storyIds;
        }

        #endregion
    }
}
