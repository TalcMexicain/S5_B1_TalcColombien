using ViewModel;
using Model;

namespace View.Pages;

public partial class OptionCreationPage : ContentPage
{

    private readonly EventViewModel eventViewModel;
    private readonly OptionViewModel optionViewModel;

	public OptionCreationPage(EventViewModel eventVM)
	{
		InitializeComponent();
        eventViewModel = eventVM;
        optionViewModel = new OptionViewModel();
        BindingContext = eventViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //  Load events dynamically
        eventViewModel.LoadEvent();
    }

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        Option option = new Option();
        option.NameOption = this.OptionNameEntry.Text;
        option.Text = this.OptionTextWord.Text;

        // Assign the selected event to the LinkedEvent property of the option
        if (EventPicker.SelectedItem is Event selectedEvent)
        {
           
            option.LinkedEvent = selectedEvent;
        }
        else
        {
            option.LinkedEvent = null;
            DisplayAlert("Erreur", "Veuillez sélectionner un événement.", "OK");
        }

        eventViewModel.AddOption(option);
        await Shell.Current.GoToAsync(nameof(EventCreationPage));
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EventCreationPage));
    }
}