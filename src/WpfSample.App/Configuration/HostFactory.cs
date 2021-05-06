using Microsoft.Extensions.Hosting;

namespace WpfSample.App.Configuration
{
    internal static class HostFactory
    {
        public static IHost Create()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(Startup.ConfigureAppConfiguration)
                .ConfigureServices(Startup.ConfigureServices)
                .ConfigureLogging(Startup.ConfigureLogging);

            return hostBuilder.Build();
        }
    }
}
