using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    /// <summary>
    /// Represents an Event in a Story.
    /// </summary>
    public class Event
    {
        #region Fields

        private int idEvent;
        private string name;
        private string description;
        private List<Option> options;

        #endregion

        #region Properties

        public int IdEvent
        {
            get => idEvent;
            set => idEvent = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public List<Option> Options
        {
            get => options;
            set => options = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with name and description parameters.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="description">Description of the event.</param>
        public Event(string name, string description)
        {
            this.options = new List<Option>();
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Default constructor for serialization or initialization without parameters.
        /// </summary>
        public Event()
        {
            this.options = new List<Option>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an event and removes its options.
        /// </summary>
        public void DeleteEvent()
        {
            foreach (var option in this.options.ToList())
            {
                DeleteOption(option); // Delete each option linked to this event
            }
        }

        /// <summary>
        /// Adds an option to the event.
        /// </summary>
        /// <param name="option">The option to add to this event.</param>
        public void AddOption(Option option)
        {
            this.options.Add(option);
        }

        /// <summary>
        /// Removes an option from the event.
        /// </summary>
        /// <param name="option">The option to remove from this event.</param>
        public void DeleteOption(Option option)
        {
            this.options.Remove(option);
        }

        /// <summary>
        /// Generates a new unique Id for the event.
        /// This should be handled by the system creating the event.
        /// </summary>
        public static int GenerateNewEventId(List<Event> existingEvents)
        {
            int returnId = 1;

            if (existingEvents != null && existingEvents.Count > 0)
            {
                returnId = existingEvents.Max(e => e.IdEvent) + 1;
            }
                
            return returnId;
        }

        #endregion
    }
}
