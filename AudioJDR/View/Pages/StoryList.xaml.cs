using View.Pages;
using ViewModel;
using Model;

namespace View;

public partial class StoryList : ContentPage
{
    private StoryViewModel _viewModel;

    public StoryList()
    {
        InitializeComponent();
        _viewModel = new StoryViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadStories();
    }
    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryMap));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainCreatorPage));
    }
    private async void OnCreateNewStoryButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EventCreationPage));
    }
    
}
