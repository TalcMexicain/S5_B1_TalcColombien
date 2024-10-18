#if ANDROID
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.OS;
using AndroidX.DocumentFile.Provider;
using System.IO;

namespace Model
{
    public class AndroidFileService : IFileService
    {
        private const int FolderPickerRequestCode = 9999;

        public async Task ExportStoryAsync(string fileName, byte[] fileContent)
        {
            // Launch an Intent to select a folder
            var intent = new Intent(Intent.ActionOpenDocumentTree);
            ((Activity)Platform.CurrentActivity).StartActivityForResult(intent, FolderPickerRequestCode);

            // Handle result in OnActivityResult (in MainActivity.cs)
            var saveFolder = await AwaitFolderSelection();

            if (!string.IsNullOrEmpty(saveFolder))
            {
                var filePath = Path.Combine(saveFolder, fileName);
                File.WriteAllBytes(filePath, fileContent);
            }
        }

        public async Task<byte[]> ImportStoryAsync()
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a story file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/json" } } // Restrict to .json files
            })
            });

            if (file != null)
            {
                return File.ReadAllBytes(file.FullPath); // Return byte array to be deserialized
            }

            return null;
        }

        // Await the folder selection result
        private TaskCompletionSource<string> _folderPathCompletionSource;

        private Task<string> AwaitFolderSelection()
        {
            _folderPathCompletionSource = new TaskCompletionSource<string>();
            return _folderPathCompletionSource.Task;
        }

        // Call this method in MainActivity.cs in OnActivityResult
        public void OnFolderPicked(Android.Net.Uri uri)
        {
            var path = GetPathFromUri(uri);
            _folderPathCompletionSource.SetResult(path);
        }

        private string GetPathFromUri(Android.Net.Uri uri)
        {
            DocumentFile documentFile = DocumentFile.FromTreeUri(Platform.CurrentActivity, uri);
            return documentFile.Uri.Path;
        }
    }
}
#endif