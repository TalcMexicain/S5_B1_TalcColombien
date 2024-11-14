#if WINDOWS
using System.Speech.Synthesis;

namespace Model
{
    /// <summary>
    /// Windows implementation of the ISpeechSynthesizer interface for text-to-speech functionality
    /// </summary>
    public class WindowsSynthesizer : ISpeechSynthesizer, IDisposable
    {

        private readonly SpeechSynthesizer _synthesizer;

        public WindowsSynthesizer()
        {
            _synthesizer = new SpeechSynthesizer();
            InitSynthesizerParameters();
        }

        private void InitSynthesizerParameters()
        {
            _synthesizer.Volume = 100;
            _synthesizer.Rate = 0;
        }

        public async void SynthesizeTextAsync(string textToSynthesize)
        {
            _synthesizer.SpeakAsync(textToSynthesize);
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

        public void SetRate(int voiceRate)
        {
            if (voiceRate < -10 || voiceRate > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceRate), "Rate parameter must be between -10 and 10");
            }

            _synthesizer.Rate = voiceRate;
        }

        public void Dispose()
        {
            _synthesizer?.Dispose();
        }
    }
}
#endif