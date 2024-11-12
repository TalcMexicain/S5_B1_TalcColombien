using View.Pages;

namespace View
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainCreatorPage), typeof(MainCreatorPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(StoryList), typeof(StoryList));
            Routing.RegisterRoute(nameof(EventCreationPage), typeof(EventCreationPage));
            Routing.RegisterRoute(nameof(OptionCreationPage), typeof(OptionCreationPage));
            Routing.RegisterRoute(nameof(StoryMap), typeof(StoryMap));

            Routing.RegisterRoute(nameof(YourStories), typeof(YourStories));
            Routing.RegisterRoute(nameof(PlayPage), typeof(PlayPage));
        }
    }
}
