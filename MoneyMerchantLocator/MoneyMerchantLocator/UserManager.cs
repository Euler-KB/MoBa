using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Models;
using MoneyMerchantLocator.Proxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator
{
    public class UserTicket
    {
        public User User { get; set; }

        public UserCredentials Credentials { get; set; }
    }


    public static class UserManager
    {
        private static UserTicket _currentUser;

        public static bool UserActive => Current != null;

        static UserManager()
        {
            //  Load existing user
            string userinfo = Settings.UserInfo;

            if (userinfo?.Trim().Length > 0)
            {
                _currentUser = JsonConvert.DeserializeObject<UserTicket>(userinfo);
            }

        }

        /// <summary>
        /// Gets or sets the currently loggen in user
        /// </summary>
        public static UserTicket Current
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    Settings.UserInfo = JsonConvert.SerializeObject(_currentUser);
                }
            }
        }

    }
}
