using WpfSample.App.Services;
using WpfSample.App.Services.Interfaces;
using WpfSample.App.ViewModels;
using WpfSample.App.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WpfSample.App.Extensions;

namespace WpfSample.App.Configuration
{
    public static class Startup
    {
        public static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            builder.Sources.Clear();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }

        public static void ConfigureLogging(ILoggingBuilder builder)
        {

        }

        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddConfigurationSettings(configuration);

            // Register all services
            services.AddScoped<ISampleService, SampleService>();



            // Register all ViewModels.
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SettingsViewModel>();

            // Register all the Windows of the applications.
            services.AddTransient<MainWindow>();
        }

        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
            => ConfigureServices(context.Configuration, services);
    }
}
