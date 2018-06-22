using MoneyMerchantLocator.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MoneyMerchantLocator.API.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if(auth?.Scheme == "Basic" && auth.Parameter != null)
            {
                try
                {
                    //
                    string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Parameter));
                    string[] parts = decoded.Split(':');

                    string username = parts[0];
                    string password = parts[1];

                    //
                    using (var dbContext = new AppModel())
                    {
                        var user =  dbContext.Users.FirstOrDefault(x => x.Username == username);
                        if(user != null && Helpers.PasswordHelpers.AreEqual(user.PasswordSalt , user.PasswordHash , password))
                        {
                            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                                new Claim(ClaimTypes.Name,user.FirstName  + " " + user.LastName),
                                new Claim(ClaimTypes.Role , "Admin" )
                            }, "Basic"));

                            //  set user
                            request.Properties["CurrentUser"] = user;

                            Thread.CurrentPrincipal = principal;

                            if (HttpContext.Current != null)
                                HttpContext.Current.User = principal;
                        }

                    }

                }
                catch
                {
                    //  Just flow through smoothly
                }

            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}