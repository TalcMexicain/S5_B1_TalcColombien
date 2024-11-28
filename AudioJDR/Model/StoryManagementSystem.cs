#if WINDOWS
using Model.Platforms.Windows;
#elif ANDROID
using Model.Platforms.Android;
using Android.Content;

#endif
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
            this.speechSynthesizer = new WindowsSynthesizer();
#elif ANDROID
            Context context = Android.App.Application.Context;
            this.speechSynthesizer = new AndroidSynthesizer(context);
#endif
            tokenSource = new CancellationTokenSource();
        }

        public async void TextToSpeech(string textToSynthesize)
        {
            this.speechSynthesizer.SynthesizeTextAsync(textToSynthesize);
        }

        public void StopTextToSpeech()
        {
            this.speechSynthesizer.StopSynthesisTextAsync();
        }

        public async void SpeechToText()
        {
            var isAuthorized = await this.speechRecognizer.RequestPermissions();

            if (isAuthorized)
            {
                IProgress<string> progress = new Progress<string>(text =>
                {
                    RecognitionText = text;
                    Console.WriteLine($"Recognized: {text}");
                });

                var result = await this.speechRecognizer.Listen(new CultureInfo("fr-FR"), progress, tokenSource.Token);
                RecognitionText = result;

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
