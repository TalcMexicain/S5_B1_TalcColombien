using View.Pages;

namespace View
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainCreatorPage), typeof(MainCreatorPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
