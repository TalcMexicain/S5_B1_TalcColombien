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
        #region Attributs

        private int idEvent;
        private string name;
        private string description;

        private List<Option> options;
        #endregion

        #region Properties

        public int IdEvent { get => idEvent; set => idEvent = value; }

        public string Name { get => name; set => name = value; }

        public string Description { get => description; set => description = value; }

        public List<Option> Options { get => options; set => options = value; }
        #endregion

        #region Constructors

        public Event(string name, string description)
        {
            this.options = new List<Option>();
            this.name = name;
            this.description = description;
        }
        #endregion

        /// <summary>
        /// Deletes an event
        /// </summary>
        public void DeleteEvent()
        {
            foreach (var option in this.options.ToList())
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
            this.options.Add(option);
        }

        /// <summary>
        /// Removes the option from the event.
        /// </summary>
        /// <param name="option"></param>
        public void DeleteOption(Option option)
        {
            this.options.Remove(option);
        }
    }
}
