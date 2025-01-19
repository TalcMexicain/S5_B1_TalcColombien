using Android.Runtime;
using Android.Speech.Tts;
using Android.Content;
using TextToSpeech = Android.Speech.Tts.TextToSpeech;
using Android.OS;
using Model.Resources.Localization;

namespace Model.Platforms.Android
{
    /// <summary>
    /// Android implementation of the ISpeechSynthesizer interface for text-to-speech functionality
    /// </summary>
    public class AndroidSynthesizer : Java.Lang.Object, ISpeechSynthesizer, TextToSpeech.IOnInitListener, IDisposable
    {
        #region Fields

        private TextToSpeech _textToSpeech;
        private bool _isInitialized;
        private Context _context;

        private int _volume;
        private float _voiceRate;
        private string _currentVoiceName;

        #endregion

        #region Constructor

        public AndroidSynthesizer(Context context)
        {
            this._context = context;
            this._isInitialized = false;
            this._textToSpeech = new TextToSpeech(this._context,this);

            this._volume = 50;
            this._voiceRate = 1.0f;
            this._currentVoiceName = string.Empty;
        }

        #endregion

        #region TTS.IOnInitListener Implementation

        public void OnInit(OperationResult status)
        {
            _isInitialized = status == OperationResult.Success;
            if (_isInitialized)
            {
                _textToSpeech.SetLanguage(Java.Util.Locale.Default);
            }
        }

        #endregion

        #region ISpeechSynthesizer Implementation

        public void SynthesizeTextAsync(string textToSynthesize)
        {
            if(_isInitialized && !string.IsNullOrEmpty(textToSynthesize))
            {
                Bundle bundle = new Bundle();
                bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, _volume / 100f);
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
            throw new NotSupportedException("Android TTS does not support paused synthesis.");
        }

        public void ResumePausedSynthesis()
        {
            throw new NotSupportedException("Android TTS does not support resuming paused synthesis.");
        }

        public void SetVoiceVolume(int voiceVolume)
        {
            const int minVoiceVolume = 0;
            const int maxVoiceVolume = 100;

            if (voiceVolume < minVoiceVolume || voiceVolume > maxVoiceVolume)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceVolume), string.Format(AppResourcesModel.TTS_SetVoiceVolume_Exception, minVoiceVolume.ToString(), maxVoiceVolume.ToString()));
            }

            this._volume = voiceVolume;
        }

        public int GetVoiceVolume()
        {
            return this._volume;
        }

        public void SetVoiceRate(float voiceRate)
        {
            const float minVoiceRate = 0.5f;
            const float maxVoiceRate = 2.0f;

            if (voiceRate < minVoiceRate || voiceRate > maxVoiceRate)
            {
                throw new ArgumentOutOfRangeException(nameof(voiceRate), string.Format(AppResourcesModel.TTS_SetVoiceRate_Exception, minVoiceRate.ToString(), maxVoiceRate.ToString()));   
            }
            _voiceRate = voiceRate;

            if (_isInitialized)
            {
                _textToSpeech.SetSpeechRate(voiceRate);
            }
        }

        public float GetVoiceRate()
        {
            return _voiceRate;
        }

        public void SetVoiceType(string voiceName)
        {
            if (_isInitialized && !string.IsNullOrEmpty(voiceName))
            {
                Voice? voice = _textToSpeech.Voices.FirstOrDefault(x => x.Name == voiceName);  

                if (voice != null)
                {
                    _textToSpeech.SetVoice(voice);
                    _currentVoiceName = voiceName;
                }
                else
                {
                    throw new ArgumentException(string.Format(AppResourcesModel.TTS_SetVoiceType_Exception, nameof(voiceName)));
                }
            }
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

        public string GetCurrentVoiceTypeName()
        {
            return _currentVoiceName;
        }

        public void Dispose()
        {
            if (_textToSpeech != null)
            {
                _textToSpeech.Dispose();
                _textToSpeech.Shutdown();
                _textToSpeech = null;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
