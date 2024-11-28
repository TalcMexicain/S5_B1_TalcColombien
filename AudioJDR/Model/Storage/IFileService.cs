namespace Model
{
    /// <summary>
    /// Interface for Platform Specific Exportation and Importation Handlers
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Exports a story file into a user selected folder
        /// </summary>
        /// <param name="fileName">the name of the file</param>
        /// <param name="fileContent">the file's content</param>
        /// <returns>a boolean indicating if the export was successful</returns>
        Task<bool> ExportStoryAsync(string fileName, byte[] fileContent);

        /// <summary>
        /// Imports a story from a user selected file
        /// </summary>
        /// <returns>the file data (of a story)</returns>
        Task<byte[]> ImportStoryAsync();
    }
}
