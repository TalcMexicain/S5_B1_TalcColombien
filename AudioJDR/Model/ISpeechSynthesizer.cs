using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Interface for platform speech synthesizer (text-to-speech)
    /// </summary>
    public interface ISpeechSynthesizer
    {
        /// <summary>
        /// Synthesizes and speaks the provided text
        /// </summary>
        /// <param name="textToSynthesize">The text to synthesize and speak</param>
        void SynthesizeTextAsync(string textToSynthesize);

        /// <summary>
        /// Stops any ongoing speech synthesis asynchronously
        /// </summary>
        void StopSynthesisTextAsync();

        /// <summary>
        /// Pauses the currently running speech synthesis
        /// </summary>
        void PauseCurrentSynthesis();

        /// <summary>
        /// Resumes a speech synthesis that was previously paused
        /// </summary>
        void ResumePausedSynthesis();

        /// <summary>
        /// Sets the volume of the voice used for speech synthesis
        /// </summary>
        /// <param name="voiceVolume">The volume of the voice (between 0 and 100)</param>
        void SetVoiceVolume(int voiceVolume);

        /// <summary>
        /// Gets the current volume of the voice used for speech synthesis
        /// </summary>
        /// <returns>The current volume value (between 0 and 100)</returns>
        int GetVoiceVolume();

        /// <summary>
        /// Sets the type of voice to use for speech synthesis
        /// This method selects a specific voice (e.g., male, female, or a specific accent)
        /// </summary>
        /// <param name="voiceName">The name of the voice to use</param>
        void SetVoiceType(string voiceName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetInstalledVoices();

        /// <summary>
        /// Sets the rate at which the voice speaks
        /// Adjust the speed of the speech output
        /// </summary>
        /// <param name="voiceRate">The rate of the voice (a float between 0.5f and 2.0f)</param>
        void SetVoiceRate(float voiceRate);

        /// <summary>
        /// Releases any resources used by the speech synthesizer
        /// This method should be called when the object is no longer needed to free memory and other resources
        /// </summary>
        void Dispose();
    }
}
