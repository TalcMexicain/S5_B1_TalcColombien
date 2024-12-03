using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Storage;

namespace TestViewModel
{
    public class FakeStoryManager : IStoryManager
    {
        private List<Story> _fakeManagerStories;

        public List<Story> FakeManagerStories
        {
            get => _fakeManagerStories;
        }

        public FakeStoryManager() 
        {
            _fakeManagerStories = new List<Story>();
        }

        public Task SaveCurrentStoryAsync(Story story)
        {
            _fakeManagerStories.Add(story);
            return Task.CompletedTask;
        }

        public void DeleteStory(int storyId)
        {
            Story? storyToDelete = null;

            foreach (Story story in _fakeManagerStories)
            {
                if (story.IdStory == storyId)
                {
                    storyToDelete = story;
                }
            }

            if (storyToDelete != null)
            {
                _fakeManagerStories.Remove(storyToDelete);
            }
        }

        public Task<List<Story>> GetSavedStoriesAsync()
        {
            return Task.FromResult(_fakeManagerStories);
        }

        public Task<Story> LoadStory(int storyId)
        {
            return Task.FromResult(_fakeManagerStories.FirstOrDefault(s => s.IdStory == storyId));
        }

        public void ClearAllSavedStories()
        {
            _fakeManagerStories.Clear();
        }
    }
}
