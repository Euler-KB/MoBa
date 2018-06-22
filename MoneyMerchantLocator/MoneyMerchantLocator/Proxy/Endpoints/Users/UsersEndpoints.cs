using MoneyMerchantLocator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Proxy.Endpoints.Users
{
    public static class UsersEndpoints
    {
        public static readonly string BaseEndpoint = "api/users";

        public static ApiEndpoint<User> Login(string username, string password) => new ApiEndpoint<User>($"{BaseEndpoint}/login", HttpMethod.Post,new
        {
            Username = username,
            Password = password
        });

        public static ApiEndpoint<User> CurrentUser => new ApiEndpoint<User>($"{BaseEndpoint}/myself", HttpMethod.Get);

        public static ApiEndpoint<User> RegisterUser(RegisterUserModel model) => new ApiEndpoint<User>($"{BaseEndpoint}/register", HttpMethod.Post, model);

        public static ApiEndpoint ChangePassword(string oldpassword, string newPassword) => new ApiEndpoint($"{BaseEndpoint}/change/password", HttpMethod.Post, new
        {
            OldPassword = oldpassword,
            NewPassword = newPassword
        });


        public static ApiEndpoint<User> Update(int id, UpdateUserModel model) => new ApiEndpoint<User>($"{BaseEndpoint}/{id}", HttpMethod.Put, model);

    }
}
