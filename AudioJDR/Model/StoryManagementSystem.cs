using System.Diagnostics;
using System.Globalization;

namespace Model
{
    public class StoryManagementSystem
    {
        private ISpeechSynthesizer speechSynthesizer;
        

        private CancellationTokenSource tokenSource;
        private string recognitionText;

        public string RecognitionText 
        { 
            get => recognitionText; 
            set => recognitionText = value; 
        }

        public StoryManagementSystem(ISpeechSynthesizer speechSynthesizer) 
        {
            this.speechSynthesizer = speechSynthesizer;
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

    }
}
