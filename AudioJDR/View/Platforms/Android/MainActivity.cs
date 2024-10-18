using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Model;

namespace View
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private static AndroidFileService _androidFileService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _androidFileService = AndroidFileService.Instance;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 9999 && resultCode == Result.Ok && data != null)
            {
                var uri = data.Data;
                if (uri != null)
                {
                    Console.WriteLine("Folder picked successfully.");
                    AndroidFileService.Instance.OnFolderPicked(uri); // Use singleton instance
                }
            }
            else
            {
                Console.WriteLine("No folder picked.");
                AndroidFileService.Instance.OnFolderPicked(null); // Handle no folder selected
            }
        }

    }
}
