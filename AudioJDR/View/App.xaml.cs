using System.Globalization;

namespace View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //SetCulture("en"); // To test in other languages
            MainPage = new AppShell();
        }

        /// <summary>
        /// Changes the language of the app
        /// </summary>
        /// <param name="cultureCode">Country code of wanted language (e.g. 'fr')</param>
        public void SetCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
