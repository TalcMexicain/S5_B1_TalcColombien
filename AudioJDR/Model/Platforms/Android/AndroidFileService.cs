#if ANDROID
using Android.App;
using Android.Content;
using Android.Provider;
using AndroidX.DocumentFile.Provider;
using System.IO;
using System.Text.Json;

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

        public async Task ExportStoryAsync(string fileName, byte[] fileContent)
        {
            // Initialize the TaskCompletionSource before starting the folder picker intent
            _folderPathCompletionSource = new TaskCompletionSource<Android.Net.Uri>();

            // Log the start of the export
            Console.WriteLine($"Exportation of story with fileName = {fileName} initiated");

            // Launch an Intent to select a folder
            var intent = new Intent(Intent.ActionOpenDocumentTree);
            ((Activity)Platform.CurrentActivity).StartActivityForResult(intent, FolderPickerRequestCode);

            // Await the result of folder selection
            var folderUri = await _folderPathCompletionSource.Task;

            if (folderUri != null)
            {
                // Write the story file to the selected folder
                WriteStoryFileToUri(folderUri, fileName, fileContent);
                Console.WriteLine($"Story with fileName = {fileName} exported successfully");
            }
            else
            {
                Console.WriteLine("No folder selected, story export aborted");
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

        // This method writes the story file into the selected folder using ContentResolver
        private void WriteStoryFileToUri(Android.Net.Uri folderUri, string fileName, byte[] fileContent)
        {
            // Create a DocumentFile object for the selected folder
            DocumentFile pickedDir = DocumentFile.FromTreeUri(Platform.CurrentActivity, folderUri);

            if (pickedDir != null)
            {
                // Create a new file in the selected directory
                DocumentFile newFile = pickedDir.CreateFile("application/json", fileName); // JSON file

                // Open the OutputStream to write to the file
                using (var outputStream = Platform.CurrentActivity.ContentResolver.OpenOutputStream(newFile.Uri))
                {
                    if (outputStream != null)
                    {
                        outputStream.Write(fileContent, 0, fileContent.Length); // Write the actual byte array
                        outputStream.Flush(); // Ensure all data is written
                    }
                }
            }
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