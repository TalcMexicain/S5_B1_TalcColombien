using ViewModel;

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

    private async void OnMainPageButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new MainPage();
    }
}
