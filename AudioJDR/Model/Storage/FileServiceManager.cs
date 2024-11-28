using System.Diagnostics;
using System.Text.Json;

namespace Model
{
    /// <summary>
    /// General exportation and importation handler
    /// </summary>
    public static class FileServiceManager
    {
        private static IFileService _fileService;

        static FileServiceManager()
        {
            // The class used is selected according to the platform
#if WINDOWS
            _fileService = new WindowsFileService();
#elif ANDROID
            _fileService = AndroidFileService.Instance;
#endif
        }

        /// <summary>
        /// Export story using its ID to name the file and serializing it using JsonSerializer
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        public static async Task<bool> ExportStoryAsync(Story story)
        {
            bool success = false;
            var fileName = $"{story.IdStory}.json";
            
            try
            {
                var optionsJson = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                };
                var fileContent = JsonSerializer.SerializeToUtf8Bytes(story, optionsJson);
                success = await _fileService.ExportStoryAsync(fileName, fileContent);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Serialization error: {ex.Message}");
                throw new InvalidDataException("File Couldn't be serialized.");
            }
            
            return success;
        }

        /// <summary>
        /// Import story, validate, and return the deserialized story
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static async Task<Story> ImportStoryAsync()
        {
            var fileData = await _fileService.ImportStoryAsync(); // Platform-specific implementation handles file selection and content

            if (fileData != null)
            {
                try
                {
                    var jsonString = System.Text.Encoding.UTF8.GetString(fileData);
                    Debug.WriteLine(jsonString);

                    var optionsJson = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    };

                    var importedStory = JsonSerializer.Deserialize<Story>(jsonString, optionsJson);

                    if (importedStory != null && !string.IsNullOrEmpty(importedStory.Title))
                    {
                        return importedStory; // Return the valid story object
                    }
                    else
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

            return null; // Return null if no valid story was found
        }
    }
}

