using System.Diagnostics;
using System.Text.Json;

namespace Model
{
    /// <summary>
    /// General exportation and importation handler for stories.
    /// This class provides methods to export and import stories using the selected file service based on the platform.
    /// </summary>
    public static class FileServiceManager
    {
        #region Fields 

        private static IFileService _fileService;

        #endregion

        #region Constructor

        static FileServiceManager()
        {
            // The class used is selected according to the platform
#if WINDOWS
            _fileService = new WindowsFileService();
#elif ANDROID
            _fileService = AndroidFileService.Instance;
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Exports a story to a JSON file.
        /// </summary>
        /// <param name="story">The story to be exported.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
        /// <exception cref="InvalidDataException">Thrown when the story cannot be serialized.</exception>
        public static async Task<bool> ExportStoryAsync(Story story)
        {
            bool success = false;
            string fileName = $"{story.IdStory}.json";
            byte[] fileContent = null;

            try
            {
                JsonSerializerOptions optionsJson = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };
                fileContent = JsonSerializer.SerializeToUtf8Bytes(story, optionsJson);
                success = await _fileService.ExportStoryAsync(fileName, fileContent);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Serialization error: {ex.Message}");
                throw new InvalidDataException("File couldn't be serialized.");
            }

            return success;
        }

        /// <summary>
        /// Imports a story from a JSON file.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the imported story.</returns>
        /// <exception cref="InvalidDataException">Thrown when the file does not contain a valid story.</exception>
        public static async Task<Story> ImportStoryAsync()
        {
            byte[] fileData = await _fileService.ImportStoryAsync();
            Story importedStory = null;

            if (fileData != null)
            {
                try
                {
                    var jsonString = System.Text.Encoding.UTF8.GetString(fileData);
                    var optionsJson = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    };

                    importedStory = JsonSerializer.Deserialize<Story>(jsonString, optionsJson);

                    if (importedStory == null || string.IsNullOrEmpty(importedStory.Title))
                    {
                        throw new InvalidDataException("Invalid story data in the file.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Deserialization error: {ex.Message}");
                    throw new InvalidDataException("The file does not contain a valid story.");
                }
            }

            return importedStory;
        }

        #endregion
    }
}

