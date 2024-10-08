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

        public EventViewModel()
        {
            Events = new ObservableCollection<Event>();
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
