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
        public async Task ExportStoryAsync(string fileName, byte[] fileContent)
        {
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
            }
        }

        public async Task<byte[]> ImportStoryAsync()
        {
            var hwnd = ((MauiWinUIWindow)Microsoft.Maui.Controls.Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".json"); // Restrict to .json files
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var content = await FileIO.ReadBufferAsync(file);
                return content.ToArray();
            }

            return null;
        }
    }
}
#endif