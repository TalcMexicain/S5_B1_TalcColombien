#if WINDOWS
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Model
{
    /// <summary>
    /// Windows-specific exportation and importation handler for stories.
    /// This class implements the IFileService interface to handle file operations on Windows.
    /// </summary>
    public class WindowsFileService : IFileService
    {
        #region Public Methods 

        /// <summary>
        /// Exports a story to a specified JSON file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the story as.</param>
        /// <param name="fileContent">The content of the story in byte array format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
        public async Task<bool> ExportStoryAsync(string fileName, byte[] fileContent)
        {
            bool success = false;
            
            var hwnd = ((MauiWinUIWindow)Microsoft.Maui.Controls.Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            picker.FileTypeFilter.Add("*");
            InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                var saveFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(saveFile, fileContent);
                success = true;
            }
            
            return success;
        }

        /// <summary>
        /// Imports a story from a JSON file.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the content of the imported story as a byte array.</returns>
        public async Task<byte[]> ImportStoryAsync()
        {
            byte[] content = null;
            
            var hwnd = ((MauiWinUIWindow)Microsoft.Maui.Controls.Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            picker.FileTypeFilter.Add(".json");
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var buffer = await FileIO.ReadBufferAsync(file);
                content = buffer.ToArray();
            }

            return content;
        }

        #endregion
    }
}
#endif