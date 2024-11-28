using System.Globalization;

namespace View.Pages
{
    public partial class SettingsPage : ContentPage
    {
        #region Constructor

        public SettingsPage()
        {
            InitializeComponent();
            SetResponsiveSizes();
            PickerInitialization();
            this.SizeChanged += OnSizeChanged;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Adjusts UI sizes when the page size changes.
        /// </summary>
        private void OnSizeChanged(object? sender, EventArgs e)
        {
            SetResponsiveSizes();
        }

        #endregion

        #region UI Management

        /// <summary>
        /// Adjusts the sizes of buttons and other UI elements dynamically based on the current page dimensions.
        /// Ensures that elements do not shrink or grow beyond reasonable limits.
        /// </summary>
        private void SetResponsiveSizes()
        {
            double pageWidth = this.Width;
            double pageHeight = this.Height;

            if (pageWidth > 0 && pageHeight > 0)
            {
                UIHelper.SetButtonSize(this, ThemeToggleButton, false);
                UIHelper.SetButtonSize(this, BackButton, true);

                LanguagePicker.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.BUTTON_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
                LanguagePicker.HeightRequest = Math.Max(pageHeight * UIHelper.Sizes.BUTTON_HEIGHT_FACTOR, UIHelper.Sizes.MIN_EDITOR_HEIGHT);
            }
        }

        #endregion

        #region Picker Initialization

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

        #endregion

        #region Theme and Language Management

        /// <summary>
        /// Event handler for the Theme Toggle button click.
        /// Toggles between light and dark themes for the application.
        /// </summary>
        private void OnThemeButtonClicked(object sender, EventArgs e)
        {
            Application.Current.UserAppTheme = Application.Current.RequestedTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        }

        /// <summary>
        /// Event handler triggered when the selected language in the picker is changed.
        /// Changes the app language if the selected language is different from the current one.
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem?.ToString();
            var currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if ((selectedLanguage == "English" && currentLanguage == "en") ||
                (selectedLanguage == "Français" && currentLanguage == "fr"))
            {
                return;
            }

            SetLanguage(selectedLanguage == "English" ? "en" : "fr");
        }

        /// <summary>
        /// Changes the app's language by setting the culture to the selected language code.
        /// Reloads the current page to apply the language changes.
        /// </summary>
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

        #endregion

        #region Navigation

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }

        #endregion
    }
}
