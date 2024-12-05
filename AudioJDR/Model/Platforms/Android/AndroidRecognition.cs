using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Platforms.Android
{
    public class AndroidRecognition : ISpeechRecognition
    {
        #region Fields

        private readonly Context _context;
        private SpeechRecognizer _speechRecognizer;
        private Intent _recognizerIntent;
        private HashSet<string> _keywords;

        #endregion

        #region Event

        public event Action<string> SpeechRecognized;

        #endregion

        #region Constructor

        public AndroidRecognition(Context context)
        {
            _context = context;

            _recognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            _recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            _recognizerIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);

            _speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(_context);
            _speechRecognizer.SetRecognitionListener(new AndroidRecognitionListener(OnSpeechRecognized, _speechRecognizer, _recognizerIntent));

            _keywords = new HashSet<string>();
        }

        #endregion

        #region Public Methods

        public async void StartRecognition()
        {
            if (await PermissionsHelper.CheckAndRequestMicrophonePermissionAsync())
            {
                _speechRecognizer.StartListening(_recognizerIntent);
            }
            else
            {
                throw new PermissionException("Microphone permission denied. Speech recognition cannot start.");
            }
        }

        public void UpdateGrammar(string[] keywords)
        {
            _keywords = new HashSet<string>(keywords.Select(k => k.ToLowerInvariant()));
        }

        public void UnloadAllGrammars()
        {
            _keywords.Clear();
        }

        #endregion

        #region Private Methods 

        private void OnSpeechRecognized(string recognizedText)
        {
            if (_keywords.Count > 0)
            {
                var matchedKeyword = _keywords.FirstOrDefault(k => recognizedText.ToLowerInvariant().Contains(k));
                if (!string.IsNullOrEmpty(matchedKeyword))
                {
                    SpeechRecognized?.Invoke(matchedKeyword);
                }
            }
            else
            {
                SpeechRecognized?.Invoke(recognizedText);
            }
        }

        #endregion
    }
}
