using ViewModel;
using Model;

namespace View;

public partial class StoryMap : ContentPage
{
    public StoryMap()
    {
        InitializeComponent();
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}