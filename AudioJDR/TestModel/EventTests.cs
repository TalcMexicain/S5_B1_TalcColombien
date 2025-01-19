using Model;
using Model.Characters;
using Model.Items;

namespace UnitTests
{
    public class EventTests
    {

        [Fact]
        public void SetId_Test()
        {
            Event evt = new Event();
            int idEvent = 1;
            evt.IdEvent = idEvent;
            Assert.Equal(idEvent, evt.IdEvent);
        }

        [Fact]
        public void SetName_Test()
        {
            Event evt = new Event();
            string name = "Event";
            evt.Name = name;
            Assert.Equal(name, evt.Name);
        }

        [Fact]
        public void SetDescription_Test()
        {
            Event evt = new Event();
            string desc = "Description";
            evt.Description = desc;
            Assert.Equal(desc, evt.Description);
        }

        [Fact]
        public void SetIsFirst_Test()
        {
            Event evt = new Event();
            bool firstEvent = true;
            evt.IsFirst = firstEvent;
            Assert.True(evt.IsFirst);
        }

        [Fact]
        public void SetEnemy_Test()
        {
            Event evt = new Event();
            Enemy enemy = new Enemy();
            evt.Enemy = enemy;
            Assert.Equal(enemy, evt.Enemy);
        }

        [Fact]
        public void AddItem_Test()
        {
            Event evt = new Event();
            KeyItem keyItem = new KeyItem();
            evt.AddItem(keyItem);
            List<Item> items = evt.GetItemsToPickUp();
            Assert.NotEmpty(items);
            Assert.Single(items);
        }

        [Fact]
        public void GetItemsToPickUp_Test()
        {
            Event evt = new Event();
            KeyItem keyItem = new KeyItem();
            evt.AddItem(keyItem);
            List<Item> items = evt.GetItemsToPickUp();
            items.Add(keyItem);
            Assert.NotEmpty(evt.GetItemsToPickUp());
            Assert.Single(evt.GetItemsToPickUp());
        }

        [Fact]
        public void RemoveItem_Test()
        {
            Event evt = new Event();
            KeyItem keyItem = new KeyItem();
            evt.AddItem(keyItem);
            evt.RemoveItem(keyItem);
            List<Item> items = evt.GetItemsToPickUp();
            Assert.Empty(items);
        }


        [Fact]
        public void AddOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToAdd = new Option(eventToTest);
            eventToTest.AddOption(optionToAdd);

            Assert.Contains(optionToAdd, eventToTest.GetOptions());
        }

        [Fact]
        public void DeleteOption_Test()
        {
            Event eventToTest = new Event("Event Name", "Event Description");

            Option optionToDelete = new Option(eventToTest);
            eventToTest.AddOption(optionToDelete);
            Assert.Contains(optionToDelete, eventToTest.GetOptions());

            eventToTest.DeleteOption(optionToDelete);
            Assert.Empty(eventToTest.GetOptions());
            Assert.DoesNotContain(optionToDelete, eventToTest.GetOptions());
        }

        [Fact]
        public void DeleteEvent_Test()
        {
            Event eventToTest = new Event();
            eventToTest.AddOption(new Option());
            eventToTest.AddOption(new Option());

            eventToTest.DeleteEvent();

            Assert.Empty(eventToTest.GetOptions());
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
