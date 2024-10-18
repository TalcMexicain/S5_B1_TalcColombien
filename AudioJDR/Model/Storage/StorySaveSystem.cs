using System.Text.Json;

namespace Model.Storage
{
    public class StorySaveSystem
    {
        private readonly string _storiesFolderPath;

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

        /// <summary>
        /// Saves a story asynchronously to a file, using the story's ID as the filename.
        /// </summary>
        public async Task SaveStoryAsync(Story story)
        {
            // Use the story's ID as the filename to prevent duplicates
            string storyFilePath = Path.Combine(_storiesFolderPath, $"{story.IdStory}.json");

            var optionsJson = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
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

            if (File.Exists(storyFilePath))
            {
                string storyJson = await File.ReadAllTextAsync(storyFilePath);
                var optionsJson = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };

                return JsonSerializer.Deserialize<Story>(storyJson, optionsJson);
            }

            return null;
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
            var stories = new List<Story>();

            if (Directory.Exists(_storiesFolderPath))
            {
                var storyFiles = Directory.GetFiles(_storiesFolderPath, "*.json");

                foreach (var storyFile in storyFiles)
                {
                    string storyJson = await File.ReadAllTextAsync(storyFile);
                    var optionsJson = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    };

                    var story = JsonSerializer.Deserialize<Story>(storyJson, optionsJson);
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
            if (Directory.Exists(_storiesFolderPath))
            {
                var storyFiles = Directory.GetFiles(_storiesFolderPath, "*.json");
                var storyIds = new List<int>();

                foreach (var storyFile in storyFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(storyFile);

                    // Convert the filename (which is the ID) back to an integer
                    if (int.TryParse(fileName, out int storyId))
                    {
                        storyIds.Add(storyId);
                    }
                }

                return storyIds;
            }

            return new List<int>();
        }
    }
}
