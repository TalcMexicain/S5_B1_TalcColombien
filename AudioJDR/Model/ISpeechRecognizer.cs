using System.Globalization;

namespace Model
{
    /// <summary>
    /// Interface for platform specific speech recognition
    /// This interface defines the necessary methods and events for recognizing speech input
    /// </summary>
    public interface ISpeechRecognizer
    {
        /// <summary>
        /// Requests the necessary permissions to use the microphone for speech recognition
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the permissions were granted</returns>
        Task<bool> RequestPermissions();

        /// <summary>
        /// Starts listening for speech input asynchronously
        /// </summary>
        /// <param name="culture">The culture information for recognizing speech (e.g., language and region)</param>
        /// <param name="recognitionResult">A progress object to report intermediate recognition results</param>
        /// <param name="cancellationToken">A cancellation token to stop the recognition process if needed</param>
        /// <returns>A task that represents the asynchronous operation, containing the final recognized text</returns>
        Task<string> Listen(CultureInfo culture, IProgress<string> recognitionResult, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the recognized text from the speech input
        /// This method returns the text that has been recognized from the speech input
        /// </summary>
        /// <returns>The recognized text as a string</returns>
        string GetRecognizedText();

        /// <summary>
        /// Event triggered when speech is recognized
        /// </summary>
        event EventHandler<string> OnSpeechRecognized;
    }
}
