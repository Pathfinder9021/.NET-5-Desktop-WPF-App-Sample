using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfSample.App.Services.Interfaces;
using System.Threading.Tasks;

namespace WpfSample.App.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        private readonly ISampleService sampleService;

        private string input;
        public string Input
        {
            get => input;
            set => Set(ref input, value);
        }

        public string WindowTitle { get; set; } = "App sample";

        public RelayCommand ExecuteCommand { get; }

        public MainViewModel(ISampleService sampleService)
        {
            this.sampleService = sampleService;

            ExecuteCommand = new RelayCommand(async () => await ExecuteAsync());
        }

        private async Task ExecuteAsync()
        {
            var data = await sampleService.GetData();
            Input = data;
        }
    }
}
