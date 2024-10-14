using System.Collections.ObjectModel;

namespace Model
{
    // All the code in this file is included in all platforms.
    public class Story
    {

        #region Attributs

        private int idStory;
        private string title;
        private string description;
        private string author;

        private ObservableCollection<Event> events;
        #endregion

        #region Properties

        public int IdStory { get => idStory; set => idStory = value; }

        public string Title { get => title; set => title = value; }

        public string Description { get => description; set => description = value; }

        public string Author { get => author; set => author = value; }

        public ObservableCollection<Event> Events { get => events;  set => events = value; }

        #endregion

        #region Constructors

        public Story(string title, string description)
        {
            this.events = new ObservableCollection<Event>();
            this.title = title;
            this.description = description;
        }

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
        /// <param name="evt"></param>
        public void AddEvent(Event evt)
        {
            this.events.Add(evt);
        }

        /// <summary>
        /// Deletes the event the story
        /// </summary>
        /// <param name="evt"></param>
        public void DeleteEvent(Event evt)
        {
            evt.DeleteEvent(); // Delete all options in the event
            this.events.Remove(evt);
        }
    }
}
