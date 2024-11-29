using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.Maui.ApplicationModel;

namespace Model
{
    /// <summary>
    /// Class that manages and stores global the application's global parameters
    /// All application settings are stored in local preferences that are not platform-dependent thanks to MAUI.
    /// </summary>
    public class GlobalSettings
    {
        #region Const

        private const string themeKey = "App_Theme";
        private const string languageKey = "App_Language";
        private const string volumeKey = "Volume_TTS";
        private const string rateKey = "Rate_TTS";
        private const string voiceTypeKey = "VoiceType_TTS";

        private const AppTheme defaultTheme = AppTheme.Light;
        private const string defaultLanguage = "fr-FR";
        private const int defaultVolumeTTS = 50;
        private const float defaultRateTTS = 1.0f;
        private const string defaultVoiceTypeTTS = "";

        #endregion

        #region Fields

        private ICollection<string> _availableVoicesTypeTTS;
        private ISpeechSynthesizer _speechSynthesizer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or stores the current application theme (White or Dark Theme)
        /// The default value is the constant "defaultTheme" (<see cref="defaultTheme"/>)
        /// </summary>
        public AppTheme AppTheme
        { 
            get
            {
                return (AppTheme)Preferences.Get(themeKey, (int)defaultTheme);
            }
            set
            {
                Preferences.Set(themeKey, (int)value);
            }
        }

        /// <summary>
        /// Gets or stores the current languages
        /// If the device language is supported, it uses that | otherwise, it defaults to the value of the constant "defaultLanguage" (<see cref="defaultLanguage"/>)
        /// </summary>
        public string Language
        {
            get
            {
                return Preferences.Get(languageKey, GetDefautlLanguage());
            }
            set
            { 
                Preferences.Set(languageKey, value);
            }
        }

        /// <summary>
        /// Gets or stores the volume for text-to-speech (TTS)
        /// The defautl value is set by the cosntant "defautlVolumeTTS" (<see cref="defaultVolumeTTS"/>)
        /// </summary>
        public int VolumeTTS
        {
            get
            {
                return Preferences.Get(volumeKey, defaultVolumeTTS);
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "TextToSpeech volume setting value must be between 0 and 100.");
                }

                Preferences.Set(volumeKey, value);
            }
        }

        /// <summary>
        /// Gets or stores the rate (speed) for (TTS)
        /// The default value is set by the constant "defaultRateTTS" (<see cref="defaultRateTTS"/>)
        /// </summary>
        public float RateTTS
        {
            get
            {
                return Preferences.Get(rateKey, defaultRateTTS);
            }
            set
            {
                if (value < 0.5f || value > 2.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "TextToSpeech rate setting value must a float between 0.5 and 2.0.");
                }

                Preferences.Set(rateKey, value);
            }
        }

        /// <summary>
        /// Gets or stores voice type for text-to-speech (TTS)
        /// The default value is set by the cosntant "defaultVoiceTypeTTS" (<see cref="defaultVoiceTypeTTS"/>)
        /// </summary>
        public string VoiceTypeTTS
        {
            get
            {
                return Preferences.Get(voiceTypeKey, defaultVoiceTypeTTS);
            }
            set
            {
                Preferences.Set(voiceTypeKey, value);
            }
        }

        /// <summary>
        /// Gets the collection of available voice types for text-to-speech (TTS)
        /// </summary>
        public ICollection<string> AvailableVoicesTypeTTS
        {
            get
            {
                return this._availableVoicesTypeTTS;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of the class
        /// </summary>
        /// <param name="speechsSynthesizer">An instence of a speech synthesizer</param>
        public GlobalSettings(ISpeechSynthesizer speechsSynthesizer)
        {
            this._availableVoicesTypeTTS = new List<string>();
            this._speechSynthesizer = speechsSynthesizer;
            LoadAllVoicesTypeTTS();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies voice volume stored to the current speechSynthesizer
        /// </summary>
        public void ApplyTTSVoiceVolume()
        {
            this._speechSynthesizer.SetVoiceVolume(VolumeTTS);
        }

        /// <summary>
        /// Applies voice rate stored to the current speechSynthesizer
        /// </summary>
        public void ApplyTTSVoiceRate() 
        {
            this._speechSynthesizer.SetVoiceRate(RateTTS);
        }

        /// <summary>
        /// Applies voice type stored to the current speechSynthesizer
        /// </summary>
        public void ApplyTTSVoiceType()
        {
            if (VoiceTypeTTS != "")
            {
                this._speechSynthesizer.SetVoiceType(VoiceTypeTTS);
            }
        }
        #endregion

        #region Private Methods

        private void LoadAllVoicesTypeTTS()
        {
            this._availableVoicesTypeTTS = this._speechSynthesizer.GetInstalledVoices();
        }

        private string GetDefautlLanguage()
        {
            string currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (string.IsNullOrEmpty(currentLanguage) || !IsSupportedLanguage(currentLanguage)) 
            {
                currentLanguage = defaultLanguage;
            }

            return currentLanguage;
        }

        private bool IsSupportedLanguage(string languageToVerify)
        {
            List<string> supportedLanguages = new List<string> { "fr" , "en" };

            return supportedLanguages.Contains(languageToVerify);
        }

        #endregion
    }
}
