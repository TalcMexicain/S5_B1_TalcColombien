using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using Model;
using Model.Items;

namespace TestViewModel
{
    public class TestOptionVM
    {
        private FakeStoryManager _fakeStoryManager;
        private StoryViewModel _storyViewModel;
        private EventViewModel _eventVM;

        public TestOptionVM()
        {
            _fakeStoryManager = new FakeStoryManager();
            _storyViewModel = new StoryViewModel(_fakeStoryManager);
            InitializeStoryVMAndEventVM();
            _eventVM = new EventViewModel(_storyViewModel);
        }

        [Fact]
        public void SetCurrentOption()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            Option option = new Option();

            optionVM.CurrentOption = option;

            Assert.Equal(option, optionVM.CurrentOption);
        }

        [Fact]
        public void OptionVM_Constructor_NoArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new OptionViewModel(null));
        }

        [Fact]
        public void OptionVM_Constructor_NullOption()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            Assert.NotNull(optionVM.CurrentOption);
            Assert.Equal(string.Empty, optionVM.CurrentOption.NameOption);
            Assert.Null(optionVM.CurrentOption.LinkedEvent);
        }

        [Fact]
        public async void UpdateOptionAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            string updatedName = "Updated Option";
            Option updateOption = new Option()
            {
                NameOption = updatedName,
                LinkedEvent = new Event(){ IdEvent = 1, Name = "Linked Event"}
            };

            await optionVM.UpdateOptionAsync(updateOption);

            Assert.Equal(updatedName, optionVM.CurrentOption.NameOption);
            Assert.NotNull(optionVM.CurrentOption.LinkedEvent);
            Assert.Equal("Linked Event", optionVM.CurrentOption.LinkedEvent.Name);
        }

        [Fact]
        public async Task UpdateOptionAsync_TestNullException()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            await Assert.ThrowsAsync<ArgumentNullException>(() => optionVM.UpdateOptionAsync(null));
        }

        [Fact]
        public async void InitializeNewOptionAsync()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            await optionVM.InitializeNewOptionAsync();

            Assert.NotNull(optionVM.CurrentOption);
            Assert.Single(_eventVM.Options);
            Assert.Equal(string.Empty, optionVM.CurrentOption.NameOption);
            Assert.Null(optionVM.CurrentOption.LinkedEvent);
            Assert.Equal(optionVM.GenerateNewOptionId() - 1, optionVM.CurrentOption.IdOption);
        }

        [Fact]
        public async void DeleteAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            Option optionToDelete = new Option()
            {
                IdOption = optionVM.GenerateNewOptionId(),
                NameOption = "Option",
                LinkedEvent = null
            };

            await _eventVM.AddOptionAsync(optionToDelete);

            Assert.Single(_eventVM.Options);

            await optionVM.DeleteAsync();
            Assert.Empty(_eventVM.Options);
        }

        [Fact]
        public async void AddWordAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            string wordToAdd = "NewWord";
            bool result = await optionVM.AddWordAsync(wordToAdd);

            Assert.True(result);
            Assert.Contains(wordToAdd, optionVM.Words);
        }

        [Fact]
        public async void AddWordAsync_TestExistingWord()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            string wordToAddTwice = "WordToAdd";
            await optionVM.AddWordAsync(wordToAddTwice);

            bool result = await optionVM.AddWordAsync(wordToAddTwice);

            Assert.False(result);
            Assert.Single(optionVM.Words);
        }

        [Fact]
        public async void RemoveWordAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            string wordToRemove = "Word";
            await optionVM.AddWordAsync(wordToRemove);

            bool result = await optionVM.RemoveWordAsync(wordToRemove);

            Assert.True(result);
            Assert.Empty(optionVM.Words);
        }

        [Fact]
        public async void RemoveWordAsync_TestNotFoundWord()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);

            string wordToRemove = "Word";

            bool result = await optionVM.RemoveWordAsync(wordToRemove);

            Assert.False(result);
            Assert.Empty(optionVM.Words);
        }

        [Fact]
        public async void AddRequiredItemAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);
            await optionVM.InitializeNewOptionAsync();

            KeyItem keyItem = new KeyItem();

            bool result = await optionVM.AddRequiredItemAsync(keyItem);

            Assert.True(result);
            Assert.Contains(keyItem, optionVM.RequiredItems);
            Assert.Contains(keyItem, optionVM.CurrentOption.GetRequiredItems());
        }

        [Fact]
        public async void AddRequiredItemAsync_TestExistingItem()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);
            await optionVM.InitializeNewOptionAsync();

            KeyItem keyItem = new KeyItem();

            await optionVM.AddRequiredItemAsync(keyItem);
            bool result = await optionVM.AddRequiredItemAsync(keyItem);

            Assert.False(result);
            Assert.Single(optionVM.RequiredItems);
        }

        [Fact]
        public async void RemoveRequiredItemAsync_Test()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);
            await optionVM.InitializeNewOptionAsync();

            KeyItem keyItem = new KeyItem();

            await optionVM.AddRequiredItemAsync(keyItem);
            bool result = await optionVM.RemoveRequiredItemAsync(keyItem);

            Assert.True(result);
            Assert.DoesNotContain(keyItem, optionVM.RequiredItems);
            Assert.DoesNotContain(keyItem, optionVM.CurrentOption.GetRequiredItems());
        }

        [Fact]
        public async void RemoveRequiredItemAsync_TestNotFound()
        {
            OptionViewModel optionVM = new OptionViewModel(_eventVM);
            await optionVM.InitializeNewOptionAsync();

            KeyItem keyItem = new KeyItem();

            bool result = await optionVM.RemoveRequiredItemAsync(keyItem);

            Assert.False(result);
            Assert.Empty(optionVM.RequiredItems);
        }


        private async void InitializeStoryVMAndEventVM()
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
    }
}
