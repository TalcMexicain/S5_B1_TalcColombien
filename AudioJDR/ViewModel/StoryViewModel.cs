using Model;
using System.Collections.ObjectModel;

namespace ViewModel
{
    public class StoryViewModel : BaseViewModel
    {
        public ObservableCollection<Story> Stories { get; set; }

        public ObservableCollection<Event> Events => SelectedStory?.Events;

        public StoryViewModel()
        {
            Stories = new ObservableCollection<Story>();
        }

        private Story _selectedStory;
        public Story SelectedStory
        {
            get => _selectedStory;
            set
            {
                _selectedStory = value;
                OnPropertyChanged(nameof(SelectedStory));
                OnPropertyChanged(nameof(Events)); // Notify when the Events collection changes
            }
        }

        /// <summary>
        /// Load stories (temp) to display them on the StoryList page
        /// </summary>
        public void LoadStories()
        {
            Stories.Clear();
            ObservableCollection<Event> events = new ObservableCollection<Event>
            {
                new Event("EVENEMENT 1", "Event one description"),
                new Event("EVENEMENT 2", "Event two description"),
                new Event("EVENEMENT 3", "Event three description"),
                new Event("EVENEMENT 4", "Event four description"),
            };
            Stories.Add(new Story("AVENTURE 1", "Fantaisie médiévale") { IdStory = 1, Events = events });
            Stories.Add(new Story("AVENTURE 2", "Thriller/Drame") { IdStory = 2, Events = new ObservableCollection<Event>() });
            Stories.Add(new Story("AVENTURE 3", "Action") { IdStory = 3, Events = new ObservableCollection<Event>() });
            Stories.Add(new Story("AVENTURE 4", "Yaume") { IdStory = 4, Events = new ObservableCollection<Event>() });
        }

        public Story GetStoryById(int storyId)
        {
            return Stories.FirstOrDefault(s => s.IdStory == storyId);
        }

        public int GenerateNewStoryId()
        {
            // Generate a new ID based on the highest existing ID + 1
            if (Stories.Count == 0)
                return 1; // If there are no stories, start with ID 1

            return Stories.Max(s => s.IdStory) + 1; // Get the highest current ID and increment
        }

        /// <summary>
        /// Adds a story with name and description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddStory(string name, string description)
        {
            var newStory = new Story(name, description);
            Stories.Add(newStory);
        }

        /// <summary>
        /// Deletes a story
        /// </summary>
        /// <param name="story"></param>
        public void DeleteStory(Story story)
        {
            if (story != null)
            {
                story.DeleteStory();
                Stories.Remove(story);
            }
        }
    }
}
