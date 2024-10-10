using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace UnitTests
{
    public class OptionTests
    {
        [Fact]
        public void Properties_Test()
        {
            string text = "Initial Text";
            Event initialEvent = new Event("Initial Event", "Description");

            Option optionToTest = new Option(text, initialEvent);

            optionToTest.Text = "Updated Text";
            Event updatedEvent = new Event("Intial Event", "Description");
            optionToTest.LinkedEvent = updatedEvent;

            Assert.Equal("Updated Text", optionToTest.Text);
            Assert.Equal(updatedEvent, optionToTest.LinkedEvent);
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventSample = new Event("Initial Event", "Description");
            Option optionToTest = new Option("Option Text", eventSample);

            optionToTest.DeleteLinkedOption();

            Assert.Null(optionToTest.LinkedEvent);
        }

        [Fact]
        public void LinkOptionToEvent_Test()
        {
            Option optionToTest = new Option("Option Text", null);
            Assert.Null(optionToTest.LinkedEvent);

            Event eventSample = new Event("Initial Event", "Description");
            optionToTest.LinkOptionToEvent(eventSample);

            Assert.Equal(eventSample, optionToTest.LinkedEvent);
        }
    }
}
