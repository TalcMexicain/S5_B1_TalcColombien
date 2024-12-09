using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Platforms.Android
{
    public class AndroidRecognitionListener : Java.Lang.Object, IRecognitionListener
    {
        private Action<string> _onSpeechRecognized;
        private SpeechRecognizer _speechRecognizer;
        private Intent _recognitionIntent;

        public AndroidRecognitionListener(Action<string> onSpeechRecognized, SpeechRecognizer speechRecognizer, Intent recognitionIntent)
        {
            _onSpeechRecognized = onSpeechRecognized;
            _speechRecognizer = speechRecognizer;
            _recognitionIntent = recognitionIntent;
        }

        public void OnResults(Bundle? results)
        {
            IList<string> matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            if (matches != null && matches.Count > 0)
            {
                _onSpeechRecognized?.Invoke(matches[0]);
            }

            RestartSpeechRecognition();
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {

        }

        public void OnBeginningOfSpeech()
        {

        }

        public void OnBufferReceived(byte[]? buffer)
        {
            
        }

        public void OnEndOfSpeech()
        {
            
        }

        public void OnEvent(int eventType, Bundle? @params)
        {
            
        }

        public void OnPartialResults(Bundle? partialResults)
        {
            
        }

        public void OnReadyForSpeech(Bundle? @params)
        {
            Console.WriteLine("Speech Recognition is Ready !");
        }

        public void OnRmsChanged(float rmsdB)
        {
            
        }


        private void RestartSpeechRecognition()
        {
            _speechRecognizer.StopListening();
            _speechRecognizer.StartListening(_recognitionIntent);
        }
    }
}
