using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Model;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible of managing SpeechRecognition and their operations
    /// Handles voice recognition
    /// </summary>
    public class SpeechRecognitionViewModel : BaseViewModel
    {
        #region Fields 

        private SpeechRecognitionModel _speechRecognitionModel;
        private StringBuilder _recognizedTextAccumulator;
        private string _recognizedText;

        #endregion

        #region Events 

        public event Action OptionSubmitted; // When "validate" is recognized
        public event Action TextCleared; // When "cancel" is recognized
        public event Action AddWordsToView;

        #endregion

        #region Properties 

        /// <summary>
        /// Gets the text recognized by the speech engine
        /// This property is automatically updated and notifies the view when changed
        /// </summary>
        public string RecognizedText
        {
            get => _recognizedText;
            private set => SetProperty(ref _recognizedText, value);
        }

        #endregion

        #region Constructor 

        /// <summary>
        /// Initializes a new instance of the SpeechRecognition class
        /// Sets up the speech recognition model and subscribies to its events
        /// </summary>
        public SpeechRecognitionViewModel()
        {
            // Instantiate the speech recognition model
            _speechRecognitionModel = new SpeechRecognitionModel();
            _recognizedTextAccumulator = new StringBuilder();

            // Listen to recognition events
            _speechRecognitionModel.SpeechRecognized += OnSpeechRecognized;
        }

        #endregion

        #region Publics Methods 

        /// <summary>
        /// Starts speech recognition with a list of keywords.
        /// </summary>
        public void StartRecognition(IEnumerable<string> keywords)
        {
            _speechRecognitionModel.UpdateGrammar(keywords.ToArray());
            _speechRecognitionModel.StartRecognition();
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}
