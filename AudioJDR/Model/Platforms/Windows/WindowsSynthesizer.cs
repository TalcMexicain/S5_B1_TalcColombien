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
            if (string.IsNullOrWhiteSpace(voiceName))
            {
                throw new ArgumentException("Voice name cannot be null or whitespace",nameof(voiceName));
            }

            try
            {
                _synthesizer.SelectVoice(voiceName);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to set the voice: {voiceName}", ex);
            }
        }

        public ICollection<string> GetInstalledVoices()
        {
            var voices = _synthesizer.GetInstalledVoices();

            List<string> voicesNames = new List<string>();

            foreach (var voice in voices)
            {
                voicesNames.Add(voice.VoiceInfo.Name);
            }

            return voicesNames;
        }

        public void SetVoiceVolume(int voiceVolume)
        {
            if (voiceVolume < 0 || voiceVolume > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceVolume),"Volume must be between 0 and 100");
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
            
            if (voiceRate < 0.5f || voiceRate > 2.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceRate), "Rate parameter must be between -10 and 10");
            }

            int normalizedRate = (int)Math.Round(minOutputRate + ((voiceRate - minInputRate) / (maxInputRate - minInputRate)) * (maxOutputRate - minOutputRate));

            _synthesizer.Rate = normalizedRate;
        }

        public void Dispose()
        {
            _synthesizer?.Dispose();
        }

        #endregion
    }
}