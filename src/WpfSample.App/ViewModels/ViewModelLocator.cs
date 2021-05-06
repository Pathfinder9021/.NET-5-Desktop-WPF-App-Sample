using Microsoft.Extensions.DependencyInjection;

namespace WpfSample.App.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();

        public SettingsViewModel SettingsViewModel => App.ServiceProvider.GetRequiredService<SettingsViewModel>();
    }
}
