using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Speech.Recognition;
using Model;

namespace ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing SpeechRecognition and its operations.
    /// Handles voice recognition with contextual grammar.
    /// </summary>
    public class SpeechRecognitionViewModel : BaseViewModel
    {
        #region Fields 

        private SpeechRecognitionModel _speechRecognitionModel;
        private StringBuilder _recognizedTextAccumulator;
        private string _recognizedText;
        private string _currentContext; // Tracks the active context


        #endregion

        #region Events 

        public event Action OptionSubmitted; // When "validate" is recognized
        public event Action TextCleared; // When "cancel" is recognized
        public event Action AddWordsToView;
        public event Action NavigateToPlay;
        public event Action NavigateNext;
        public event Action NavigatePrevious;
        public event Action RepeatSpeech;

        #endregion

        #region Properties 

        /// <summary>
        /// Gets the text recognized by the speech engine.
        /// This property is automatically updated and notifies the view when changed.
        /// </summary>
        public string RecognizedText
        {
            get => _recognizedText;
            private set => SetProperty(ref _recognizedText, value);
        }

        #endregion

        #region Constructor 

        /// <summary>
        /// Initializes a new instance of the SpeechRecognitionViewModel class.
        /// Sets up the speech recognition model and subscribes to its events.
        /// </summary>
        public SpeechRecognitionViewModel()
        {
            _speechRecognitionModel = new SpeechRecognitionModel();
            _recognizedTextAccumulator = new StringBuilder();

            // Listen to recognition events
            _speechRecognitionModel.SpeechRecognized += OnSpeechRecognized;
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Starts speech recognition with a list of keywords for a specific context.
        /// </summary>
        public void StartRecognition(IEnumerable<string> keywords, string context)
        {
            UpdateGrammar(keywords, context);
            _speechRecognitionModel.StartRecognition();
        }

        /// <summary>
        /// Updates the grammar for the recognizer. Clears previous grammars if the context changes.
        /// </summary>
        public void UpdateGrammar(IEnumerable<string> keywords, string context)
        {
            if (_currentContext != context)
            {
                Debug.WriteLine($"Context has changed. Unloading previous grammars.");
                UnloadGrammars(); // Unload existing grammars if the context changes
                _currentContext = context;
            }
            Debug.WriteLine($"Updating grammar with new keywords: {string.Join(", ", keywords)}");
            _speechRecognitionModel.UpdateGrammar(keywords.ToArray());
        }

        /// <summary>
        /// Unloads all active grammars.
        /// </summary>
        public void UnloadGrammars()
        {
            _speechRecognitionModel.UnloadAllGrammars();
            _currentContext = null; // Reset the context
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the recognized text from the engine.
        /// </summary>
        private void OnSpeechRecognized(string recognizedText)
        {
            Debug.WriteLine($"Recognized: {recognizedText}");
            switch (recognizedText.ToLowerInvariant())
            {
                case "valider":
                    OptionSubmitted?.Invoke();
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    TextCleared?.Invoke();
                    break;

                case "annuler":
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    TextCleared?.Invoke();
                    break;

                case "jouer":
                    NavigateToPlay?.Invoke();
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    break;

                case "repeter":
                    RepeatSpeech?.Invoke();
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    break;

                case "continuer":
                    NavigateNext?.Invoke();
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    break;

                case "retour":
                    NavigatePrevious?.Invoke();
                    _recognizedTextAccumulator.Clear();
                    RecognizedText = string.Empty;
                    break;

                default:
                    if (_recognizedTextAccumulator.Length > 0)
                    {
                        _recognizedTextAccumulator.Append(" ");
                    }
                    _recognizedTextAccumulator.Append(recognizedText);

                    RecognizedText = _recognizedTextAccumulator.ToString();
                    AddWordsToView?.Invoke();
                    break;
            }
        }

        #endregion
    }
}
