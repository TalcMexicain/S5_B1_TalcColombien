using Model;
using System.Diagnostics;
using System.Globalization;
using ViewModel;

namespace View
{
    public partial class App : Application
    {
        #region Fields
        
        private GlobalSettingsViewModel globalSettings;

        #endregion

        #region Constructor

        public App(ISpeechSynthesizer speechSynthesizer)
        {
            InitializeComponent();

            MainPage = new AppShell();
            this.globalSettings = new GlobalSettingsViewModel(speechSynthesizer);
            InitializeAppSettings();
        }

        #endregion

        #region Private Methods 

        private void InitializeAppSettings()
        {
            //Initialize Language
            string cultureCode = this.globalSettings.Language;
            SetCulture(cultureCode);

            //Initialize AppTheme
            if (globalSettings.AppTheme == AppTheme.Light)
            {
                Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                Current.UserAppTheme = AppTheme.Dark;
            }

            //Initialize Voice VolumeTTS
            this.globalSettings.ApplySpeechToTextSettings();
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
