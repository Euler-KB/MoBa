using MoneyMerchantLocator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy.Endpoints.Merchants
{
    public static class MerchantsEndpoints
    {
        public static readonly string BaseUrl = "api/merchants";

        public static ApiEndpoint<List<Merchant>> GetAll() => new ApiEndpoint<List<Merchant>>(BaseUrl, HttpMethod.Get);

        public static ApiEndpoint<Merchant> Get(int id) => new ApiEndpoint<Merchant>($"{BaseUrl}/{id}", HttpMethod.Get);

        public static ApiEndpoint Delete(int id) => new ApiEndpoint($"{BaseUrl}/{id}", HttpMethod.Delete);

        public static ApiEndpoint<Merchant> Create(CreateMerchantModel merchant) => new ApiEndpoint<Merchant>(BaseUrl, HttpMethod.Post, merchant);

        public static ApiEndpoint Update(int id, UpdateMerchantModel update) => new ApiEndpoint($"{BaseUrl}/{id}", HttpMethod.Put, update);
    }
}
