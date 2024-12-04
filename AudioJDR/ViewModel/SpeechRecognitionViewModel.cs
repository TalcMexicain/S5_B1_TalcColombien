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

        private ISpeechRecognition _speechRecognition;
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
        public event Action<string> NavigateToNewGame;
        public event Action<string> ContinueGame;
        public event Action ClosePopUp;

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
        public SpeechRecognitionViewModel(ISpeechRecognition speechRecognition)
        {
            _speechRecognition = speechRecognition;
            _recognizedTextAccumulator = new StringBuilder();

            // Listen to recognition events
            _speechRecognition.SpeechRecognized += OnSpeechRecognized;
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Starts speech recognition with a list of keywords for a specific context.
        /// </summary>
        public void StartRecognition(IEnumerable<string> keywords, string context)
        {
            UpdateGrammar(keywords, context);
            _speechRecognition.StartRecognition();
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
            _speechRecognition.UpdateGrammar(keywords.ToArray());
        }

        /// <summary>
        /// Unloads all active grammars.
        /// </summary>
        public void UnloadGrammars()
        {
            _speechRecognition.UnloadAllGrammars();
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
           
            if (HandleSpecificCommands(recognizedText))
            {
                return; 
            }

            HandleGeneralCommands(recognizedText);
        }
        
        private bool HandleSpecificCommands(string recognizedText)
        {
            var normalizedText = recognizedText.ToLowerInvariant();

            // Commande "nouvelle partie"
            if (normalizedText.Contains("nouvelle partie"))
            {
                var potentialTitle = ExtractTitleFromAccumulator(normalizedText, "nouvelle partie");
                if (!string.IsNullOrEmpty(potentialTitle))
                {
                    Debug.WriteLine($"Recognized title for new game: {potentialTitle}");
                    NavigateToNewGame?.Invoke(potentialTitle);
                }
                else
                {
                    Debug.WriteLine("Aucun titre valide trouvé avant 'nouvelle partie'.");
                }
                ClearAccumulator();
                return true; 
            }

            // Commande "continuer"
            if (normalizedText.Contains("continuer"))
            {
                var potentialTitle = ExtractTitleFromAccumulator(normalizedText, "continuer");
                if (!string.IsNullOrEmpty(potentialTitle))
                {
                    Debug.WriteLine($"Recognized title for continue: {potentialTitle}");
                    ContinueGame?.Invoke(potentialTitle);
                }
                else
                {
                    Debug.WriteLine("Aucun titre valide trouvé avant 'continuer'.");
                }
                ClearAccumulator();
                return true; 
            }

            return false; 
        }

        private string ExtractTitleFromAccumulator(string recognizedText, string command)
        {
            var potentialTitle = _recognizedTextAccumulator.ToString().Trim();
            potentialTitle = potentialTitle.Replace(command, "").Trim();
            return potentialTitle;
        }

        private void HandleGeneralCommands(string recognizedText)
        {
            switch (recognizedText.ToLowerInvariant())
            {
                case "valider":
                    OptionSubmitted?.Invoke();
                    ClearAccumulator();
                    TextCleared?.Invoke();
                    break;

                case "annuler":
                    ClearAccumulator();
                    TextCleared?.Invoke();
                    break;

                case "jouer":
                    NavigateToPlay?.Invoke();
                    ClearAccumulator();
                    break;

                case "repeter":
                    RepeatSpeech?.Invoke();
                    ClearAccumulator();
                    break;

                case "vers la liste des histoires":
                    NavigateNext?.Invoke();
                    ClearAccumulator();
                    break;

                case "retour":
                    NavigatePrevious?.Invoke();
                    ClearAccumulator();
                    break;

                case "ok":
                    ClosePopUp?.Invoke();
                    ClearAccumulator();
                    break;

                default:
                    
                    AddToAccumulator(recognizedText);
                    break;
            }
        }

        private void ClearAccumulator()
        {
            _recognizedTextAccumulator.Clear();
            RecognizedText = string.Empty;
        }
        
        private void AddToAccumulator(string recognizedText)
        {
            if (_recognizedTextAccumulator.Length > 0)
            {
                _recognizedTextAccumulator.Append(" ");
            }
            _recognizedTextAccumulator.Append(recognizedText);

            RecognizedText = _recognizedTextAccumulator.ToString();
            AddWordsToView?.Invoke();
        }

        #endregion
    }
}
