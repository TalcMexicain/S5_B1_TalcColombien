using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace ViewModel
{
    public class OptionViewModel : BaseViewModel
    {
        public ObservableCollection<Option> Options { get; set; }

        public OptionViewModel()
        {
            Options = new ObservableCollection<Option>();
        }

        /// <summary>
        /// Adds a new option with text and linked event
        /// </summary>
        /// <param name="text"></param>
        /// <param name="linkedEvent"></param>
        public void AddOption(string text, Event? linkedEvent)
        {
            var newOption = new Option(text, linkedEvent);
            Options.Add(newOption);
        }

        /// <summary>
        /// Deletes an option
        /// </summary>
        /// <param name="option"></param>
        public void DeleteOption(Option option)
        {
            if (option != null)
            {
                option.DeleteLinkedOption();
                Options.Remove(option);
            }
        }
    }
}
