using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SpeechRecognitionModel
    {
        #region Fields 

        private SpeechRecognitionEngine _recognizer;
        private Action _onOptionSubmittedCallback;

        #endregion

        #region Events 

        /// <summary>
        /// Event triggered when a text is recognized.
        /// </summary>
        public event Action<string> SpeechRecognized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechRecognitionModel"/> class.
        /// Sets up the speech recognition engine with French culture settings.
        /// </summary>
        public SpeechRecognitionModel()
        {
            try
            {
                // Initialize the speech recognition engine
                _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("fr-FR"));

                // Configure the audio input
                _recognizer.SetInputToDefaultAudioDevice();

                // Subscribe to the "speech recognized" event
                _recognizer.SpeechRecognized += (s, e) =>
                {
                    SpeechRecognized?.Invoke(e.Result.Text); // Notify recognized text
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing speech recognition: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods 

        /// <summary>
        /// Starts the speech recognition process.
        /// </summary>
        public void StartRecognition()
        {
            if (_recognizer != null && _recognizer.AudioState == AudioState.Stopped)
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        /// <summary>
        /// Updates the grammar of the speech recognition engine with the provided keywords.
        /// </summary>
        /// <param name="keywords">An array of keywords to include in the grammar.</param>
        public void UpdateGrammar(string[] keywords)
        {
            _recognizer.UnloadAllGrammars();

            // Update the grammar with the provided keywords
            var choices = new Choices(keywords);
            var grammarBuild = new GrammarBuilder(choices)
            {
                Culture = new System.Globalization.CultureInfo("fr-FR")
            };
            var grammar = new Grammar(grammarBuild);

            _recognizer.LoadGrammar(grammar);
        }

        public void UnloadAllGrammars()
        {
            _recognizer?.UnloadAllGrammars();
        }

        #endregion
    }
}
