using Model;

namespace UnitTests
{
    public class EventTests
    {

        [Fact]
        public void SetId()
        {
            Event evt = new Event();
            int idEvent = 1;
            evt.IdEvent = idEvent;
            Assert.Equal(idEvent, evt.IdEvent);
        }

        [Fact]
        public void SetName()
        {
            Event evt = new Event();
            string name = "Event";
            evt.Name = name;
            Assert.Equal(name, evt.Name);
        }

        [Fact]
        public void SetDescription()
        {
            Event evt = new Event();
            string desc = "Description";
            evt.Description = desc;
            Assert.Equal(desc, evt.Description);
        }

        [Fact]
        public void SetIsFirst()
        {
            Event evt = new Event();
            bool firstEvent = true;
            evt.IsFirst = firstEvent;
            Assert.True(evt.IsFirst);
        }

        [Fact]
        public void AddOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToAdd = new Option(eventToTest);
            eventToTest.AddOption(optionToAdd);

            Assert.Contains(optionToAdd, eventToTest.Options);
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToDelete = new Option(eventToTest);
            eventToTest.AddOption(optionToDelete);
            Assert.Contains(optionToDelete, eventToTest.Options);

            eventToTest.DeleteOption(optionToDelete);
            Assert.Empty(eventToTest.Options);
            Assert.DoesNotContain(optionToDelete, eventToTest.Options);
        }

        [Fact]
        public void DeleteEvent_Test()
        {
            Event eventToTest = new Event();
            eventToTest.AddOption(new Option());
            eventToTest.AddOption(new Option());

            eventToTest.DeleteEvent();

            Assert.Empty(eventToTest.Options);
        }

        [Fact]
        public void GenereteNewEventId_EmptyList_Test()
        {
            List<Event> events = new List<Event>();

            int newEventId = 0;
            newEventId = Event.GenerateNewEventId(events);
            Assert.Empty(events);
            Assert.Equal(1,newEventId);
        }

        [Fact]
        public void GenereteNewEventId_NotEmptyList_Test()
        {
            List<Event> existingEvents = new List<Event>
            {
                new Event { IdEvent = 1, Name = "Event 1", Description = "Description 1" },
                new Event { IdEvent = 2, Name = "Event 2", Description = "Description 2" },
                new Event { IdEvent = 3, Name = "Event 3", Description = "Description 3" }
            };

            int newEventId = 0;
            newEventId = Event.GenerateNewEventId(existingEvents);
            Assert.Equal(4, newEventId);
        }
    }
}
