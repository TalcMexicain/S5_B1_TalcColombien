using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Represents an Option in a Event
    /// </summary>
    public class Option
    {
        #region Fields

        private int idOption;
        private string nameOption;
        private string text;

        private Event? linkedEvent;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the option
        /// </summary>
        public int IdOption 
        { 
            get => idOption; 
            set => idOption = value; 
        }

        /// <summary>
        /// Gets or sets the name of the option
        /// </summary>
        public string NameOption 
        { 
            get => nameOption; 
            set => nameOption = value; 
        }

        /// <summary>
        /// Gets or sets the text of the words that will trigger the option
        /// </summary>
        public string Text 
        { 
            get => text; 
            set => text = value; 
        }

        /// <summary>
        /// Gets or sets the event to which this option is linked
        /// </summary>
        public Event? LinkedEvent 
        { 
            get => linkedEvent; 
            set => linkedEvent = value; 
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Option class with specified text and linked event.
        /// </summary>
        /// <param name="text">The text/word that will trigger the option</param>
        /// <param name="linkedEvent">The event to which the option is linked</param>
        public Option(string text, Event? linkedEvent)
        {
            this.text = text;
            this.linkedEvent = linkedEvent;
        }

        /// <summary>
        /// Default constructor for serialization or initialization without parameters.
        /// </summary>
        public Option() 
        {

        }
        #endregion

        /// <summary>
        /// Removes the link between this option and the event.
        /// </summary>
        public void DeleteLinkedOption()
        {
            this.linkedEvent = null;
        }

        /// <summary>
        /// Links this option to a specific event
        /// </summary>
        /// <param name="linkedEvent">The event to link to this option</param>
        public void LinkOptionToEvent(Event linkedEvent)
        {
            this.linkedEvent = linkedEvent;
        }
    }
}
