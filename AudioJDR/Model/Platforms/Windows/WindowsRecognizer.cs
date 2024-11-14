using System.Diagnostics;
using System.Globalization;
using System.Speech.Recognition;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using SpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;

namespace Model
{
    public class WindowsRecognizer : ISpeechRecognizer
    {
        private SpeechRecognitionEngine speechRecognitionEngine;
        private SpeechRecognizer speechRecognizer;
        private string recognitionText;

        public event EventHandler<string> OnSpeechRecognized;

        public Task<string> Listen(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
        {
            Task<string> stringToReturn = null;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                stringToReturn = ListenOnline(culture, recognitionResult, cancellationToken);
            }
            else
            {
                stringToReturn = ListenOffline(culture, recognitionResult, cancellationToken);
            }

            return stringToReturn;
        }


        private async Task<string> ListenOnline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
        {
            recognitionText = string.Empty;
            speechRecognizer = new SpeechRecognizer(new Language(culture.IetfLanguageTag));
            await speechRecognizer.CompileConstraintsAsync();

            var taskResult = new TaskCompletionSource<string>();
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated += (s, e) =>
            {
                recognitionText += e.Result.Text;
                recognitionResult?.Report(e.Result.Text);
            };
            speechRecognizer.ContinuousRecognitionSession.Completed += (s, e) =>
            {
                switch (e.Status)
                {
                    case SpeechRecognitionResultStatus.Success:
                        taskResult.TrySetResult(recognitionText);
                        break;
                    case SpeechRecognitionResultStatus.UserCanceled:
                        taskResult.TrySetCanceled();
                        break;
                    default:
                        taskResult.TrySetException(new Exception(e.Status.ToString()));
                        break;
                }
            };
            await speechRecognizer.ContinuousRecognitionSession.StartAsync();
            await using (cancellationToken.Register(async () =>
            {
                await StopRecording();
                taskResult.TrySetCanceled();
            }))
            {
                return await taskResult.Task;
            }
        }


        private async Task<string> ListenOffline(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken)
        {
            speechRecognitionEngine = new SpeechRecognitionEngine(culture);
            speechRecognitionEngine.LoadGrammarAsync(new DictationGrammar());
            speechRecognitionEngine.SpeechRecognized += (s, e) =>
            {
                recognitionResult?.Report(e.Result.Text);
            };
            speechRecognitionEngine.SetInputToDefaultAudioDevice();
            speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            var taskResult = new TaskCompletionSource<string>();
            await using (cancellationToken.Register(() =>
            {
                StopOfflineRecording();
                taskResult.TrySetCanceled();
            }))
            {
                return await taskResult.Task;
            }
        }

        private void StopOfflineRecording()
        {
            speechRecognitionEngine?.RecognizeAsyncCancel();
        }

        private async Task StopRecording()
        {
            if (speechRecognizer != null)
            {
                await speechRecognizer.StopRecognitionAsync();
            }
        }

        public Task<bool> RequestPermissions()
        {
            return Task.FromResult(true);
        }

        public string GetRecognizedText()
        {
            return recognitionText;
        }

        public async ValueTask DisposeAsync()
        {
            await StopRecording();
            StopOfflineRecording();
            speechRecognitionEngine?.Dispose();
            speechRecognizer?.Dispose();
        }
    }
}
