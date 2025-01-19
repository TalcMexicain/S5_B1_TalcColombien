using Model;
using System.Diagnostics;
using System.Globalization;
using View.Resources.Localization;
using ViewModel;

namespace View.Pages
{
    public partial class SettingsPage : ContentPage
    {
        #region Const

        private const string decimalFormatOneDigit = "F1";
        private const float r = 0.5f;
        private const int v = 10;
        #endregion

        #region Fields

        private GlobalSettingsViewModel _globalSettingsViewModel;
        private SpeechSynthesizerViewModel _speechViewModel;
        private SpeechRecognitionViewModel _recognitionViewModel;
        private string PageContext;

        #endregion

        #region Constructor

        public SettingsPage(ISpeechSynthesizer speechSynthesizer, ISpeechRecognition speechRecognition)
        {
            InitializeComponent();
            SetResponsiveSizes();
            this.SizeChanged += OnSizeChanged;
            this._globalSettingsViewModel = new GlobalSettingsViewModel(speechSynthesizer);
            this._speechViewModel = new SpeechSynthesizerViewModel(speechSynthesizer);
            _recognitionViewModel = new SpeechRecognitionViewModel(speechRecognition);

            PageContext = "SettingsPage";

            _recognitionViewModel.NavigatePrevious += async () => await NavigatePrevious();
            _recognitionViewModel.ChangeTheme += async () => await ChangeTheme();
            _recognitionViewModel.ChangeLanguageEN += async () => await ChangeLanguageEN();
            _recognitionViewModel.ChangeLanguageFR += async () => await ChangeLanguageFR();
            _recognitionViewModel.TestVoice += async () => await TestVoice();
            _recognitionViewModel.IncreaseVolume += async () => await IncreaseVolume();
            _recognitionViewModel.DecreaseVolume += async () => await DecreaseVolume();
            _recognitionViewModel.IncreaseSpeed += async () => await IncreaseSpeed();
            _recognitionViewModel.DecreaseSpeed += async () => await DecreaseSpeed();
            InitializeSettingsElements();
        }
        #endregion

        #region Event Handlers


        protected override void OnAppearing()
        {
            base.OnAppearing();
            var keywords = new[] { AppResources.Back, AppResources.ChangeTheme, AppResources.LanguageEN, AppResources.LanguageFR, AppResources.TestVoice, AppResources.IncreaseVolume, AppResources.DecreaseVolume, AppResources.IncreaseSpeed, AppResources.Decrease_Speed };
            _recognitionViewModel.StartRecognition(keywords, PageContext);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _recognitionViewModel.UnloadGrammars();
            _recognitionViewModel.StopRecognition();

        }

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
                UIHelper.SetButtonSize(this, TestVoiceButton, true);
                UIHelper.SetButtonSize(this, BackButton, true);

