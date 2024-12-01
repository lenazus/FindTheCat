using FindDeCat.Services;
using Microsoft.Extensions.DependencyInjection;
using MediaManager;
using FindDeCat.Pages;
using FindDeCat.Services.Sound;
using FindDeCat.Configuration;
using Serilog;
using FindDeCat.Models;
using static FindDeCat.Configuration.AppConfiguration;
using FindDeCat.Helpers;

namespace FindDeCat
{
    public partial class App : Application
    {
        public static IServiceProvider? ServicesProvider { get; private set; }

        public App()
        {
            try
            {
                InitializeComponent();

                ConfigureServices();

                if (ServicesProvider == null)
                {
                    throw new InvalidOperationException(AppConfiguration.Messages.SERVICE_PROVIDER_NOT_INITIALIZED);
                }

                MainPage = new NavigationPage(new GamePage(
                    ServicesProvider.GetRequiredService<IPickerHandler>(),
                    ServicesProvider.GetRequiredService<GameState>(), 
                    ServicesProvider.GetRequiredService<IGameUIService>(),
                    ServicesProvider.GetRequiredService<IAutoClickService>()
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{AppConfiguration.Messages.GENERAL_ERROR_MESSAGE}, {ex.Message}");

                MainPage = new ErrorPage(AppConfiguration.Messages.GENERAL_ERROR_MESSAGE);
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                //to test how will be displayed in mobile, when running in windows
                window.Width = WindowConstants.TYPICAL_MOBILE_WIDTH;
                window.Height = WindowConstants.TYPICAL_MOBILE_HEIGHT;
            }

            return window;
        }

        protected override void OnStart()
        {
            // Called when the application starts.
        }

        //not working
        protected override void OnSleep()
        {
            // Pause auto-click when the app goes into the background
            var autoClickService = ServicesProvider.GetRequiredService<AutoClickService>();
            autoClickService.PauseAutoClick();
            //Log.Information("App -> OnSleep is called");
        }

        /// <summary>
        /// Configures the services for Dependency Injection
        /// </summary>
        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            CrossMediaManager.Current.Init();

            // Register services for Dependency Injection
            services.AddSingleton<GameState>(); // Register GameState
            services.AddSingleton<IGameUIService, GameUIService>();
            services.AddSingleton<IPickerHandler, PickerHandler>();
            services.AddSingleton<IAutoClickService, AutoClickService>();
            services.AddSingleton<ISoundPlayerService, SoundPlayerService>();
            services.AddSingleton<ITranslations, Translations>();
            services.AddSingleton<IMediaManager>(provider => CrossMediaManager.Current);

            ServicesProvider = services.BuildServiceProvider();
        }
    }
}