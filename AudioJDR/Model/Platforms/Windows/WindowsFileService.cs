#if WINDOWS
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Model
{
    /// <summary>
    /// Windows specific exportation and importation handler
    /// </summary>
    public class WindowsFileService : IFileService
    {
        public async Task<bool> ExportStoryAsync(string fileName, byte[] fileContent)
        {
            bool success = false;
            
            var hwnd = ((MauiWinUIWindow)Microsoft.Maui.Controls.Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
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

        public async Task<byte[]> ImportStoryAsync()
        {
            byte[] content = null;
            
            var hwnd = ((MauiWinUIWindow)Microsoft.Maui.Controls.Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
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
    }
}
#endif