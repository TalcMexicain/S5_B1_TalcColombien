using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace TestViewModel
{
    public class TestEventVM : IDisposable
    {
        private FakeStoryManager _fakeStoryManager;
        private StoryViewModel _storyViewModel;

        public TestEventVM() 
        {
            _fakeStoryManager = new FakeStoryManager();
            _storyViewModel = new StoryViewModel(_fakeStoryManager);
            InitializeStoryVM();
        }

        [Fact]
        public void SetCurrentEvent()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            Event currentEvent = new Event();
            eventVM.CurrentEvent = currentEvent;

            Assert.Equal(currentEvent, eventVM.CurrentEvent);
        }

        [Fact]
        public async void UpdateEventAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            Event existingEvent = new Event()
            {
                IdEvent = 1,
                Name = "Old Event",
                Description = "Description"
            };

            await _storyViewModel.AddEventAsync(existingEvent);
            eventVM.CurrentEvent = existingEvent;

            string nameEvent = "Updated Event";
            string descriptionEvt = "Updated Description";
            Event updatedEvent = new Event
            {
                IdEvent = 1,
                Name = nameEvent,
                Description = descriptionEvt
            };

            await eventVM.UpdateEventAsync(updatedEvent);

            Assert.Equal(nameEvent, eventVM.CurrentEvent.Name);
            Assert.Equal(descriptionEvt, eventVM.CurrentEvent.Description);
        }

        [Fact]
        public async void UpdateEventAsync_TestException()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            Event? nullEvent = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() => eventVM.UpdateEventAsync(nullEvent));
        }

        [Fact]
        public async void InitializeNewEventAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            await eventVM.InitializeNewEventAsync();

            Assert.NotNull(eventVM.CurrentEvent);
            Assert.Equal(_storyViewModel.Events.Count, eventVM.CurrentEvent.IdEvent);
            Assert.Equal(string.Empty, eventVM.CurrentEvent.Name);
            Assert.Equal(string.Empty, eventVM.CurrentEvent.Description);
            Assert.Empty(eventVM.CurrentEvent.GetOptions());

            Assert.Single(_storyViewModel.Events);
            Assert.Equal(eventVM.CurrentEvent, _storyViewModel.CurrentStory.Events.First());
        }

        [Fact]
        public async void DeleteEventAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            Event eventToDelete = new Event
            {
                IdEvent = 1,
                Name = "Event to Delete",
                Description = "Description"
            };

            await _storyViewModel.AddEventAsync(eventToDelete);
            eventVM.CurrentEvent = eventToDelete;

            await eventVM.DeleteEventAsync();
            Assert.Empty(_storyViewModel.CurrentStory.Events);
        }

        [Fact]
        public async void AddOptionAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);
            await eventVM.InitializeNewEventAsync();

            Option optionToAdd = new Option
            {
                IdOption = 1,
                NameOption = "New Option"
            };

            await eventVM.AddOptionAsync(optionToAdd);

            Assert.Single(eventVM.Options);
            Assert.Equal(optionToAdd.IdOption, eventVM.Options.First().CurrentOption.IdOption);
            Assert.Equal(optionToAdd.NameOption, eventVM.Options.First().CurrentOption.NameOption);
        }

        [Fact]
        public async void AddOptionAsync_TestException()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            Option? nullOption = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => eventVM.AddOptionAsync(nullOption));
        }

        [Fact]
        public async void DeleteOptionAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            await eventVM.InitializeNewEventAsync();

            Option optionToDelete = new Option
            {
                IdOption = 1,
                NameOption = "New Option"
            };

            await eventVM.AddOptionAsync(optionToDelete);

            await eventVM.DeleteOptionAsync(optionToDelete.IdOption);

            Assert.Empty(eventVM.CurrentEvent.GetOptions());
            Assert.Empty(eventVM.Options);
        }

        [Fact]
        public async void GetOptionViewModelAsync_Test()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);
            await eventVM.InitializeNewEventAsync();

            Option newOption = new Option
            {
                IdOption = 1,
                NameOption = "Option"
            };
            await eventVM.AddOptionAsync(newOption);

            OptionViewModel optionVM = await eventVM.GetOptionViewModelAsync(newOption.IdOption);

            Assert.NotNull(optionVM);
            Assert.Equal(newOption.IdOption,optionVM.CurrentOption.IdOption);
            Assert.Equal(newOption.NameOption, optionVM.CurrentOption.NameOption);
        }

        [Fact]
        public async void GetOptionViewModelAsync_TestException()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);
            await eventVM.InitializeNewEventAsync();

            int idNoOption = 1;
            await Assert.ThrowsAsync<ArgumentNullException>(() => eventVM.GetOptionViewModelAsync(idNoOption));
        }

        [Fact]
        public async void GenerateNewEventId()
        {
            EventViewModel eventVM = new EventViewModel(_storyViewModel);

            int newEventId = eventVM.GenerateNewEventId();
            Assert.Equal(1, newEventId);
        }

        private async void InitializeStoryVM()
        {
            Story story = new Story()
            {
                IdStory = 1,
                Title = "Title",
                Description = "Description"
            };

            _storyViewModel.CurrentStory = story;
            await _storyViewModel.AddStoryAsync(story);
        }

        public void Dispose()
        {
            _fakeStoryManager.ClearAllStoryEvents();
        }
    }
}
