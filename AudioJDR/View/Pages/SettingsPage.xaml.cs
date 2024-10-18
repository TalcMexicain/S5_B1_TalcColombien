using System.Globalization;

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

        /// <summary>
        /// Adjusts UI sizes when the page size changes.
        /// </summary>
        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetResponsiveSizes();
        }

        /// <summary>
        /// Adjusts the sizes of buttons and other UI elements dynamically based on the current page dimensions.
        /// Ensures that elements do not shrink or grow beyond reasonable limits.
        /// </summary>
        private void SetResponsiveSizes()
        {
            double pageWidth = this.Width;
            double pageHeight = this.Height;

            double minButtonWidth = 250;
            double minButtonHeight = 60;

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

                double buttonFontSize = Math.Min(buttonWidth * 0.08, 30);

                ThemeToggleButton.FontSize = buttonFontSize;
                LanguagePicker.FontSize = buttonFontSize;
                BackButton.FontSize = buttonFontSize;

                ThemeToggleButton.Padding = new Thickness(20, 5);
                BackButton.Padding = new Thickness(20, 5);
            }
        }

        /// <summary>
        /// Initializes the language picker with available languages (English, French).
        /// Automatically selects the current app language in the picker when the page loads.
        /// </summary>
        private void PickerInitialization()
        {
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

        /// <summary>
        /// Event handler for the Theme Toggle button click.
        /// Toggles between light and dark themes for the application.
        /// </summary>
        /// <param name="sender">The source of the event (the Theme Toggle button).</param>
        /// <param name="e">Event arguments.</param>
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
        /// Event handler triggered when the selected language in the picker is changed.
        /// Changes the app language if the selected language is different from the current one.
        /// </summary>
        /// <param name="sender">The source of the event (the Language Picker).</param>
        /// <param name="e">Event arguments.</param>
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

        /// <summary>
        /// Changes the app's language by setting the culture to the selected language code.
        /// Reloads the current page to apply the language changes.
        /// </summary>
        /// <param name="languageCode">The language code ("en" for English, "fr" for French).</param>
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
