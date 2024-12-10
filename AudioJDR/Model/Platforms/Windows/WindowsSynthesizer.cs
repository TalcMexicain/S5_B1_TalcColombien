using Model.Resources.Localization;
using System.Speech.Synthesis;

namespace Model.Platforms.Windows
{
    /// <summary>
    /// Windows implementation of the ISpeechSynthesizer interface for text-to-speech functionality
    /// </summary>
    public class WindowsSynthesizer : ISpeechSynthesizer, IDisposable
    {
        #region Fields 

        private readonly SpeechSynthesizer _synthesizer;
        private GlobalSettings _globalSettings;

        #endregion

        #region Constructor

        public WindowsSynthesizer()
        {
            _synthesizer = new SpeechSynthesizer();
            this._globalSettings = new GlobalSettings(this);
        }

        #endregion

        #region ISpeechSynthesizer Implementation

        public async void SynthesizeTextAsync(string textToSynthesize)
        {
            _synthesizer.SpeakAsync(textToSynthesize);
        }

        public void StopSynthesisTextAsync()
        {
            if (_synthesizer.State == SynthesizerState.Speaking) 
            {
                _synthesizer.SpeakAsyncCancelAll();
            }
        }

        public void PauseCurrentSynthesis()
        {
            if (_synthesizer.State == SynthesizerState.Speaking)
            {
                _synthesizer.Pause();
            }
        }

        public void ResumePausedSynthesis()
        {
            if (_synthesizer.State == SynthesizerState.Speaking)
            {
                _synthesizer.Resume();
            }
        }

        public void SetVoiceType(string voiceName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(voiceName))
                {
                    _synthesizer.SelectVoice(voiceName);
                }

            }
            catch(Exception ex)
            {
                throw new Exception(string.Format(AppResourcesModel.TTS_SetVoiceType_NotFound, voiceName) +  ex.Message);
            }
        }

        public string GetCurrentVoiceTypeName()
        {
            return _synthesizer.Voice.Name;
        }

        public ICollection<string> GetInstalledVoices()
        {
           IReadOnlyList<InstalledVoice> voices = _synthesizer.GetInstalledVoices();

            List<string> voicesNames = new List<string>();

            foreach (InstalledVoice voice in voices)
            {
                voicesNames.Add(voice.VoiceInfo.Name);
            }

            return voicesNames;
        }

        public void SetVoiceVolume(int voiceVolume)
        {
            const int minVoiceVolume = 0;
            const int maxVoiceVolume = 100;

            if (voiceVolume < minVoiceVolume || voiceVolume > maxVoiceVolume)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceVolume),string.Format(AppResourcesModel.TTS_SetVoiceVolume_Exception,minVoiceVolume.ToString(),maxVoiceVolume.ToString()));
            }

            _synthesizer.Volume = voiceVolume;
        }

        public int GetVoiceVolume()
        {
            return _synthesizer.Volume;
        }

        public void SetVoiceRate(float voiceRate)
        {
            const int minOutputRate = -10;
            const int maxOutputRate = 10;
            const float minInputRate = 0.5f;
            const float maxInputRate = 2.0f;
            
            if (voiceRate < minInputRate || voiceRate > maxInputRate)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceRate), string.Format(AppResourcesModel.TTS_SetVoiceRate_Exception, minInputRate.ToString(), maxInputRate.ToString()));
            }

            int normalizedRate = (int)Math.Round(minOutputRate + ((voiceRate - minInputRate) / (maxInputRate - minInputRate)) * (maxOutputRate - minOutputRate));

            _synthesizer.Rate = normalizedRate;
        }

        public float GetVoiceRate()
        {
            return _synthesizer.Rate;
        }

        public void Dispose()
        {
            _synthesizer?.Dispose();
        }

        #endregion
    }
}