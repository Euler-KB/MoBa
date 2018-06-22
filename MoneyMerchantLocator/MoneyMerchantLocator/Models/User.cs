using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Models
{
    public enum AccountType
    {
        Administrator,
        User
    }


    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateRegistered { get; set; }
    }

    public class UpdateUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class RegisterUserModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public AccountType AccountType { get; set; }
    }
}
