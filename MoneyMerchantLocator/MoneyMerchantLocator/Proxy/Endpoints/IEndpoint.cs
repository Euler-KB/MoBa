using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy.Endpoints
{
    public interface IEndpoint
    {
        bool RequireAuthentication { get; }

        HttpRequestMessage Request { get; }
    }
}
