using Model;

namespace UnitTests
{
    public class StoryTests
    {
        [Fact]
        public void Properties_Test()
        {
            string title = "Initial Title";
            string description = "Initial Description";
            string author = "Initial Author";

            Story story = new Story(title, description)
            {
                Author = author
            };

            story.Title = "Updated Title";
            story.Description = "Updated Description";
            story.Author = "Updated Author";

            Assert.Equal("Updated Title", story.Title);
            Assert.Equal("Updated Description", story.Description);
            Assert.Equal("Updated Author", story.Author);
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
        public void DeleteStory_Test()
        {
            Story story = new Story("Story Test", "Description");

            Event testEvent1 = new Event("Event1", "Event1 Description");
            Event testEvent2 = new Event("Event2", "Event2 Description");

            Option testOption1 = new Option(testEvent1);
            testEvent1.AddOption(testOption1);

            story.AddEvent(testEvent1);
            story.AddEvent(testEvent2);

            story.DeleteStory();

            Assert.Empty(story.Events);
        }
    }
}
