using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace ViewModel
{
    public class EventViewModel : BaseViewModel
    {
        public ObservableCollection<Event> Events { get; set; }
        public ObservableCollection<Option> Options { get; set; }

        public EventViewModel()
        {
            Events = new ObservableCollection<Event>();
            Options = new ObservableCollection<Option>();
        }

        /// <summary>
        /// Load events (temp) to display them in the OptionCreationPage List event
        /// </summary>
        public void LoadEvent()
        {
            Events.Clear();
            Events.Add(new Event("EVENEMENT 1", "Event one description"));
            Events.Add(new Event("EVENEMENT 2", "Event two description"));
            Events.Add(new Event("EVENEMENT 3", "Event three description"));
            Events.Add(new Event("EVENEMENT 4", "Event four description"));
        }

        /// <summary>
        /// Load events options (temp) to display them in the EventCreationPage List Of Option
        /// </summary>
        public void LoadEventsOptions()
        {
            foreach(Event @event in Events)
            {
                @event.Options.Add(new Option("Text Option", null) { NameOption = "OPTION 1" });
                @event.Options.Add(new Option("Text Option", null) { NameOption = "OPTION 2" });
                @event.Options.Add(new Option("Text Option", null) { NameOption = "OPTION 3" });
            }
        }

        /// <summary>
        /// Adds a new event with name and description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddEvent(string name, string description)
        {
            var newEvent = new Event(name, description);
            Events.Add(newEvent);
        }


        public void AddOption(Option option)
        {
            Options.Add(option);
        }

        /// <summary>
        /// Deletes an event
        /// </summary>
        /// <param name="ev"></param>
        public void DeleteEvent(Event ev)
        {
            if (ev != null)
            {
                ev.DeleteEvent();
                Events.Remove(ev);
            }
        }
    }
}
