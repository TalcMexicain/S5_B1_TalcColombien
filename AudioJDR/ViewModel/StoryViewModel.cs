using Model;
using System.Collections.ObjectModel;

namespace ViewModel
{
    // All the code in this file is included in all platforms.
    public class StoryViewModel : BaseViewModel
    {
        public ObservableCollection<Story> Stories { get; set; }

        public StoryViewModel()
        {
            Stories = new ObservableCollection<Story>();
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
