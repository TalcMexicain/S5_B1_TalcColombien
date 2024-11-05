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
        private List<string> words;
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
            this.words = new List<string>();
        }

        /// <summary>
        /// Default constructor for serialization or initialization without parameters.
        /// </summary>
        public Option()
        {
            this.words = new List<string>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Add a word to the list of words if it is not in the list
        /// </summary>
        /// <param name="word">The word to add</param>
        public void AddWordInList(string word)
        {
            if (!this.words.Contains(word))
            {
                this.words.Add(word);
            }
        }

        /// <summary>
        /// Remove a word to the list of words if it exist in list
        /// </summary>
        /// <param name="word">The word to remove</param>
        public void RemoveWordInList(string word)
        {
            if (this.words.Contains(word))
            {
                this.words.Remove(word);
            }
        }

        /// <summary>
        /// Get the list of words that will trigger the option
        /// </summary>
        /// <returns>The list of words</returns>
        public List<string> GetWords()
        {
            return this.words;
        }

        /// <summary>
        /// Checks if the list is not empty or null
        /// </summary>
        /// <returns>True if the list has at least one element otherwise false</returns>
        public bool IsWordsListNotEmpty()
        {
            return this.words != null && this.words.Any();
        }

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

        #endregion
    }
}
