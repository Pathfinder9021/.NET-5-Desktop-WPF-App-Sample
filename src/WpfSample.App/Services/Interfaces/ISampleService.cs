using System.Threading.Tasks;

namespace WpfSample.App.Services.Interfaces
{
    public interface ISampleService
    {
        Task<string> GetData();
    }
}
