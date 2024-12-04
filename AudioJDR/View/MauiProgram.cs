using Microsoft.Extensions.Logging;
using Model;
using View.Pages;

#if ANDROID
    using Android.Content;
    using Model.Platforms.Android;
#elif WINDOWS 
    using Model.Platforms.Windows;
#endif

namespace View
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("UncialAntiqua-Regular.ttf", "UncialAntiquaRegular");
                });
                builder.Services.AddSingleton<SettingsPage>();
                builder.Services.AddSingleton<App>();
                builder.Services.AddSingleton<MainPlayerPage>();
                builder.Services.AddSingleton<PlayPage>();
                builder.Services.AddSingleton<MainPage>();
                builder.Services.AddSingleton<YourStories>();
#if WINDOWS
                builder.Services.AddSingleton<ISpeechSynthesizer, WindowsSynthesizer>();
                builder.Services.AddSingleton<ISpeechRecognition, WindowsRecognition>();
#elif ANDROID
                //builder.Services.AddSingleton<ISpeechSynthesizer, AndroidSynthesizer()>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
