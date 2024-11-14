using System.Diagnostics;
using System.Globalization;

namespace Model
{
    public class StoryManagementSystem
    {
        private ISpeechSynthesizer speechSynthesizer;
        private ISpeechRecognizer speechRecognizer;

        private CancellationTokenSource tokenSource;
        private string recognitionText;

        public string RecognitionText 
        { 
            get => recognitionText; 
            set => recognitionText = value; 
        }

        public StoryManagementSystem() 
        {
#if WINDOWS
            speechSynthesizer = new WindowsSynthesizer();
            speechRecognizer = new WindowsRecognizer();
#endif
            tokenSource = new CancellationTokenSource();
        }

        public async void TextToSpeech(string textToSynthesize)
        {
            this.speechSynthesizer.SynthesizeTextAsync(textToSynthesize);
        }

        public async void SpeechToText()
        {
            var isAuthorized = await this.speechRecognizer.RequestPermissions();

            if (isAuthorized)
            {
                IProgress<string> progress = new Progress<string>(text =>
                {
                    // Mise à jour en temps réel du texte reconnu
                    RecognitionText = text;
                    Console.WriteLine($"Recognized: {text}");
                });

                var result = await this.speechRecognizer.Listen(new CultureInfo("fr-FR"), progress, tokenSource.Token);
                RecognitionText = result; // Mise à jour avec le résultat final

                Debug.WriteLine($"Final result: {RecognitionText}");
            }

            else
            {
                throw new Exception("Permission Error Or No microphone access");
            }
        }

        public void ListenCancel()
        {
            tokenSource?.Cancel();
        }
    }
}
