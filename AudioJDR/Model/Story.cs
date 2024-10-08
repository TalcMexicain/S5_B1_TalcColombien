namespace Model
{
    // All the code in this file is included in all platforms.
    public class Story
    {
        public int IdStory { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public List<Event> Events { get; set; }

        public Story(string title, string description)
        {
            Events = new List<Event>();
            Title = title;
            Description = description;
        }

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
