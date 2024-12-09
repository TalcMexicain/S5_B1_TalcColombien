using Model;
using Model.Storage;

namespace UnitTests
{
    public class StorySaveSystemTests : IDisposable
    {
        private readonly StorySaveSystem _storySaveSystem;
        private readonly string _testFolder;

        public StorySaveSystemTests()
        {
            _testFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TestStoriesSaves");

            if (Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, true);
            }

            _storySaveSystem = new StorySaveSystem(_testFolder);
        }

        [Fact]
        public async Task SaveStoryAsync_ShouldCreateFile()
        {
            Story storyTest = new Story
            {
                IdStory = 1,
                Title = "Test Story",
                Description = "A test story"
            };

            Event event1 = new Event() 
            { 
                IdEvent = 1, 
                Name = "Event 1", 
                Description = "Description 1" 
            };

            Option option1 = new Option {
                IdOption = 1,
                NameOption = "Option 1",
                LinkedEvent = event1 
            };

            event1.AddOption(option1);
            storyTest.Events.Add(event1);

            string storyFilePath = Path.Combine(_testFolder, $"{storyTest.IdStory}.json");

            await _storySaveSystem.SaveStoryAsync(storyTest);

            Assert.True(File.Exists(storyFilePath), "Story File should exist at : " + storyFilePath);
        }


        [Fact]
        public async Task LoadStoryAsync_ShouldReturnCorrectStory()
        {

            Story storyTest = new Story
            {
                IdStory = 1,
                Title = "Test Story",
                Description = "A test story"
            };

            Event event1 = new Event("Event 1", "Description 1") { IdEvent = 1 };
            Option option1 = new Option { IdOption = 1, NameOption = "Option 1", LinkedEvent = event1 };
            event1.AddOption(option1);
            storyTest.Events.Add(event1);

            Event event2 = new Event("Event 2", "Description 2") { IdEvent = 2 };
            Option option2 = new Option { IdOption = 2, NameOption = "Option 2", LinkedEvent = event2 };
            event2.AddOption(option2);
            storyTest.Events.Add(event2);

            await _storySaveSystem.SaveStoryAsync(storyTest);

            // Act
            var loadedStory = await _storySaveSystem.LoadStoryAsync(storyTest.IdStory);

            // Checking the story
            Assert.NotNull(loadedStory);
            Assert.Equal(storyTest.IdStory, loadedStory.IdStory);
            Assert.Equal(storyTest.Title, loadedStory.Title);
            Assert.Equal(storyTest.Description, loadedStory.Description);

            // Checking the events in story
            Assert.Equal(storyTest.Events.Count, loadedStory.Events.Count);

            // Checking first event and its options
            var loadedEvent1 = loadedStory.Events.FirstOrDefault(e => e.IdEvent == event1.IdEvent);
            Assert.NotNull(loadedEvent1);
            Assert.Equal(event1.Name, loadedEvent1.Name);
            Assert.Equal(event1.Description, loadedEvent1.Description);
            Assert.Equal(event1.GetOptions().Count, loadedEvent1.GetOptions().Count);

            var loadedOption1 = loadedEvent1.GetOptions().FirstOrDefault(o => o.IdOption == option1.IdOption);
            Assert.NotNull(loadedOption1);
            Assert.Equal(option1.NameOption, loadedOption1.NameOption);
            Assert.Equal(option1.LinkedEvent.IdEvent, loadedOption1.LinkedEvent.IdEvent);

            // Checking second event and its options
            var loadedEvent2 = loadedStory.Events.FirstOrDefault(e => e.IdEvent == event2.IdEvent);
            Assert.NotNull(loadedEvent2);
            Assert.Equal(event2.Name, loadedEvent2.Name);
            Assert.Equal(event2.Description, loadedEvent2.Description);
            Assert.Equal(event2.GetOptions().Count, loadedEvent2.GetOptions().Count);

            var loadedOption2 = loadedEvent2.GetOptions().FirstOrDefault(o => o.IdOption == option2.IdOption);
            Assert.NotNull(loadedOption2);
            Assert.Equal(option2.NameOption, loadedOption2.NameOption);
            Assert.Equal(option2.LinkedEvent.IdEvent, loadedOption2.LinkedEvent.IdEvent);
        }

        [Fact]
        public void DeleteStory_ShouldRemoveFile()
        {

            Story story = new Story
            {
                IdStory = 3,
                Title = "Story to Delete",
                Description = "Story for delete test"
            };

            string storyFilePath = Path.Combine(_testFolder, $"{story.IdStory}.json");

            // Save the story
            _storySaveSystem.SaveStoryAsync(story).Wait();

            _storySaveSystem.DeleteStory(story.IdStory);

            // Assert
            Assert.False(File.Exists(storyFilePath));
        }

        [Fact]
        public async Task GetSavedStoriesAsync_ShouldReturnListOfStories()
        {
            Story story1 = new Story { IdStory = 4, Title = "Story 1", Description = "First story" };
            Story story2 = new Story { IdStory = 5, Title = "Story 2", Description = "Second story" };

            await _storySaveSystem.SaveStoryAsync(story1);
            await _storySaveSystem.SaveStoryAsync(story2);

            var savedStories = await _storySaveSystem.GetSavedStoriesAsync();

            // Assert
            Assert.Equal(2, savedStories.Count);
            Assert.Contains(savedStories, s => s.IdStory == story1.IdStory);
            Assert.Contains(savedStories, s => s.IdStory == story2.IdStory);
        }

        [Fact]
        public void GetAvailableStories_ShouldReturnListOfStoryIds()
        {

            Story story1 = new Story { IdStory = 6, Title = "Story 1", Description = "First story" };
            Story story2 = new Story { IdStory = 7, Title = "Story 2", Description = "Second story" };

            _storySaveSystem.SaveStoryAsync(story1).Wait();
            _storySaveSystem.SaveStoryAsync(story2).Wait();

            var storyIds = _storySaveSystem.GetAvailableStories();

            // Assert
            Assert.Equal(2, storyIds.Count);
            Assert.Contains(story1.IdStory, storyIds);
            Assert.Contains(story2.IdStory, storyIds);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, true);
            }
        }
    }
}
