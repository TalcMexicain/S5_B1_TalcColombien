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
            LanguagePicker.Title = $"{AppResources.SelectLanguage} ({AppResources.RestartWarning})";
        }

        /// <summary>
        /// Handles the theme toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnThemeButtonClicked(object sender, EventArgs e)
        {
            // Toggle between light and dark themes
            if (Application.Current.RequestedTheme == AppTheme.Light)
            {
                Application.Current.UserAppTheme = AppTheme.Dark; // Switch to dark theme
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light; // Switch to light theme
            }
        }

        /// <summary>
        /// Handles language change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem.ToString();

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

            // Refresh the app
            Application.Current.MainPage = new AppShell();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Go back to the previous page
        }
    }
}
