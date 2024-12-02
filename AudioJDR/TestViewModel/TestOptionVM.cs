using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using Model;

namespace TestViewModel
{
    public class TestOptionVM
    {
        private StoryViewModel _storyVM;

        public TestOptionVM() 
        {
            Story story = new Story();
            story.IdStory = 1;
            story.AddEvent(new Event());

            _storyVM = new StoryViewModel();
            _storyVM.CurrentStory = story;
        }

        [Fact]
        public void SetCurrentOption()
        {
            EventViewModel eventVM = new EventViewModel(_storyVM);
            OptionViewModel optionVM = new OptionViewModel(eventVM);

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
            EventViewModel eventVM = new EventViewModel(_storyVM);
            OptionViewModel optionVM = new OptionViewModel(eventVM);

            Assert.NotNull(optionVM.CurrentOption);
            Assert.Equal(string.Empty, optionVM.CurrentOption.NameOption);
            Assert.Null(optionVM.CurrentOption.LinkedEvent);
        }

        [Fact]
        public async Task UpdateOptionAsync_NullException()
        {
            EventViewModel eventVM = new EventViewModel(_storyVM);
            OptionViewModel optionVM = new OptionViewModel(eventVM);

            await Assert.ThrowsAsync<ArgumentNullException>(() => optionVM.UpdateOptionAsync(null));
        }
    }
}
