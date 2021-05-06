using GalaSoft.MvvmLight;
using WpfSample.App.Models;

namespace WpfSample.App.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        public SettingsViewModel()
        {

        }

        public SettingsModel Settings { get; set; }
    }
}
