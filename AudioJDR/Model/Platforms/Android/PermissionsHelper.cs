using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Platforms.Android
{
    public static class PermissionsHelper
    {
        public static async Task<bool> CheckAndRequestMicrophonePermissionAsync()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Microphone permission denied.");
                await Application.Current.MainPage.DisplayAlert(
                    "Permission Denied",
                    "The app requires microphone access to perform speech recognition. Please enable it in the settings.", 
                    "OK");

                OpenAppSettings();
            }

            return status == PermissionStatus.Granted;
        }

        private static void OpenAppSettings()
        {
            try
            {
                AppInfo.ShowSettingsUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to open app settings: " + ex.Message);
            }
        }
    }
}
