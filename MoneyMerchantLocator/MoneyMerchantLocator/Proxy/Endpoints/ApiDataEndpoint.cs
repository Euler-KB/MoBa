using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy.Endpoints
{
    public class ApiEndpoint<T> : ApiEndpoint
    {
        public ApiEndpoint(string url, HttpMethod method) : base(url, method)
        {

        }

        public ApiEndpoint(string url, HttpMethod method, object jsonContent) : base(url, method, jsonContent)
        {
        }
    }
}
