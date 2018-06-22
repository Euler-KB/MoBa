using MoneyMerchantLocator.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Store
{
    public class LoadState<T>
    {
        private ApiResponse<T> response;

        public T Data => response.Data;

        public bool Successful => response.Successful;

        public string Message => response.FormattedMessage;

        public LoadState()
        {

        }

        public LoadState(ApiResponse<T> response)
        {
            this.response = response;
        }
    }
}
