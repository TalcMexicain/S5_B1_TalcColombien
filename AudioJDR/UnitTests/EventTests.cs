using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model;

namespace UnitTests
{
    public class EventTests
    {
        [Fact]
        public void Properties_Test()
        {
            string name = "Initial Name";
            string description = "Initial Description";

            Event eventToTest = new Event(name, description);

            eventToTest.Name = "Updated Name";
            eventToTest.Description = "Updated Description";

            Assert.Equal("Updated Name", eventToTest.Name);
            Assert.Equal("Updated Description", eventToTest.Description);
        }

        [Fact]
        public void AddOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToAdd = new Option("Option 1", eventToTest);
            eventToTest.AddOption(optionToAdd);

            Assert.Contains(optionToAdd, eventToTest.Options);
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToDelete = new Option("Option 1", eventToTest);
            eventToTest.AddOption(optionToDelete);
            Assert.Contains(optionToDelete, eventToTest.Options);

            eventToTest.DeleteOption(optionToDelete);
            Assert.Empty(eventToTest.Options);
            Assert.DoesNotContain(optionToDelete, eventToTest.Options);

        }

        [Fact]
        public void GenerateNewEventId_Test()
        {

            List<Event> eventListTest =
            [
                new Event("Event 1", "Description 1") { IdEvent = 1 },
                new Event("Event 2", "Description 2") { IdEvent = 2 },
                new Event("Event 3", "Description 3") { IdEvent = 3 },
            ];

            int newId = Event.GenerateNewEventId(eventListTest);

            Assert.Equal(4, newId);
        }

        [Fact]
        public void GenerateNewEventId_EmptyList_Test()
        {
            List<Event> emptyEventList = new List<Event>();

            int newId = Event.GenerateNewEventId(emptyEventList);

            Assert.Equal(1, newId); 
        }

        [Fact]
        public void GenerateNewEventId_NullList_Test()
        {
            int newId = Event.GenerateNewEventId(null);

            Assert.Equal(1, newId);
        }
    }
}
