using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace Model
{
    public class SpeechRecognitionModel
    {
        private SpeechRecognitionEngine _recognizer;
        private Action _onOptionSubmittedCallback;

        // Événement déclenché lorsqu'un texte est reconnu
        public event Action<string> SpeechRecognized;

        public SpeechRecognitionModel()
        {
            try
            {
                // Initialiser le moteur de reconnaissance vocale
                _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("fr-FR"));

                // Configurer l'entrée audio
                _recognizer.SetInputToDefaultAudioDevice();

                // Abonnement à l'événement "texte reconnu"
                _recognizer.SpeechRecognized += (s, e) =>
                {
                    SpeechRecognized?.Invoke(e.Result.Text); // Notifie le texte reconnu
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'initialisation de la reconnaissance vocale : {ex.Message}");
            }
        }

        // Méthode pour démarrer la reconnaissance vocale
        public void StartRecognition()
        {
            if (_recognizer != null && _recognizer.AudioState == AudioState.Stopped)
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        public void UpdateGrammar(string[] keywords)
        {
           
            // Mettre à jour la grammaire avec les mots-clés fournis
            var choices = new Choices(keywords);
            var grammar = new Grammar(new GrammarBuilder(choices));
            _recognizer.LoadGrammar(grammar);
        }
    }
}
