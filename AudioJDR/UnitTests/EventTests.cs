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
    }
}
