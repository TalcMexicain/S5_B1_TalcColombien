using System.Collections.ObjectModel;

namespace Model
{

    /// <summary>
    /// Represents a Story in audioJDR game that contains events
    /// All the code in this file is included in all platforms.
    /// </summary>
    public class Story
    {

        #region Fields

        private int idStory;
        private string title;
        private string description;
        private string author;

        private ObservableCollection<Event> events;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID of the story
        /// </summary>
        public int IdStory 
        { 
            get => idStory; 
            set => idStory = value; 
        }

        /// <summary>
        /// Gets or sets the title of the story
        /// </summary>
        public string Title 
        { 
            get => title; 
            set => title = value; 
        }

        /// <summary>
        /// Gets or sets the description of the story
        /// </summary>
        public string Description 
        { get => description; 
            set => description = value; 
        }

        /// <summary>
        /// Gets or sets the author of the story
        /// </summary>
        public string Author 
        { get => author; 
            set => author = value; 
        }

        /// <summary>
        /// Gets or sets the list of events in the story
        /// </summary>
        public ObservableCollection<Event> Events 
        { get => events;  
            set => events = value; 
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Story class with a title and description
        /// </summary>
        /// <param name="title">The title of the story</param>
        /// <param name="description">The description of the story</param>
        public Story(string title, string description)
        {
            this.events = new ObservableCollection<Event>();
            this.title = title;
            this.description = description;
        }

        /// <summary>
        /// Default constructor for serialization or initialization without parameters
        /// </summary>
        public Story()
        {
            this.events = new ObservableCollection<Event>();
        }
        #endregion

        /// <summary>
        /// Deletes the Story
        /// </summary>
        public void DeleteStory()
        {
            foreach (var evt in this.events.ToList())
            {
                DeleteEvent(evt); // Delete each event linked to the story
            }
        }

        /// <summary>
        /// Adds an event to the story
        /// </summary>
        /// <param name="evt">The event to add to the story</param>
        public void AddEvent(Event evt)
        {
            this.events.Add(evt);
        }

        /// <summary>
        /// Deletes the event the story
        /// </summary>
        /// <param name="evt">The event to delete</param>
        public void DeleteEvent(Event evt)
        {
            evt.DeleteEvent(); // Delete all options in the event
            this.events.Remove(evt);
        }
    }
}
