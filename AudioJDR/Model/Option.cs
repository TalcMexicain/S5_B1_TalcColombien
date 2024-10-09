using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Option
    {
        #region Attributs

        private int idOption;
        private string text;

        private Event? linkedEvent;
        #endregion

        #region Properties

        public int IdOption { get => idOption; set => idOption = value; }
        public string Text { get => text; set => text = value; }

        public Event? LinkedEvent { get => linkedEvent; set => linkedEvent = value; }
        #endregion

        #region Constructors
        
        public Option(string text, Event? linkedEvent)
        {
            Text = text;
            LinkedEvent = linkedEvent;
        }
        #endregion

        public void DeleteOption()
        {
            LinkedEvent = null;
        }

        public void LinkOptionToEvent(Event linkedEvent)
        {
            LinkedEvent = linkedEvent;
        }
    }
}
