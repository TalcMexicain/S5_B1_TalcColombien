using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Storage
{
    /// <summary>
    /// Represents a saved game state including the story and the current event.
    /// </summary>
    public class Save
    {

        #region Fields

        private int idSave;
        private DateTime saveDate;
        private Story story;
        private Event currentEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the id for the save
        /// </summary>
        public int IdSave { get => idSave; set => idSave = value; }

        /// <summary>
        /// Gets or sets the date when the save as been made
        /// </summary>
        public DateTime SaveDate { get => saveDate; set => saveDate = value; }

        /// <summary>
        /// Gets or sets the story associated with the save
        /// </summary>
        public Story Story { get => story; set => story = value; }

        /// <summary>
        /// Gets or sets the current event at the time the game save
        /// </summary>
        public Event CurrentEvent { get => currentEvent; set => currentEvent = value; }

        #endregion

        /// <summary>
        /// Constructor's of the class Save
        /// </summary>
        /// <param name="story">The story to be saved</param>
        /// <param name="actualEvent">The current event when the story as been saved</param>
        public Save(Story story, Event actualEvent)
        {
            Story = story;
            CurrentEvent = actualEvent;
            SaveDate = DateTime.Now;
        }

        /// <summary>
        /// Deletes the save.
        /// </summary>
        /// <param name="savingPath"></param>
        public void DeleteSave(string savingPath = null)
        {
            SaveSystem saveSystem = new SaveSystem(savingPath);
            saveSystem.DeleteSave(this.story.Title);
        }
    }
}
