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

        private List<Event> events;
        #endregion

        #region Properties
        public int IdStory { get => idStory; set => idStory = value; }

        public string Title { get => title; set => title = value; }

        public string Description { get => description; set => description = value; }

        public string Author { get => author; set => author = value; }

        public List<Event> Events { get => events;  set => events = value; }

        #endregion

        #region Constructors

        public Story(string title, string description)
        {
            this.events = new List<Event>();
            this.title = title;
            this.description = description;
        }
        #endregion

        /// <summary>
        /// Deletes the Story
        /// </summary>
        public void DeleteStory()
        {
            foreach (var evt in Events.ToList())
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
            Events.Add(evt);
        }

        /// <summary>
        /// Deletes the event the story
        /// </summary>
        /// <param name="evt"></param>
        public void DeleteEvent(Event evt)
        {
            evt.DeleteEvent(); // Delete all options in the event
            Events.Remove(evt);
        }
    }
}
