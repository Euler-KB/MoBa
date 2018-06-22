using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MoneyMerchantLocator.Models;
using MoneyMerchantLocator.Proxy.Endpoints;

namespace MoneyMerchantLocator.Proxy
{
    public class UserCredentials
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class WebAPIClient
    {
        private UserCredentials userCredentials;
        private string serverAddress;
        private HttpClient httpClient = new HttpClient();

        public async Task<ApiResponse> Authenticate(string username, string password)
        {
            var response = await ExecuteAsync(Endpoints.Users.UsersEndpoints.Login(username, password));
            if (response.Successful)
            {
                userCredentials = new UserCredentials()
                {
                    Username = username,
                    Password = password
                };

                UserManager.Current = new UserTicket()
                {
                    Credentials = userCredentials,
                    User = await response.GetDataAsync()
                };

                //
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            }


            return response;
        }

        public bool IsAuthenticated => userCredentials != null;

        public Task SignOut()
        {
            userCredentials = null;
            UserManager.Current = null;
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            return Task.FromResult(true);
        }

        public WebAPIClient() : this(AppResources.SERVER_ADDRESS)
        {

        }

        public WebAPIClient(string serverAddress)
        {
            this.serverAddress = serverAddress;
            this.httpClient.BaseAddress = new Uri(serverAddress);
            httpClient.Timeout = TimeSpan.FromSeconds(12);

            if (UserManager.UserActive)
            {
                userCredentials = UserManager.Current.Credentials;
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userCredentials.Username}:{userCredentials.Password}")));
            }

        }

        protected async Task<T> InternalExecuteAsync<T>(IEndpoint endpoint, Func<HttpRequestException, T> exceptionHandler, Func<HttpResponseMessage, Task<T>> responeHandler)
        {
            if (endpoint.RequireAuthentication && !IsAuthenticated)
                return exceptionHandler(new HttpRequestException("Authentication required for endpoint!"));

            try
            {

                var request = endpoint.Request;
                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                return await responeHandler(response);

            }
            catch (HttpRequestException ex)
            {
                return exceptionHandler(ex);
            }
        }

        public Task<ApiResponse> ExecuteAsync(ApiEndpoint endpoint)
        {
            return InternalExecuteAsync(endpoint, (ex) => new ApiResponse(ex), (response) => Task.FromResult(new ApiResponse(response)));
        }

        public Task<ApiResponse<T>> ExecuteAsync<T>(ApiEndpoint<T> endpoint)
        {
            return InternalExecuteAsync(endpoint, (ex) => new ApiResponse<T>(ex), async (response) =>
            {
                var result = new ApiResponse<T>(response);
                if (result.Successful)
                {
                    await result.GetDataAsync();
                }

                return result;
            });
        }

        public async Task<T> ExecuteDataAsync<T>(ApiEndpoint<T> endpoint)
        {
            var response = await ExecuteAsync(endpoint);
            if (response.Successful)
            {
                return await response.GetDataAsync();
            }

            throw new InvalidOperationException("Operation was not successfull");
        }

    }
}
