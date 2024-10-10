using Model;
using System.Collections.ObjectModel;

namespace ViewModel
{
    public class StoryViewModel : BaseViewModel
    {
        public ObservableCollection<Story> Stories { get; set; }
        public StoryViewModel()
        {
            Stories = new ObservableCollection<Story>();
        }
        /// <summary>
        /// Load stories (temp) to display them on the StoryList page
        /// </summary>
        public void LoadStories()
        {
            Stories.Clear();
            Stories.Add(new Story("AVENTURE 1", "Fantaisie médiévale") { Events = new List<Event>()});
            Stories.Add(new Story("AVENTURE 2", "Thriller/Drame") { Events = new List<Event>()} );
            Stories.Add(new Story("AVENTURE 3", "Action") { Events = new List<Event>() });
            Stories.Add(new Story("AVENTURE 4", "yaume") { Events = new List<Event>() });

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
