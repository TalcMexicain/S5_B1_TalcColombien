using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace TestViewModel
{
    public class TestStoryVM : IDisposable
    {
        private FakeStoryManager _fakeStoryManager;

        public TestStoryVM()
        {
            _fakeStoryManager = new FakeStoryManager();
        }

        [Fact]
        public void SetStory()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story story = new Story();
            storyVM.CurrentStory = story;
            Assert.Equal(story,storyVM.CurrentStory);
        }

        [Fact]
        public async void LoadStoriesAsync_Test()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story storyToAdd = GetOneTestStory();
            await _fakeStoryManager.SaveCurrentStoryAsync(storyToAdd);

            await storyVM.LoadStoriesAsync();
            Assert.Single(storyVM.Stories);
            Assert.Equal(storyToAdd.IdStory, storyVM.Stories[0].IdStory);
            Assert.Equal(storyToAdd.Title, storyVM.Stories[0].Title);
        }

        [Fact]
        public async void AddStoriesAsync_Test()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story storyToAdd = GetOneTestStory();
            await storyVM.AddStoryAsync(storyToAdd);

            Story savedStory = await _fakeStoryManager.LoadStory(storyToAdd.IdStory);
            Assert.Equal(savedStory.Title, storyToAdd.Title);

            Story? storyInVM = storyVM.Stories.FirstOrDefault(s => s.IdStory == storyToAdd.IdStory);
            Assert.NotNull(storyInVM);
            Assert.Equal(storyToAdd.IdStory, storyInVM.IdStory);
            Assert.Equal(storyToAdd.Description, storyInVM.Description);
        }

        [Fact]
        public void AddStoriesAsync_TestException()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story? nullStory = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => storyVM.AddStoryAsync(nullStory));
        }

        [Fact]
        public async void UpdateStoryAsync_Test()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            // Saving an existing story to the fakeStoryManager
            Story existingStory = GetOneTestStory();
            int storyId = existingStory.IdStory;
            await _fakeStoryManager.SaveCurrentStoryAsync(existingStory);

            // The story to update
            Story updatedStory = GetOneTestStory();
            string updatedTitle = "Updated Title";
            string updatedDescription = "Updated Description";
            updatedStory.Title = updatedTitle;
            updatedStory.Description = updatedDescription;

            // Initialize the ViewModel and update the story
            await storyVM.LoadStoriesAsync();
            storyVM.CurrentStory = existingStory;

            await storyVM.UpdateStoryAsync(storyId, updatedStory);

            Story? updatedStoryInVM = storyVM.Stories.FirstOrDefault(s => s.IdStory == storyId);

            Assert.NotNull(updatedStoryInVM);
            Assert.Equal(updatedTitle, updatedStoryInVM.Title);
            Assert.Equal(updatedDescription, updatedStoryInVM.Description);
            Assert.Equal(updatedStory.Events.Count, updatedStoryInVM.Events.Count);
        }

        [Fact]
        public void UpdateStoryAsync_TestException()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story? nullStory = null;

            Assert.ThrowsAsync<ArgumentException>(() => storyVM.UpdateStoryAsync(1,nullStory));
        }

        [Fact]
        public async void DeleteStoryAsync()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story storyToDelete = GetOneTestStory();
            await storyVM.AddStoryAsync(storyToDelete);

            // Verify if the story is saved
            Story? storyInVM = storyVM.Stories.FirstOrDefault(s => s.IdStory == storyToDelete.IdStory);
            Assert.Equal(storyToDelete, storyInVM);

            await storyVM.DeleteStoryAsync(storyToDelete.IdStory);
            Assert.Empty(storyVM.Stories);
            Assert.Empty(_fakeStoryManager.FakeManagerStories);
        }

        [Fact]
        public async void GetStoryByIdAsync_Test()
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story storyToAdd = GetOneTestStory();
            await _fakeStoryManager.SaveCurrentStoryAsync(storyToAdd);
            await storyVM.LoadStoriesAsync();

            Story fetchedStory = await storyVM.GetStoryByIdAsync(storyToAdd.IdStory);

            Assert.NotNull(fetchedStory);
            Assert.Equal(storyToAdd.IdStory, fetchedStory.IdStory);
            Assert.Equal(storyToAdd.Title, fetchedStory.Title);
            Assert.Equal(storyToAdd.Description, fetchedStory.Description);
            Assert.Equal(storyToAdd.Events.Count, fetchedStory.Events.Count);
        }

        [Fact]
        public void GetStoryById_TestException() 
        {
            StoryViewModel storyVM = new StoryViewModel(_fakeStoryManager);

            Story storyFirst = GetOneTestStory();

            int notFoundId = storyFirst.IdStory + 1;
            Assert.ThrowsAsync<KeyNotFoundException>(() => storyVM.GetStoryByIdAsync(notFoundId));
        }

        private Story GetOneTestStory()
        {
            Story story = new Story
            {
                Title = "Title",
                Description = "Description",
                IdStory = 1
            };

            Event firstEventStory1 = new Event
            {
                Name = "FirstEvent",
                Description = "Description"
            };

            Event eventStory1 = new Event
            {
                IdEvent = 1,
                Name = "Event",
                Description = "Description"
            };

            Option optionStory1 = new Option()
            {
                IdOption = 1,
                NameOption = "Option"
            };

            firstEventStory1.AddOption(optionStory1);
            story.AddEvent(firstEventStory1);
            story.AddEvent(eventStory1);

            return story;
        }

        /// <summary>
        /// Called after each test to clean the FakeManager saved stories
        /// </summary>
        public void Dispose()
        {
            _fakeStoryManager.ClearAllSavedStories();
        }
    }
}
