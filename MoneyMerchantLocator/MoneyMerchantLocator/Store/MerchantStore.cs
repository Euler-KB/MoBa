using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Store
{

    public class MerchantStore
    {
        public static MerchantStore Instance { get; } = new MerchantStore();

        private List<Models.Merchant> _localStore;

        public bool LocalReady
        {
            get { return _localStore != null; }
        }

        /// <summary>
        /// Returns the local version of the store
        /// </summary>
        public List<Models.Merchant> Local
        {
            get
            {
                return _localStore;
            }
        }

        private async Task EnsureLoadedAsync()
        {
            if (_localStore == null)
            {
                await GetAll();
            }

        }

        public async Task<LoadState<List<Models.Merchant>>> GetAll()
        {
            var proxy = Factory.ProxyFactory.GetProxy();
            var response = await proxy.ExecuteAsync(Proxy.Endpoints.Merchants.MerchantsEndpoints.GetAll());

            if (response.Successful)
            {
                _localStore = await response.GetDataAsync();
            }

            return new LoadState<List<Models.Merchant>>(response);
        }

        public async Task<LoadState<Models.Merchant>> Get(int id)
        {
            var proxy = Factory.ProxyFactory.GetProxy();
            return new LoadState<Models.Merchant>(await proxy.ExecuteAsync(Proxy.Endpoints.Merchants.MerchantsEndpoints.Get(id)));
        }

        public async Task<LoadState<Models.Merchant>> AddMerchant(Models.CreateMerchantModel merchant)
        {
            var proxy = Factory.ProxyFactory.GetProxy();
            var response = await proxy.ExecuteAsync(Proxy.Endpoints.Merchants.MerchantsEndpoints.Create(merchant));
            if (response.Successful)
            {
                Local.Add(response.Data);
            }

            return new LoadState<Models.Merchant>(response);
        }

        public async Task<bool> Remove(int id)
        {
            var proxy = Factory.ProxyFactory.GetProxy();
            var response = await proxy.ExecuteAsync(Proxy.Endpoints.Merchants.MerchantsEndpoints.Delete(id));
            if (response.Successful)
            {
                var itemIndex = Local.FindIndex(t => t.Id == id);
                if (itemIndex >= 0)
                    Local.RemoveAt(itemIndex);

            }

            return response.Successful;
        }

        public async Task<bool> Update(int id, Models.UpdateMerchantModel merchant)
        {
            var proxy = Factory.ProxyFactory.GetProxy();
            var response = await proxy.ExecuteAsync(Proxy.Endpoints.Merchants.MerchantsEndpoints.Update(id, merchant));
            if (response.Successful)
            {
                var item = Local.Find(x => x.Id == id);
                if(item != null)
                {

                }
            }

            return response.Successful;
        }

    }
}