                LanguagePicker.WidthRequest = Math.Max(pageWidth * UIHelper.Sizes.BUTTON_WIDTH_FACTOR, UIHelper.Sizes.MIN_FRAME_WIDTH);
                LanguagePicker.HeightRequest = Math.Max(pageHeight * UIHelper.Sizes.BUTTON_HEIGHT_FACTOR, UIHelper.Sizes.MIN_EDITOR_HEIGHT);
            }
        }

        #endregion

        #region Elements Initialization

        private void InitializeSettingsElements()
        {
            LanguagePickerInitialization();
            VoiceTypeTTSPickerInitialization();
            VolumeSliderInitialization();
            RateSliderInitialization();
        }

        private void LanguagePickerInitialization()
        {
            Dictionary<string, string> languageMapping = new Dictionary<string, string>
            {
                {"en", "English" },
                {"fr", "Français" }
            };

            LanguagePicker.ItemsSource = new List<string>(languageMapping.Values);

            LanguagePicker.Loaded += (sender, e) =>
            {
                string currentLanguageCode = this._globalSettingsViewModel.Language;

                if (!string.IsNullOrEmpty(currentLanguageCode) && languageMapping.ContainsKey(currentLanguageCode))
                {
                    LanguagePicker.SelectedItem = languageMapping[currentLanguageCode];
                }
            };
        }

        private void VoiceTypeTTSPickerInitialization()
        {
            VoiceTypeTTSPicker.ItemsSource = this._globalSettingsViewModel.AvailableVoicesTypeTTS;
            VoiceTypeTTSPicker.SelectedItem = this._globalSettingsViewModel.VoiceTypeTTS;
        }

        private void VolumeSliderInitialization()
        {
            // Initialize label value for slider
            int valueVolumeTTS = this._globalSettingsViewModel.VolumeTTS;
            VolumeValueLabel.Text = valueVolumeTTS.ToString();

            // Initialize slider's value
            VolumeSlider.Value = valueVolumeTTS;
        }

        private void RateSliderInitialization()
        {
            // Initialize label value for slider 
            float valueRateTTS = this._globalSettingsViewModel.RateTTS;
            RateValueLabel.Text = valueRateTTS.ToString(decimalFormatOneDigit);

            // Initialize slider's value
            RateSlider.Value = valueRateTTS;
        }

        #endregion

        #region Language Management

        private async Task ChangeLanguageEN()
        {
            SetLanguage("en");
            MessagingCenter.Send(this, "LanguageChanged");

            
        }

        private async Task ChangeLanguageFR()
        {
            SetLanguage("fr");
            MessagingCenter.Send(this, "LanguageChanged");
            
        }

        /// <summary>
        /// Event handler triggered when the selected language in the picker is changed.
        /// Changes the app language if the selected language is different from the current one.
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            string? selectedLanguage = LanguagePicker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                if (selectedLanguage == "English")
                {
                    SetLanguage("en");
                }
                else if (selectedLanguage == "Français")
                {
                    SetLanguage("fr");
                }
                MessagingCenter.Send(this, "LanguageChanged");
            }

        }

        /// <summary>
        /// Changes the app's language by setting the culture to the selected language code.
        /// Reloads the current page to apply the language changes.
        /// </summary>
        private void SetLanguage(string languageCode)
        {
            CultureInfo culture = new CultureInfo(languageCode);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            this._globalSettingsViewModel.Language = languageCode;

            UpdateUIWithNewLanguage();
        }

        private void UpdateUIWithNewLanguage()
        {
            AppResources.Culture = new CultureInfo(_globalSettingsViewModel.Language);

            PageSettingsTitle.Text = AppResources.Settings;

            ThemeToggleButton.Text = AppResources.ToggleTheme;
            BackButton.Text = AppResources.Back;
            LanguagePicker.Title = AppResources.SelectLanguage;

            SynthesisTitleLabel.Text = AppResources.SynthesisSettingsLabel;
            VoiceVolumeLabel.Text = AppResources.VoiceVolume;
            VoiceRateLabel.Text = AppResources.VoiceSpeed;
            TestVoiceButton.Text = AppResources.TestVoice;
            VoiceTypeTTSPicker.Title = AppResources.VoiceType;
        }

        #endregion

        #region Theme Management

        private async Task ChangeTheme()
        {
            AppTheme appRequestedTheme = Application.Current.RequestedTheme;
            AppTheme appNewTheme = AppTheme.Unspecified;

            if (appRequestedTheme == AppTheme.Light)
            {
                appNewTheme = AppTheme.Dark;
            }
            else
            {
                appNewTheme = AppTheme.Light;
            }

            //Change the current theme
            Application.Current.UserAppTheme = appNewTheme;

            //Saves the theme in settings
            this._globalSettingsViewModel.AppTheme = appNewTheme;
        }

        /// <summary>
        /// Event handler for the Theme Toggle button click.
        /// Toggles between light and dark themes for the application.
        /// </summary>
        private async void OnThemeButtonClicked(object sender, EventArgs e)
        {
            await ChangeTheme();
        }

        #endregion

        #region VoiceType Management

        private void OnVoiceTypeChanged(object sender, EventArgs e)
        {
            string? selectedVoiceType = VoiceTypeTTSPicker.SelectedItem.ToString();
            this._globalSettingsViewModel.VoiceTypeTTS = selectedVoiceType;
        }

        #endregion

        #region VolumeTTS Management

        private async Task DecreaseVolume()
        {
            if (this._globalSettingsViewModel.VolumeTTS >= 10)
            {
                this._globalSettingsViewModel.VolumeTTS -= v;
            }
            VolumeSlider.Value = this._globalSettingsViewModel.VolumeTTS;
            VolumeValueLabel.Text = this._globalSettingsViewModel.VolumeTTS.ToString(); 
        }

        private async Task IncreaseVolume()
        {
            if (this._globalSettingsViewModel.VolumeTTS <= 90) {
                this._globalSettingsViewModel.VolumeTTS += v;
            }
            

            VolumeSlider.Value = this._globalSettingsViewModel.VolumeTTS;
            VolumeValueLabel.Text = this._globalSettingsViewModel.VolumeTTS.ToString();
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            int volumeSliderValue = (int)Math.Round(e.NewValue);
            VolumeValueLabel.Text = volumeSliderValue.ToString();

            SaveVolumeTTSToSettings(volumeSliderValue);
        }

        private void SaveVolumeTTSToSettings(int volumeToSave)
        {
            this._globalSettingsViewModel.VolumeTTS = volumeToSave;
        }

        #endregion

        #region RateTTS Management 


        private async Task DecreaseSpeed()
        {
            if(this._globalSettingsViewModel.RateTTS > 1)
            {
                this._globalSettingsViewModel.RateTTS -= r;
            }

            RateSlider.Value = this._globalSettingsViewModel.RateTTS;
            RateValueLabel.Text = this._globalSettingsViewModel.RateTTS.ToString("F1");
        }

        private async Task IncreaseSpeed()
        {
            if(this._globalSettingsViewModel.RateTTS <= 1.5)
            {
                this._globalSettingsViewModel.RateTTS += r;
            }

            RateSlider.Value = this._globalSettingsViewModel.RateTTS;
            RateValueLabel.Text = this._globalSettingsViewModel.RateTTS.ToString("F1");
        }

        private void OnRateChanged(object sender, ValueChangedEventArgs e)
        {
            float rateSliderValue = (float)e.NewValue;
            RateValueLabel.Text = rateSliderValue.ToString(decimalFormatOneDigit);

            SaveRateTTSToSettings(rateSliderValue);
        }

        private void SaveRateTTSToSettings(float rateToSave)
        {
            this._globalSettingsViewModel.RateTTS = rateToSave;
        }

        #endregion

        #region Voice Test Management
        private async Task TestVoice()
        {
            TestVoiceWithNewSettings();
        }

        private async void OnTestVoiceButtonClicked(object sender, EventArgs e)
        {
            await TestVoice();
        }

        private void TestVoiceWithNewSettings()
        {
            string sampleText = AppResources.SampleVoiceText;
            _speechViewModel.TextToSynthesize = sampleText;
            _speechViewModel.StopSynthesis();
            _speechViewModel.SynthesizeText();
        }

        #endregion

        #region Navigation

        private async Task NavigatePrevious()
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await NavigatePrevious();
        }

        #endregion
    }
}
