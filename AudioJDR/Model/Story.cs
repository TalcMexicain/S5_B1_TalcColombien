using Model.Resources.Localization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Model.Characters;

using Model.Items;

namespace Model
{

    /// <summary>
    /// Represents a Story
    /// </summary>
    public class Story : INotifyPropertyChanged
    {

        #region Fields

        private int idStory;
        private string title;
        private string description;
        private Event firstEvent;

        private ObservableCollection<Event> events;
        private Player player;
        private ObservableCollection<Enemy> enemies;
        private ObservableCollection<Item> items;

        #region Bound Properties 

        public event PropertyChangedEventHandler? PropertyChanged;

        public string NewGameText => AppResourcesModel.NewGame;
        public string ContinueText => AppResourcesModel.Continue;
        public string DeleteText => AppResourcesModel.Delete;

        #endregion

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

        /// <summary>
        /// Gets or sets the player associated with the story.
        /// </summary>
        public Player Player
        {
            get => player;
            set => player = value;
        }

        /// <summary>
        /// Gets or sets the list of enemies in the story. - unused
        /// </summary>
        public ObservableCollection<Enemy> Enemies
        {
            get => enemies;
            set => enemies = value;
        }

        /// <summary>
        /// Gets or sets the list of items in the story.
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => items;
            set => items = value;
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
            this.enemies = new ObservableCollection<Enemy>();
            this.items = new ObservableCollection<Item>();
            this.title = title;
            this.description = description;
            this.player = new Player("bob",100,10); // Base player - values will be used by default.
        }

        /// <summary>
        /// Default constructor for serialization or initialization without parameters
        /// </summary>
        public Story()
        {
            this.events = new ObservableCollection<Event>();
            this.enemies = new ObservableCollection<Enemy>();
            this.items = new ObservableCollection<Item>();
            this.player = new Player("bob", 100, 10);
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
            foreach (Event eventInStory in events)
            {
                // Check each option in the current event
                foreach (Option option in eventInStory.GetOptions())
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

        /// <summary>
        /// Adds an enemy to the story.
        /// </summary>
        /// <param name="enemy">The enemy to add.</param>
        public void AddEnemy(Enemy enemy)
        {
            this.enemies.Add(enemy);
        }

        /// <summary>
        /// Removes an enemy from the story.
        /// </summary>
        /// <param name="enemy">The enemy to remove.</param>
        public void RemoveEnemy(Enemy enemy)
        {
            this.enemies.Remove(enemy);
        }

        /// <summary>
        /// Adds an item to the story's inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item item)
        {
            this.items.Add(item);
        }

        /// <summary>
        /// Removes an item from the story's inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Item item)
        {
            this.items.Remove(item);
        }

        /// <summary>
        /// Refreshes Binding related properties
        /// </summary>
        public void RefreshLocalizedProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewGameText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContinueText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeleteText)));
        }

        #endregion
    }
}
