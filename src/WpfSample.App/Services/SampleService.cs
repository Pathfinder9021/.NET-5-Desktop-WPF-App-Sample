using WpfSample.App.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSample.App.Services
{
    public class SampleService : ISampleService
    {
        public Task<string> GetData()
        {
            return Task.FromResult("sample service");
        }
    }
}
