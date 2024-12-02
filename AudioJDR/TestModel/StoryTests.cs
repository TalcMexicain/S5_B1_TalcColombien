using Model;

namespace UnitTests
{
    public class StoryTests
    {
        [Fact]
        public void SetId()
        {
            Story story = new Story();
            int idStory = 1;
            story.IdStory = idStory;
            Assert.Equal(idStory, story.IdStory);
        }

        [Fact]
        public void SetTitle()
        {
            Story story = new Story();
            string title = "Title";
            story.Title = title;
            Assert.Equal(title, story.Title);
        }

        [Fact]
        public void SetDescription()
        {
            Story story = new Story();
            string description = "Description";
            story.Description = description;
            Assert.Equal(description, story.Description);
        }

        [Fact]
        public void SetFirstEvent()
        {
            Story story = new Story();
            Event evt = new Event();
            story.FirstEvent = evt;
            Assert.Equal(evt, story.FirstEvent);
        }

        [Fact]
        public void AddEvent_Test()
        {
            Story story = new Story("Story Test", "Description");

            Event eventToAdd = new Event("Event1", "Event1 Description");
            story.AddEvent(eventToAdd);

            Assert.Contains(eventToAdd, story.Events);
        }

        [Fact]
        public void RemoveEvent_Test()
        {
            Story story = new Story("Story Test", "Description");

            Event eventToAdd = new Event("Event1", "Event1 Description");

            story.AddEvent(eventToAdd);
            Assert.Contains(eventToAdd, story.Events);

            story.DeleteEvent(eventToAdd);
            Assert.DoesNotContain(eventToAdd, story.Events);
            Assert.Empty(story.Events);
        }

        [Fact]
        public void SetFirstEvent_Test()
        {
            Story story = new Story();
            Event evtFirst = new Event();
            Event evtNotFirst = new Event();

            story.AddEvent(evtFirst);
            story.AddEvent(evtNotFirst);

            story.SetFirstEvent(evtNotFirst);
            story.SetFirstEvent(evtFirst);

            Assert.True(story.FirstEvent.IsFirst);
            Assert.False(evtNotFirst.IsFirst);
        }

        [Fact]
        public void SetFirstEvent_TestException()
        {
            Story story = new Story();
            Event evtOne = new Event();
            story.AddEvent(evtOne);

            Event evtNotInStory = new Event();

            Assert.Throws<InvalidOperationException>(() => story.SetFirstEvent(evtNotInStory));
        }
    }
}
