using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible of managing ISpeechSynthesizer and their operations
    /// Handles voice synthesize, TTS Settings, etc
    /// </summary>
    public class SpeechSynthesizerViewModel : BaseViewModel
    {
        #region Fields 

        private readonly ISpeechSynthesizer _speechSynthesizer;
        private ObservableCollection<string> _availableVoicesTypeTTS;
        private string _textToSynthesize;

        #endregion

        #region Properties 

        /// <summary>
        /// The text to be spoken by the synthesizer
        /// </summary>
        public string TextToSynthesize
        {
            get => _textToSynthesize;
            set => SetProperty(ref _textToSynthesize, value);
        }

        /// <summary>
        /// Gets or sets the volume of the text-to-speech 
        /// </summary>
        public int VoiceVolumeTTS
        {
            get => _speechSynthesizer.GetVoiceVolume();
            set => _speechSynthesizer.SetVoiceVolume(value);
        }

        /// <summary>
        /// Gets or sets the speaking rate of the text-to-speech engine
        /// </summary>
        public float VoiceRateTTS
        {
            get => _speechSynthesizer.GetVoiceRate();
            set => _speechSynthesizer.SetVoiceRate(value);
        }

        /// <summary>
        /// Gets or sets the type of voice used  by the TTS engine
        /// </summary>
        public string VoiceTypeTTS
        {
            get => _speechSynthesizer.GetCurrentVoiceTypeName();
            set => _speechSynthesizer.SetVoiceType(value);
        }

        /// <summary>
        /// Gets the available voice types installed on the system
        /// </summary>
        public ICollection<string> AvailableVoicesTypeTTS
        {
            get => AvailableVoicesTypeTTS;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TTS ViewModel
        /// </summary>
        /// <param name="speechSynthesizer">The TTS object to be managed by this VM</param>
        public SpeechSynthesizerViewModel(ISpeechSynthesizer speechSynthesizer)
        {
            _speechSynthesizer = speechSynthesizer;
            this._availableVoicesTypeTTS = (ObservableCollection<string>)_speechSynthesizer.GetInstalledVoices();
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Synthesis and speaks the provided text
        /// </summary>
        public void SynthesizeText()
        {
            if (!string.IsNullOrEmpty(TextToSynthesize))
            {
                _speechSynthesizer.SynthesizeTextAsync(TextToSynthesize);
            }
        }

        /// <summary>
        /// Stops the currently on going speech synthesis 
        /// </summary>
        public void StopSynthesis()
        {
            _speechSynthesizer.StopSynthesisTextAsync();
        }

        /// <summary>
        /// Pauses the currently running speech synthesis
        /// </summary>
        public void PauseCurrentSynthesis()
        {
            _speechSynthesizer.PauseCurrentSynthesis();
        }

        /// <summary>
        /// Resumes a speech synthesis that was previously paused
        /// </summary>
        public void ResumePausedSynthesis()
        {
            _speechSynthesizer.ResumePausedSynthesis();
        }

        #endregion
    }
}
