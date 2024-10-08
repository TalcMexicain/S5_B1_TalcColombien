using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Option
    {
        public int IdOption { get; set; }
        public string Text { get; set; }

        public Event? LinkedEvent { get; set; }

        public Option(string text, Event? linkedEvent)
        {
            Text = text;
            LinkedEvent = linkedEvent;
        }

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
