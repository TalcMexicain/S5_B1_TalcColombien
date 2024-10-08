using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class SaveViewModel : BaseViewModel
    {
        private readonly SaveSystem _saveSystem;

        public ObservableCollection<string> AvailableSaves { get; set; }
        public Save CurrentSave { get; set; }

        public SaveViewModel()
        {
            _saveSystem = new SaveSystem();
            AvailableSaves = new ObservableCollection<string>(GetAvailableSaves());
        }

        /// <summary>
        /// Save the game, overriding if necessary (uses the story's title as the folder name)
        /// </summary>
        /// <param name="story">Story the save belongs to</param>
        /// <param name="currentEvent">Event the player is currently in</param>
        /// <returns></returns>
        public async Task SaveGameAsync(Story story, Event currentEvent)
        {
            var save = new Save(story,currentEvent)
            {
                SaveDate = DateTime.Now
            };

            await _saveSystem.SaveGameAsync(save);

            // Update the available saves list (if it's a new save)
            if (!AvailableSaves.Contains(story.Title))
            {
                AvailableSaves.Add(story.Title);
            }
        }

        /// <summary>
        /// Load a game by story title
        /// </summary>
        /// <param name="storyTitle">title of the story to load</param>
        /// <returns></returns>
        public async Task LoadGameAsync(string storyTitle)
        {
            CurrentSave = await _saveSystem.LoadGameAsync(storyTitle);
        }

        /// <summary>
        /// Delete a save by story title
        /// </summary>
        /// <param name="storyTitle"></param>
        public void DeleteSave(string storyTitle)
        {
            _saveSystem.DeleteSave(storyTitle);
            AvailableSaves.Remove(storyTitle); // Remove from the available saves list
        }

        private List<string> GetAvailableSaves()
        {
            return _saveSystem.GetAvailableSaves();
        }
    }
}
