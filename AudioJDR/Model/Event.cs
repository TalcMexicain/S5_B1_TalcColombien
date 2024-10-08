using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// An Event
    /// </summary>
    public class Event
    {
        public int IdEvent { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Option> Options { get; set; }

        public Event(string name, string description)
        {
            Options = new List<Option>();
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Deletes an event
        /// </summary>
        public void DeleteEvent()
        {
            foreach (var option in Options.ToList())
            {
                DeleteOption(option); // Delete each option linked to this event
            }
        }

        /// <summary>
        /// Adds an option to the event
        /// </summary>
        /// <param name="option"></param>
        public void AddOption(Option option)
        {
            Options.Add(option);
        }

        /// <summary>
        /// Removes the option from the event.
        /// </summary>
        /// <param name="option"></param>
        public void DeleteOption(Option option)
        {
            Options.Remove(option);
        }
    }
}
