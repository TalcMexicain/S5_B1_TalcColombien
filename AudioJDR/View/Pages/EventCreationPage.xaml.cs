using View.Pages;
using ViewModel;

namespace View;

public partial class EventCreationPage : ContentPage
{
    private readonly EventViewModel _viewModel;

    public EventCreationPage()
    {
        InitializeComponent();

        _viewModel = new EventViewModel();
        BindingContext = _viewModel;
    }

    private async void OnAddOptionClicked(object sender, EventArgs e)
    {
        //_viewModel.AddOption();
        await Navigation.PushAsync(new OptionCreationPage(_viewModel));
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryMap));
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StoryList));
    }

}