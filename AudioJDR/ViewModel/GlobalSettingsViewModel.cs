using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible of managing GlobalSettings and their operations
    /// Handles settings such as application theme, language, and text-to-speech settings
    /// </summary>
    public class GlobalSettingsViewModel : BaseViewModel
    {
        #region Fields

        private GlobalSettings _modelSettings;
        private ObservableCollection<string> _availableVoicesTypeTTS;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of available voices for text-to-speech
        /// </summary>
        public ObservableCollection<string> AvailableVoicesTypeTTS
        {
            get => _availableVoicesTypeTTS;
            private set => SetProperty(ref _availableVoicesTypeTTS, value);
        }

        /// <summary>
        /// Gets or sets the application theme
        /// </summary>
        public AppTheme AppTheme
        {
            get => _modelSettings.AppTheme;
            set
            {
                _modelSettings.AppTheme = value;
                OnPropertyChanged(nameof(AppTheme));
            }
        }

        /// <summary>
        /// Gets or sets the application language
        /// </summary>
        public string Language
        {
            get => _modelSettings.Language;
            set
            {
                _modelSettings.Language = value;
                OnPropertyChanged(nameof(Language));
            }
        }

        /// <summary>
        /// Gets or sets the volume level for TTS
        /// </summary>
        public int VolumeTTS
        {
            get => _modelSettings.VolumeTTS;
            set
            { 
                _modelSettings.VolumeTTS = value;
                OnPropertyChanged(nameof(VolumeTTS));
            }
        }

        /// <summary>
        /// Gets or sets the rate (speed) of speech for TTS
        /// </summary>
        public float RateTTS
        {
            get => _modelSettings.RateTTS;
            set
            {
                _modelSettings.RateTTS = value;
                OnPropertyChanged(nameof(RateTTS));
            }
        }

        /// <summary>
        /// Gets or sets the voice type for TTS
        /// </summary>
        public string VoiceTypeTTS
        {
            get => _modelSettings.VoiceTypeTTS;
            set
            {
                _modelSettings.VoiceTypeTTS = value;
                OnPropertyChanged(nameof(VoiceTypeTTS));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of GlobalSettingsViewModel class
        /// </summary>
        /// <param name="speechSynthesizer"></param>
        public GlobalSettingsViewModel(ISpeechSynthesizer speechSynthesizer) 
        {
            this._modelSettings = new GlobalSettings(speechSynthesizer);
            this._availableVoicesTypeTTS = new ObservableCollection<string>(this._modelSettings.AvailableVoicesTypeTTS);

            PropertyChanged += GlobalSettings_PropertyChanged;

            InitializationViewModel();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the voice volume TTS settings to the TTS instance
        /// </summary>
        public void ApplyTTSVoiceVolume()
        {
            this._modelSettings.ApplyTTSVoiceVolume();
        }

        /// <summary>
        /// Applies the voice rate (speed) TTS settings to the TTS instance
        /// </summary>
        public void ApplyTTSVoiceRate()
        {
            this._modelSettings.ApplyTTSVoiceRate();
        }

        /// <summary>
        /// Applies the voice type TTS settings to the TTS instance
        /// </summary>
        public void ApplyTTSVoiceType()
        {
            this._modelSettings.ApplyTTSVoiceType();
        }

        #endregion

        #region Private Methods 

        private void InitializationViewModel()
        {
            AppTheme = _modelSettings.AppTheme;
            Language = _modelSettings.Language;
            VolumeTTS = _modelSettings.VolumeTTS;
            RateTTS = _modelSettings.RateTTS;
            VoiceTypeTTS = _modelSettings.VoiceTypeTTS;
        }

        private void GlobalSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(VolumeTTS):
                        ApplyTTSVoiceVolume();
                        break;
                    case nameof(RateTTS):
                        ApplyTTSVoiceRate();
                        break;
                    case nameof(VoiceTypeTTS):
                    ApplyTTSVoiceType();
                    break;
                }
            }
        }

        #endregion
    }
}
