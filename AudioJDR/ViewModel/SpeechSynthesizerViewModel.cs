using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class SpeechSynthesizerViewModel : BaseViewModel
    {
        private readonly ISpeechSynthesizer _speechSynthesizer;
        private string _textToSynthesize;

        public event PropertyChangedEventHandler PropertyChanged;

        public SpeechSynthesizerViewModel(ISpeechSynthesizer speechSynthesizer)
        {
            _speechSynthesizer = speechSynthesizer;
        }

        // Text to synthesize
        public string TextToSynthesize
        {
            get => _textToSynthesize;
            set => SetProperty(ref _textToSynthesize, value);
        }

        public void SynthesizeText()
        {
            if (!string.IsNullOrEmpty(TextToSynthesize))
            {
                _speechSynthesizer.SynthesizeTextAsync(TextToSynthesize);
            }
        }

        public void StopSynthesis()
        {
            _speechSynthesizer.StopSynthesisTextAsync();
        }

    }
}
