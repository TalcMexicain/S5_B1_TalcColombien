using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Storage
{
    public class Save
    {
        public int IdSave { get; set; }
        public DateTime SaveDate { get; set; }
        public Story Story { get; set; }
        public Event CurrentEvent { get; set; }

        public Save(Story story, Event actualEvent)
        {
            Story = story;
            CurrentEvent = actualEvent;
            SaveDate = DateTime.Now;
        }

        /// <summary>
        /// Deletes the save.
        /// </summary>
        public void DeleteSave()
        {
            SaveSystem saveSystem = new SaveSystem();
            saveSystem.DeleteSave(Story.Title);
        }
    }
}
