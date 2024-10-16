using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Model;
using Model.Storage;


namespace UnitTests
{
    public class SaveSystemTests : IDisposable
    {
        private readonly SaveSystem _saveSystem;
        private readonly string _testSaveFolder;

        public SaveSystemTests()
        {
            //  Use a temporary directory for tests
            _testSaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestSaveSystemClass");

            if (Directory.Exists(_testSaveFolder))
            {
                Directory.Delete(_testSaveFolder, true);
            }

            _saveSystem = new SaveSystem(_testSaveFolder);
        }

        [Fact]
        public async Task SaveGameAsync_Should_Save_Story_And_Event()
        {

            Story story = new Story("Test Story", "This is the description of the tests story");
            Event currentEvent = new Event("Test Event", "This is the description of the tests event");

            Save save = new Save(story, currentEvent);

            // Act
            await _saveSystem.SaveGameAsync(save);

            var optionsJson = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            };

            // Assert
            string storyFilePath = Path.Combine(_testSaveFolder, "Test Story", "story.json");
            string eventFilePath = Path.Combine(_testSaveFolder, "Test Story", "current_event.json");

            Assert.True(File.Exists(storyFilePath), "Story file should exist at " + storyFilePath);
            Assert.True(File.Exists(eventFilePath), "Event file should exist at " + eventFilePath);

            string storyJson = await File.ReadAllTextAsync(storyFilePath);
            var savedStory = JsonSerializer.Deserialize<Story>(storyJson, optionsJson);
            Assert.NotNull(savedStory);
            Assert.Equal(story.Title, savedStory.Title);

            string eventJson = await File.ReadAllTextAsync(eventFilePath);
            var savedEvent = JsonSerializer.Deserialize<Event>(eventJson, optionsJson);
            Assert.NotNull(savedEvent);
            Assert.Equal(currentEvent.Name, savedEvent.Name);
        }

        [Fact]
        public async Task LoadGameAsync_Should_Return_Save_If_Exists()
        {
            // Arrange
            Story story = new Story("Test Story", "This is the description of the story");
            Event currentEvent = new Event("Test Event", "This is the description of the event");

            Event parentEvent = new Event("Test Event", "This is the description of the event");

            Option option = new Option("Test option", parentEvent);

            currentEvent.AddOption(option);
            story.AddEvent(currentEvent);
            story.AddEvent(parentEvent);
            
            //currentEvent.AddOption(option);
            var save = new Save(story, currentEvent);

            // Sauvegarder avant de charger
            await _saveSystem.SaveGameAsync(save);

            // Act
            var loadedSave = await _saveSystem.LoadGameAsync("Test Story");

            // Assert
            Assert.NotNull(loadedSave);
            Assert.Equal(save.Story.Title, loadedSave.Story.Title);
            Assert.Equal(save.CurrentEvent.Name, loadedSave.CurrentEvent.Name);
        }

        [Fact]
        public void DeleteSave_Should_Delete_Save_Folder()
        {
            // Arrange
            var save = CreateTestSave();
            _saveSystem.SaveGameAsync(save).Wait(); // Saving synchronously for simplicity

            string saveFolderPath = Path.Combine(_testSaveFolder, save.Story.Title);

            // Act
            _saveSystem.DeleteSave(save.Story.Title);

            // Assert
            Assert.False(Directory.Exists(saveFolderPath), "Save folder should be deleted.");
        }

        [Fact]
        public void GetAvailableSaves_Should_Return_All_Save_Names()
        {
            // Arrange
            var save1 = CreateTestSave("Story1");
            var save2 = CreateTestSave("Story2");

            _saveSystem.SaveGameAsync(save1).Wait();
            _saveSystem.SaveGameAsync(save2).Wait();

            // Act
            var saveNames = _saveSystem.GetAvailableSaves();

            // Assert
            Assert.Contains("Story1", saveNames);
            Assert.Contains("Story2", saveNames);
        }

        // Helper method to create a test save
        private Save CreateTestSave(string storyTitle = "TestStory")
        {
            var story = new Story(storyTitle, "description");
            var currentEvent = new Event("TestEvent", "description");

            return new Save(story, currentEvent)
            {
                SaveDate = DateTime.Now
            };
        }

        public void Dispose()
        {
            if (Directory.Exists(_testSaveFolder))
            {
                Directory.Delete(_testSaveFolder, true);
            }
        }
    }
}
