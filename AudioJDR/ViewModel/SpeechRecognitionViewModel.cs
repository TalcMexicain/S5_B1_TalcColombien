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
        public event Action NavigateToSettings;
        public event Action NavigateNext;
        public event Action NavigatePrevious;
        public event Action RepeatSpeech;
        public event Action<string> NavigateToNewGame;
        public event Action<string> ContinueGame;
        public event Action ChangeTheme;
        public event Action ChangeLanguageEN;
        public event Action ChangeLanguageFR;
        public event Action TestVoice;
        public event Action IncreaseVolume;
        public event Action DecreaseVolume;
        public event Action IncreaseSpeed;
        public event Action DecreaseSpeed;

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
        /// Stops speech recognition.
        /// </summary>
        public void StopRecognition()
        {
            _speechRecognition.StopRecognition();
        }

        /// <summary>
        /// Updates the grammar for the recognizer. Clears previous grammars if the context changes.
        /// </summary>
        public void UpdateGrammar(IEnumerable<string> keywords, string context)
        {
            if (_currentContext != context)
            {
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
           
            if (HandleSpecificCommands(recognizedText))
            {
                return; 
            }

            HandleGeneralCommands(recognizedText);
        }
        
        private bool HandleSpecificCommands(string recognizedText)
        {
            string normalizedText = recognizedText.ToLowerInvariant();
            if (normalizedText.Contains("nouvelle partie")||normalizedText.Contains("new game"))
            {
                var potentialTitle = ExtractTitleFromAccumulator(normalizedText, "nouvelle partie");
                if (!string.IsNullOrEmpty(potentialTitle))
                {
                    NavigateToNewGame?.Invoke(potentialTitle);
                }
                ClearAccumulator();
                return true; 
            }

            if (normalizedText.Contains("continuer") || normalizedText.Contains("continue"))
            {
                var potentialTitle = ExtractTitleFromAccumulator(normalizedText, "continuer");
                if (!string.IsNullOrEmpty(potentialTitle))
                {
                    ContinueGame?.Invoke(potentialTitle);
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
            string[] validateCommands = new[] { "validate", "valider" };
            string[] cancelCommands = new[] { "cancel", "annuler" };
            string[] playCommands = new[] { "jouer", "play" };
            string[] settingsCommands = new[] { "settings", "paramètres" };
            string[] repeatCommands = new[] { "repeter", "repeat" };
            string[] listStoryCommands = new[] { "liste d'histoire", "list of story" };
            string[] backCommands = new[] { "retour", "back" };
            string[] themeCommands = new[] { "change theme", "changer le theme"};
            string[] languageCommandsEN = new[] { "english", "anglais" };
            string[] languageCommandsFR = new[] { "french", "français" };
            string[] testCommands = new[] { "test voice", "tester la voix" };
            string[] increaseVolumeCommands = new[] { "increase volume", "monter le volume" };
            string[] decreaseVolumeCommands = new[] { "decrease volume", "baisser le volume" };
            string[] increaseSpeedCommands = new[] { "increase speed", "augmenter la vitesse" };
            string[] decreaseSpeedCommands = new[] { "decrease speed", "baisser la vitesse" };
            

            switch (recognizedText.ToLowerInvariant())
            {
                case string cmd when validateCommands.Contains(cmd):
                    OptionSubmitted?.Invoke();
                    ClearAccumulator();
                    TextCleared?.Invoke();
                    break;

                case string cmd when cancelCommands.Contains(cmd):
                    ClearAccumulator();
                    TextCleared?.Invoke();
                    break;

                case string cmd when playCommands.Contains(cmd):
                    NavigateToPlay?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when settingsCommands.Contains(cmd):
                    NavigateToSettings?.Invoke();
                    TextCleared?.Invoke();
                    break;

                case string cmd when repeatCommands.Contains(cmd):
                    RepeatSpeech?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when listStoryCommands.Contains(cmd):
                    NavigateNext?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when backCommands.Contains(cmd):
                    NavigatePrevious?.Invoke();
                    ClearAccumulator();
                    break;
                case string cmd when themeCommands.Contains(cmd):
                    ChangeTheme?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when languageCommandsEN.Contains(cmd):
                    ChangeLanguageEN?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when languageCommandsFR.Contains(cmd):
                    ChangeLanguageFR?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when testCommands.Contains(cmd):
                    TestVoice?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when increaseVolumeCommands.Contains(cmd):
                    IncreaseVolume?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when decreaseVolumeCommands.Contains(cmd):
                    DecreaseVolume?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when increaseSpeedCommands.Contains(cmd):
                    IncreaseSpeed?.Invoke();
                    ClearAccumulator();
                    break;

                case string cmd when decreaseSpeedCommands.Contains(cmd):
                    DecreaseSpeed?.Invoke();
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
