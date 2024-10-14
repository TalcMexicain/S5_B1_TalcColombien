using Microsoft.Maui.Controls;
using System.Globalization;
using View.Resources.Localization;

namespace View.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            SetResponsiveSizes();
            PickerInitialization();
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetResponsiveSizes();
        }

        private void SetResponsiveSizes()
        {
            // Use the current page size to set button sizes dynamically
            double pageWidth = this.Width;
            double pageHeight = this.Height;

            // Set minimum button sizes to prevent them from becoming too small
            double minButtonWidth = 250; // Adjust the minimum width for landscape
            double minButtonHeight = 60;

            // Set button sizes dynamically as a percentage of the current page size
            if (pageWidth > 0 && pageHeight > 0)
            {
                double buttonWidth = Math.Max(pageWidth * 0.35, minButtonWidth);
                double buttonHeight = Math.Max(pageHeight * 0.1, minButtonHeight);

                ThemeToggleButton.WidthRequest = buttonWidth;
                ThemeToggleButton.HeightRequest = buttonHeight;

                LanguagePicker.WidthRequest = Math.Max(pageWidth * 0.35, 300);
                LanguagePicker.HeightRequest = Math.Max(pageHeight * 0.1, 100);

                BackButton.WidthRequest = buttonWidth * 0.75;
                BackButton.HeightRequest = buttonHeight;

                // Adjust font size based on button width, with a maximum size to avoid overflow
                double buttonFontSize = Math.Min(buttonWidth * 0.08, 30); 

                ThemeToggleButton.FontSize = buttonFontSize;
                LanguagePicker.FontSize = buttonFontSize;
                BackButton.FontSize = buttonFontSize;

                // Adjust button padding to ensure text fits well
                ThemeToggleButton.Padding = new Thickness(20, 5);
                BackButton.Padding = new Thickness(20, 5);
            }
        }

        private void PickerInitialization()
        {
            // Set the items source for the picker
            LanguagePicker.ItemsSource = new List<string> { "English", "Français" };

            LanguagePicker.Loaded += (sender, e) =>
            {
                var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                if (currentLanguage == "en")
                {
                    LanguagePicker.SelectedItem = "English";
                }
                else if (currentLanguage == "fr")
                {
                    LanguagePicker.SelectedItem = "Français";
                }
                else
                {
                    LanguagePicker.SelectedItem = "English";
                }
            };
        }

        private void OnThemeButtonClicked(object sender, EventArgs e)
        {
            if (Application.Current.RequestedTheme == AppTheme.Light)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem?.ToString();
            var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if ((selectedLanguage == "English" && currentLanguage == "en") ||
                (selectedLanguage == "Français" && currentLanguage == "fr"))
            {
                return;
            }

            if (selectedLanguage == "English")
            {
                SetLanguage("en");
            }
            else if (selectedLanguage == "Français")
            {
                SetLanguage("fr");
            }
        }

        private void SetLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync(nameof(SettingsPage));
            });
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
    }
}
