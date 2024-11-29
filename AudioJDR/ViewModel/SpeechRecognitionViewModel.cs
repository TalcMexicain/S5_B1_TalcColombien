using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Model;

namespace ViewModel
{
    public class SpeechRecognitionViewModel : BaseViewModel
    {
        private readonly SpeechRecognitionModel _speechRecognitionModel;
        private StringBuilder _recognizedTextAccumulator = new StringBuilder();

        // Events to signal actions to the view
        public event Action OptionSubmitted; // When "validate" is recognized
        public event Action TextCleared; // When "cancel" is recognized
        public event Action AddWordsToView;

        // Text accumulated by speech recognition
        private string _recognizedText;
        public string RecognizedText
        {
            get => _recognizedText;
            private set => SetProperty(ref _recognizedText, value); // Automatically updates the view
        }

        public SpeechRecognitionViewModel()
        {
            // Instantiate the speech recognition model
            _speechRecognitionModel = new SpeechRecognitionModel();

            // Listen to recognition events
            _speechRecognitionModel.SpeechRecognized += OnSpeechRecognized;
        }

        /// <summary>
        /// Starts speech recognition with a list of keywords.
        /// </summary>
        public void StartRecognition(IEnumerable<string> keywords)
        {
            _speechRecognitionModel.UpdateGrammar(keywords.ToArray());
            _speechRecognitionModel.StartRecognition();
        }

        /// <summary>
        /// Handles the recognized text from the engine.
        /// </summary>
        private void OnSpeechRecognized(string recognizedText)
        {
            // "validate": triggers the action but displays nothing in the input field
            if (recognizedText.Equals("valider", StringComparison.OrdinalIgnoreCase))
            {
                // Triggers the "OptionSubmitted" event
                OptionSubmitted?.Invoke();
                _recognizedTextAccumulator.Clear();
                RecognizedText = string.Empty;
                TextCleared?.Invoke();
                Debug.WriteLine("test1");
            }
            // "cancel": resets the accumulated text and clears the input field
            else if (recognizedText.Equals("annuler", StringComparison.OrdinalIgnoreCase))
            {
                _recognizedTextAccumulator.Clear();
                RecognizedText = string.Empty;  // Clears the property bound to the input field
                TextCleared?.Invoke(); // Signals the view to clear the text field
            }
            else
            {
                // Add recognized text to the accumulated text
                if (_recognizedTextAccumulator.Length > 0)
                {
                    _recognizedTextAccumulator.Append(" ");
                }
                _recognizedTextAccumulator.Append(recognizedText);

                // Updates RecognizedText which is bound to the input field
                RecognizedText = _recognizedTextAccumulator.ToString();
                AddWordsToView?.Invoke();
            }
        }
    }
}
