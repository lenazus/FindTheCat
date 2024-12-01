using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Serilog;
using FindDeCat.Configuration;

namespace FindDeCat
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Fibi.ttf", "Fibi");
                    fonts.AddFont("Fibi-Regular.ttf", "FibiRegular");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //windows: %LocalAppData%\Packages\<PackageName>\LocalState
            //android: /data/data/<PackageName>/files
            var logFilePath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                //.WriteTo.ApplicationInsights("Your_Application_Insights_Instrumentation_Key", TelemetryConverter.Traces) //for Azure
                .CreateLogger();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}


