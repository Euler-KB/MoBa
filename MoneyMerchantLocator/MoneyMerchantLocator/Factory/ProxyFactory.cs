using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Factory
{
    public class ProxyFactory
    {
        static Proxy.WebAPIClient _proxyInstance;

        /// <summary>
        /// Returns a singleton proxy instance for making web api calls
        /// </summary>
        /// <returns></returns>
        public static Proxy.WebAPIClient GetProxy()
        {
            if(_proxyInstance == null)
            {
                _proxyInstance = new Proxy.WebAPIClient();
            }

            return _proxyInstance;
        }
    }
}
