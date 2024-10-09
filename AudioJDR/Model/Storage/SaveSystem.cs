using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Model.Storage
{
    public class SaveSystem
    {
        private readonly string _savesFolderPath;

        public SaveSystem()
        {
            // Define the path to the saves folder
            _savesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "saves");

            // Create the saves directory if it doesn't exist
            if (!Directory.Exists(_savesFolderPath))
            {
                Directory.CreateDirectory(_savesFolderPath);
            }
        }

        /// <summary>
        /// Save the game state (Story and Event) using the story name as the folder name
        /// </summary>
        /// <param name="save">Save object to save</param>
        /// <returns></returns>
        public async Task SaveGameAsync(Save save)
        {
            // Use the story's title as the save folder name
            string saveFolderName = save.Story.Title;

            // Path for this specific save
            string saveFolderPath = Path.Combine(_savesFolderPath, saveFolderName);

            // Create or override the folder for this specific save
            Directory.CreateDirectory(saveFolderPath);

            // Save the Story to a JSON file
            string storyFilePath = Path.Combine(saveFolderPath, "story.json");
            string storyJson = JsonSerializer.Serialize(save.Story, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(storyFilePath, storyJson);

            // Save the current Event to a JSON file
            string eventFilePath = Path.Combine(saveFolderPath, "current_event.json");
            string eventJson = JsonSerializer.Serialize(save.CurrentEvent, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(eventFilePath, eventJson);
        }

        /// <summary>
        /// Load a save by the story name
        /// </summary>
        /// <param name="storyTitle">Title of the Story to load</param>
        /// <returns></returns>
        public async Task<Save> LoadGameAsync(string storyTitle)
        {
            string saveFolderPath = Path.Combine(_savesFolderPath, storyTitle);

            if (!Directory.Exists(saveFolderPath))
            {
                return null; // Save folder doesn't exist
            }

            // Load the Story from the JSON file
            string storyFilePath = Path.Combine(saveFolderPath, "story.json");
            if (!File.Exists(storyFilePath)) return null;
            string storyJson = await File.ReadAllTextAsync(storyFilePath);
            Story story = JsonSerializer.Deserialize<Story>(storyJson);

            // Load the current Event from the JSON file
            string eventFilePath = Path.Combine(saveFolderPath, "current_event.json");
            if (!File.Exists(eventFilePath)) return null;
            string eventJson = await File.ReadAllTextAsync(eventFilePath);
            Event currentEvent = JsonSerializer.Deserialize<Event>(eventJson);

            return new Save(story, currentEvent)
            {
                SaveDate = Directory.GetCreationTime(saveFolderPath) // Use folder creation time as save date
            };
        }

        /// <summary>
        /// Delete a save by the story name
        /// </summary>
        /// <param name="storyTitle">Title of the story to delete</param>
        public void DeleteSave(string storyTitle)
        {
            string saveFolderPath = Path.Combine(_savesFolderPath, storyTitle);

            if (Directory.Exists(saveFolderPath))
            {
                Directory.Delete(saveFolderPath, true); // Delete the folder and its contents
            }
        }

        /// <summary>
        /// List all available saves by their story title
        /// </summary>
        /// <returns></returns>
        public List<string> GetAvailableSaves()
        {
            // Get all subfolders in the saves directory
            var saveDirectories = Directory.GetDirectories(_savesFolderPath);
            var saveNames = new List<string>();

            foreach (var directory in saveDirectories)
            {
                saveNames.Add(Path.GetFileName(directory)); // Use the folder name as the save name
            }

            return saveNames;
        }
    }
}
