using Model;
using System.Globalization;
using ViewModel;

namespace View
{
    public partial class App : Application
    {
        #region Fields

        private GlobalSettingsViewModel _globalSettingsVM;

        #endregion

        #region Constructor

        public App(ISpeechSynthesizer speechSynthesizer)
        {
            InitializeComponent();

            MainPage = new AppShell();
            this._globalSettingsVM = new GlobalSettingsViewModel(speechSynthesizer);
            InitializeAppSettings();
        }

        #endregion

        #region Private Methods 

        private void InitializeAppSettings()
        {
            // Initialize Language
            string cultureCode = this._globalSettingsVM.Language;
            SetCulture(cultureCode);

            // Initialize AppTheme
            if (_globalSettingsVM.AppTheme == AppTheme.Light)
            {
                Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                Current.UserAppTheme = AppTheme.Dark;
            }

            // Initialize Voice VolumeTTS
            this._globalSettingsVM.ApplyTTSVoiceVolume();

            // Initialize Voice RateTTS
            this._globalSettingsVM.ApplyTTSVoiceRate();

            // Initialize Voice Type
            this._globalSettingsVM.ApplyTTSVoiceType();
        }

        /// <summary>
        /// Changes the language of the app
        /// </summary>
        /// <param name="cultureCode">Country code of wanted language (e.g. 'fr')</param>
        private void SetCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        #endregion
    }
}
