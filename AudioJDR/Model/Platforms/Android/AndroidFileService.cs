#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.DocumentFile.Provider;

namespace Model
{
    public class AndroidFileService : IFileService
    {
        private const int FolderPickerRequestCode = 9999;
        private TaskCompletionSource<Android.Net.Uri> _folderPathCompletionSource;

        // Singleton instance
        private static AndroidFileService _instance;

        // Private constructor to prevent external instantiation
        private AndroidFileService() { }

        // Public method to access the singleton instance
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

        public async Task<bool> ExportStoryAsync(string fileName, byte[] fileContent)
        {
            bool success = false;
            
            _folderPathCompletionSource = new TaskCompletionSource<Android.Net.Uri>();
            Console.WriteLine($"Exportation of story with fileName = {fileName} initiated");

            var intent = new Intent(Intent.ActionOpenDocumentTree);
            ((Activity)Platform.CurrentActivity).StartActivityForResult(intent, FolderPickerRequestCode);

            var folderUri = await _folderPathCompletionSource.Task;

            if (folderUri != null)
            {
                WriteStoryFileToUri(folderUri, fileName, fileContent);
                Console.WriteLine($"Story with fileName = {fileName} exported successfully");
                success = true;
            }
            else
            {
                Console.WriteLine("No folder selected, story export aborted");
            }
            
            return success;
        }

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

        // Call this method from MainActivity.cs in OnActivityResult
        public void OnFolderPicked(Android.Net.Uri uri)
        {
            if (_folderPathCompletionSource != null)
            {
                if (uri != null)
                {
                    Console.WriteLine("Folder URI received, setting result...");
                    _folderPathCompletionSource.SetResult(uri); // Return the folder Uri
                }
                else
                {
                    Console.WriteLine("No folder picked, setting result as null.");
                    _folderPathCompletionSource.SetResult(null); // Handle the case where no folder is picked
                }
            }
            else
            {
                // This should not happen if the flow is correct
                Console.WriteLine("TaskCompletionSource was not initialized.");
            }
        }
    }
}
#endif