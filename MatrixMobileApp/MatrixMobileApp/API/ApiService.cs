using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatrixMobileApp.API
{
    internal class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://20.86.128.95/")
            };
        }

        public HttpClient Client => _client;
    }
}
