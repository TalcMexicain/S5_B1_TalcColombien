using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class OptionViewModel : BaseViewModel
    {
        public ObservableCollection<Option> Options { get; set; }

        public ICommand DeleteOptionCommand { get; }

        public OptionViewModel()
        {
            Options = new ObservableCollection<Option>();
            DeleteOptionCommand = new Command<Option>(DeleteOption);
        }

        /// <summary>
        /// Adds a new option with default text
        /// </summary>
        public void AddOption(string text = "", Event? linkedEvent = null)
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
