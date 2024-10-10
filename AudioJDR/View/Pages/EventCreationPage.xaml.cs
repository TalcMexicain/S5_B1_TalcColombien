using ViewModel;

namespace View.Pages;

public partial class EventCreationPage : ContentPage
{
    private readonly OptionViewModel _viewModel;

    public EventCreationPage()
    {
        InitializeComponent();

        _viewModel = new OptionViewModel();
        BindingContext = _viewModel;
    }

    
    private void OnAddChoiceClicked(object sender, EventArgs e)
    {
        _viewModel.AddOption();
    }

    
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }

}