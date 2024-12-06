using Model;
using Model.Storage;
using System.Collections.ObjectModel;

namespace ViewModel
{
    public class SaveViewModel : BaseViewModel
    {
        #region Fields

        private readonly SaveSystem _saveSystem;
        private ObservableCollection<string> _availableSaves;
        private Save _currentSave;

        #endregion

        #region Properties 

        public ObservableCollection<string> AvailableSaves
        {
            get => _availableSaves;
            set => _availableSaves = value;
        }


        public Save CurrentSave
        {
            get => _currentSave;
            set => _currentSave = value;
        }

        #endregion

        public SaveViewModel()
        {
            _saveSystem = new SaveSystem();
            _availableSaves = new ObservableCollection<string>(GetAvailableSaves());
        }

        /// <summary>
        /// Save the game, overriding if necessary (uses the story's title as the folder name)
        /// </summary>
        /// <param name="story">Story the save belongs to</param>
        /// <param name="currentEvent">Event the player is currently in</param>
        /// <returns></returns>
        public async Task SaveGameAsync(Story story, Event currentEvent)
        {
            Save save = new Save(story, currentEvent)
            {
                SaveDate = DateTime.Now
            };

            await _saveSystem.SaveGameAsync(save);

            // Update the available saves list (if it's a new save)
            if (!_availableSaves.Contains(story.Title))
            {
                _availableSaves.Add(story.Title);
            }
        }

        /// <summary>
        /// Load a game by story title
        /// </summary>
        /// <param name="storyTitle">title of the story to load</param>
        /// <returns></returns>
        public async Task LoadGameAsync(string storyTitle)
        {
            _currentSave = await _saveSystem.LoadGameAsync(storyTitle);
        }

        /// <summary>
        /// Delete a save by story title
        /// </summary>
        /// <param name="storyTitle"></param>
        public void DeleteSave(string storyTitle)
        {
            _saveSystem.DeleteSave(storyTitle);
            _availableSaves.Remove(storyTitle); // Remove from the available saves list
        }

        private List<string> GetAvailableSaves()
        {
            return _saveSystem.GetAvailableSaves();
        }
    }
}
