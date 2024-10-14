using Microsoft.UI.Xaml;
using System.Runtime.InteropServices; // For P/Invoke

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace View.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        // P/Invoke to use Windows API to maximize window
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_MAXIMIZE = 3; // Command to maximize the window


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            // Get the current MAUI window
            var mauiWindow = Microsoft.Maui.Controls.Application.Current.Windows.FirstOrDefault();
            if (mauiWindow != null)
            {
                // Get the native window handle
                IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(mauiWindow.Handler.PlatformView);

                // Maximize the window using native Windows API
                ShowWindow(hWnd, SW_MAXIMIZE); // Maximize the window
            }
        }
    }
}
