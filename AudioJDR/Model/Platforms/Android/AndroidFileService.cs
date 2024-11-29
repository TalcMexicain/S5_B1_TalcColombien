#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.DocumentFile.Provider;

namespace Model
{
    /// <summary>
    /// Android-specific exportation and importation handler for stories.
    /// This class implements the IFileService interface to handle file operations on Android.
    /// </summary>
    public class AndroidFileService : IFileService
    {
        private const int FolderPickerRequestCode = 9999;
        private TaskCompletionSource<Android.Net.Uri> _folderPathCompletionSource;

        private static AndroidFileService _instance;

        private AndroidFileService() { }

        /// <summary>
        /// Gets the singleton instance of the AndroidFileService.
        /// </summary>
        public static AndroidFileService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AndroidFileService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Exports a story to a specified JSON file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the story as.</param>
        /// <param name="fileContent">The content of the story in byte array format.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success or failure.</returns>
        public async Task<bool> ExportStoryAsync(string fileName, byte[] fileContent)
        {
            bool success = false;
            _folderPathCompletionSource = new TaskCompletionSource<Android.Net.Uri>();

            var intent = new Intent(Intent.ActionOpenDocumentTree);
            ((Activity)Platform.CurrentActivity).StartActivityForResult(intent, FolderPickerRequestCode);

            var folderUri = await _folderPathCompletionSource.Task;

            if (folderUri != null)
            {
                success = WriteStoryFileToUri(folderUri, fileName, fileContent);
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

            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a story file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } }
                })
            });

            if (file != null)
            {
                content = File.ReadAllBytes(file.FullPath);
            }

            return content;
        }

        private bool WriteStoryFileToUri(Android.Net.Uri folderUri, string fileName, byte[] fileContent)
        {
            bool success = false;

            DocumentFile pickedDir = DocumentFile.FromTreeUri(Platform.CurrentActivity, folderUri);

            if (pickedDir != null)
            {
                DocumentFile newFile = pickedDir.CreateFile("application/json", fileName);

                using (var outputStream = Platform.CurrentActivity.ContentResolver.OpenOutputStream(newFile.Uri))
                {
                    if (outputStream != null)
                    {
                        outputStream.Write(fileContent, 0, fileContent.Length);
                        outputStream.Flush();
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Handles the folder selection result from the folder picker.
        /// </summary>
        /// <param name="uri">The URI of the selected folder.</param>
        public void OnFolderPicked(Android.Net.Uri uri)
        {
            if (_folderPathCompletionSource != null)
            {
                if (uri != null)
                {
                    _folderPathCompletionSource.SetResult(uri);
                }
                else
                {
                    _folderPathCompletionSource.SetResult(null);
                }
            }
        }
    }
}
#endif