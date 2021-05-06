using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WpfSample.App.Extensions
{
    public static class SettingsServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration config) 
        {
            services.Configure<GeneralOptions>(config.GetSection(GeneralOptions.SectionName));
            services.Configure<SystemIntegrationOptions>(config.GetSection(SystemIntegrationOptions.SectionName));
            services.Configure<NotificationOptions>(config.GetSection(NotificationOptions.SectionName));
            services.Configure<PerformanceOptions>(config.GetSection(PerformanceOptions.SectionName));

            return services;
        }
    }
}
