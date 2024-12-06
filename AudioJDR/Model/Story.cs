using Model.Resources.Localization;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        private Event firstEvent;

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
        {
            get => description;
            set => description = value;
        }

        /// <summary>
        /// Gets or sets the list of events in the story
        /// </summary>
        public ObservableCollection<Event> Events
        {
            get => events;
            set => events = value;
        }

        /// <summary>
        /// Gets or sets the first event in the story.
        /// </summary>
        public Event FirstEvent 
        {
            get => firstEvent;
            set => firstEvent = value;
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

        #region Public Methods

        /// <summary>
        /// Adds an event to the story
        /// </summary>
        /// <param name="evt">The event to add to the story</param>
        public void AddEvent(Event evt)
        {
            this.events.Add(evt);
        }

        /// <summary>
        /// Deletes the event from the story and updates linked options.
        /// </summary>
        /// <param name="evt">The event to delete.</param>
        public void DeleteEvent(Event evt)
        {

            // Iterate through all events in the story
            foreach (var eventInStory in events)
            {
                // Check each option in the current event
                foreach (var option in eventInStory.Options.ToList())
                {
                    // If the option is linked to the event being deleted, unlink it
                    if (option.LinkedEvent == evt)
                    {
                        option.LinkedEvent = eventInStory; // Link to the current event
                    }
                }
            }

            evt.DeleteEvent(); // Delete all options in the event
            this.events.Remove(evt); // Remove the event from the story
        }

        /// <summary>
        /// Sets the specified event as the first event, ensuring no other event is marked as first.
        /// </summary>
        /// <param name="evt">The event to set as first.</param>
        /// <exception cref="InvalidOperationException">Exception thrown when the event doesn't belong to the story's events</exception>
        public void SetFirstEvent(Event evt)
        {
            if (!events.Contains(evt))
            {
                throw new InvalidOperationException(AppResourcesModel.Story_SetFirstEvent_InvalidOperationException);
            }

            if (firstEvent != null)
            {
                // Remove the first status from the current first event
                FirstEvent.IsFirst = false; // Ensure the current first event is marked as not first
            }

            firstEvent = evt;
            firstEvent.IsFirst = true; // Mark the new event as first
        }

        #endregion
    }
}
