namespace View.Pages;
using View;

public partial class MainCreatorPage : ContentPage
{
	public MainCreatorPage()
	{
		InitializeComponent();
	}
    private async void OnGoToStoryListButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}