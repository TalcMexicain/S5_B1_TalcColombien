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
            PickerInitialization();
        }

        /// <summary>
        /// Initializes the picker with the current language or sets it to the default language.
        /// </summary>
        private void PickerInitialization()
        {
            // Set the items source for the picker
            LanguagePicker.ItemsSource = new List<string> { "English", "French" };

            // Delay setting the selected item until the ItemsSource is fully initialized
            LanguagePicker.Loaded += (sender, e) =>
            {
                // Get the current language of the system (two-letter ISO code)
                var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                // Set the picker to the current language
                if (currentLanguage == "en")
                {
                    LanguagePicker.SelectedItem = "English";
                }
                else if (currentLanguage == "fr")
                {
                    LanguagePicker.SelectedItem = "French";
                }
                else
                {
                    // If the current language is not supported, default to English
                    LanguagePicker.SelectedItem = "English";
                }
            };
        }

        /// <summary>
        /// Handles the theme toggle
        /// </summary>
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

        /// <summary>
        /// Handles language change
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem?.ToString();
            var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            // Check if the selected language is already in use
            if ((selectedLanguage == "English" && currentLanguage == "en") ||
                (selectedLanguage == "French" && currentLanguage == "fr"))
            {
                // If the selected language is the same as the current language, do nothing
                return;
            }

            // If language is different, apply the change
            if (selectedLanguage == "English")
            {
                SetLanguage("en");
            }
            else if (selectedLanguage == "French")
            {
                SetLanguage("fr");
            }
        }

        /// <summary>
        /// Sets the application language
        /// </summary>
        /// <param name="languageCode"></param>
        private void SetLanguage(string languageCode)
        {
            // Set the culture for the app
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
