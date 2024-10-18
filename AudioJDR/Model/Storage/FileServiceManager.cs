using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace Model
{
    public static class FileServiceManager
    {
        private static IFileService _fileService;

        static FileServiceManager()
        {
            #if WINDOWS
            _fileService = new WindowsFileService();
            #elif ANDROID
            _fileService = new AndroidFileService();
            #endif
        }

        /// <summary>
        /// Export story using its ID to name the file and serializing it using JsonSerializer
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        public static async Task ExportStoryAsync(Story story)
        {
            var fileName = $"{story.IdStory}.json";
            var fileContent = JsonSerializer.SerializeToUtf8Bytes(story); 
            await _fileService.ExportStoryAsync(fileName, fileContent); // Pass to platform-specific implementation
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
                    Debug.WriteLine(jsonString); // Log the raw JSON string

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

