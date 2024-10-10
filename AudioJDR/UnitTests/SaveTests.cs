using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Storage;

namespace UnitTests
{
    public class SaveTests
    {
        [Fact]
        public void Properties_Test()
        {
            Story storyToTest = new Story("Test Story", "This is the test story description");
            Event currentEvent = new Event("Test Event", "This is the test event description");

            Save save = new Save(storyToTest, currentEvent);

            save.SaveDate = DateTime.Now;

            Assert.Equal(storyToTest, save.Story);
            Assert.Equal(currentEvent, save.CurrentEvent);
            Assert.True(save.SaveDate <= DateTime.Now);
        }

        [Fact]
        public async void DeleteSaves_Test()
        {
            Story storyToTest = new Story("Test Story", "This is the test story description");
            Event currentEvent = new Event("Test Event", "This is the test event description");
            Save save = new Save(storyToTest, currentEvent);

            string pathSaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestSaveClass");
            SaveSystem saveSystem = new SaveSystem(pathSaveFolder);

            await saveSystem.SaveGameAsync(save);

            //Check that the story and event are correctly saved
            string storyFilePath = Path.Combine(pathSaveFolder, "Test Story", "story.json");
            string eventFilePath = Path.Combine(pathSaveFolder, "Test Story", "current_event.json");

            Assert.True(File.Exists(storyFilePath), "Story file should exist at " + storyFilePath);
            Assert.True(File.Exists(eventFilePath), "Event file should exist at " + eventFilePath);

            save.DeleteSave(pathSaveFolder);

            Assert.False(File.Exists(storyFilePath), "Story file should not exist at " + storyFilePath);
            Assert.False(File.Exists(eventFilePath), "Event file should not exist at " + eventFilePath);

            string pathStoryFolder = Path.Combine(pathSaveFolder, save.Story.Title);
            Assert.False(Directory.Exists(pathStoryFolder), "Save folder should be deleted.");
        }
    }
}
