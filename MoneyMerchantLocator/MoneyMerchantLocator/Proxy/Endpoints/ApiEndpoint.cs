using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy.Endpoints
{
    public class ApiEndpoint : IEndpoint
    {
        private string uri;
        private HttpMethod method;
        private HttpContent content;
        private IDictionary<string, object> queryValues = new Dictionary<string, object>();
        private IDictionary<string, object> headers = new Dictionary<string, object>();
        private bool requireAuth;

        public ApiEndpoint(string url, HttpMethod method)
        {
            this.uri = url;
            this.method = method;
        }

        public ApiEndpoint(string url, HttpMethod method, object jsonContent)
        {
            this.uri = url;
            this.method = method;
            content = new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json");
        }

        public ApiEndpoint WithQuery(string key, object value)
        {
            queryValues[key] = value;
            return this;
        }

        public ApiEndpoint WithHeader(string key, object value)
        {
            headers[key] = value;
            return this;
        }

        public ApiEndpoint WillRequireAuth()
        {
            this.requireAuth = true;
            return this;
        }

        public ApiEndpoint WithStreamContent(Stream stream, string contentType)
        {
            var sc = new StreamContent(stream);
            sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            this.content = sc;
            return this;
        }

        public HttpRequestMessage Request
        {
            get
            {
                List<string> queryPart = new List<string>();
                foreach (var query in queryValues)
                {
                    if (query.Value is IEnumerable)
                    {
                        foreach(var item in query.Value as IEnumerable)
                            queryPart.Add($"{query.Key}={item}");
                    }
                    else
                    {
                        queryPart.Add($"{query.Key}={query.Value}");
                    }
                }

                HttpRequestMessage request = new HttpRequestMessage(method, uri + (queryPart.Count > 0 ? $"?{string.Join("&", queryPart)}" : ""));

                foreach (var hdr in headers)
                    request.Headers.Add(hdr.Key, hdr.Value.ToString());

                if (content != null)
                    request.Content = content;

                return request;
            }
        }

        public bool RequireAuthentication => requireAuth;
    }

}
