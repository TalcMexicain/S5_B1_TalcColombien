using Android.Runtime;
using Android.Speech.Tts;
using Android.Content;
using TextToSpeech = Android.Speech.Tts.TextToSpeech;
using Android.OS;

namespace Model.Platforms.Android
{
    public class AndroidSynthesizer : Java.Lang.Object, ISpeechSynthesizer, TextToSpeech.IOnInitListener, IDisposable
    {
        private TextToSpeech _textToSpeech;
        private bool _isInitialized;
        private Context _context;

        private int _volume;

        public AndroidSynthesizer(Context context)
        {
            this._context = context;
            this._isInitialized = false;
            this._textToSpeech = new TextToSpeech(this._context,this);

            this._volume = 50;
        }

        public void OnInit(OperationResult status)
        {
            _isInitialized = status == OperationResult.Success;
            if (_isInitialized)
            {
                _textToSpeech.SetLanguage(Java.Util.Locale.Default);
            }
        }

        public void SynthesizeTextAsync(string textToSynthesize)
        {
            if(_isInitialized && !string.IsNullOrEmpty(textToSynthesize))
            {
                Bundle bundle = new Bundle();
                bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, _volume);
                _textToSpeech.Speak(textToSynthesize, QueueMode.Flush, bundle,null);
            }
        }

        public void StopSynthesisTextAsync()
        {
            if (_isInitialized)
            {
                _textToSpeech.Stop();
            }
        }

        public void PauseCurrentSynthesis()
        {
            //TODO 
        }

        public void ResumePausedSynthesis()
        {
            //TODO
        }

        public ICollection<string> GetInstalledVoices()
        {
            ICollection<string> voicesName = new List<string>();

            if (_isInitialized && _textToSpeech != null)
            {
                ICollection<Voice>? voices = _textToSpeech.Voices;

                if (voices != null && voices.Count > 0)
                {
                    foreach (Voice voice in voices)
                    {
                        if (!string.IsNullOrEmpty(voice.Name))
                        {
                            voicesName.Add(voice.Name);
                        }
                    }
                }
            }

            return voicesName;
        }

        public int GetVoiceVolume()
        {
            return this._volume;
        }

        public void SetRate(float voiceRate)
        {
            if (voiceRate < 0.5f || voiceRate > 2.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceRate), "voiceRate parameter must be between 0.5f and 2.0f");   
            }

            _textToSpeech.SetSpeechRate(voiceRate);
        }

        public void SetVoiceType(string voiceName)
        {
            if (_isInitialized && !string.IsNullOrEmpty(voiceName))
            {
                Voice? voice = _textToSpeech.Voices.FirstOrDefault(x => x.Name == voiceName);  

                if (voice != null)
                {
                    _textToSpeech.SetVoice(voice);
                }
                else
                {
                    throw new ArgumentException($"Voice {voiceName} not found.", nameof(voiceName));
                }
            }
        }

        public void SetVoiceVolume(int voiceVolume)
        {
            if (voiceVolume < 0 || voiceVolume > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceVolume), "voiceVolume parameter must be between 0 and 100.");
            }

            this._volume = voiceVolume;
        }

        public void Dispose()
        {
            if (_textToSpeech != null)
            {
                _textToSpeech.Dispose();
                _textToSpeech.Shutdown();
            }
        }
    }
}
