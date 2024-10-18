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
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 9999 && resultCode == Result.Ok)
            {
                var uri = data.Data;
                if (uri != null)
                {
                    // Pass the result to the AndroidFileService to handle the folder path
                    var fileService = new AndroidFileService();
                    fileService.OnFolderPicked(uri);
                }
            }
        }
    }
}
