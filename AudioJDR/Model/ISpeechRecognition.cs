
namespace Model
{
    /// <summary>
    /// Interface for speech recognition (adapter pattern)
    /// </summary>
    public interface ISpeechRecognition
    {
        /// <summary>
        /// Event triggered when a text is recognized.
        /// </summary>
        event Action<string> SpeechRecognized;

        /// <summary>
        /// Starts the speech recognition process.
        /// </summary>
        void StartRecognition();

        /// <summary>
        /// Stops the speech recognition process
        /// </summary>
        void StopRecognition();

        /// <summary>
        /// Updates the grammar of the speech recognition engine with the provided keywords
        /// </summary>
        /// <param name="keywords">An array of keywords to include in the grammar</param>
        void UpdateGrammar(string[] keywords);


        /// <summary>
        /// Removes all grammars added to the speech recognition engine.
        /// </summary>
        void UnloadAllGrammars();
    }
}