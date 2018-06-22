using MoneyMerchantLocator.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;

namespace MoneyMerchantLocator.API
{
    public class AppDbInitializer : CreateDatabaseIfNotExists<AppModel>
    {
        class UserInfo
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class MerchantInfo
        {
            public string Name { get; set; }

            public string SupportedNetworks { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }

            public string Phone { get; set; }

            public string Location { get; set; }

            public string WorkingHours { get; set; }
        }

        static T LoadContent<T>(string name)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Seed", name);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static void PopulateContext(AppModel context)
        {
            context.Users.AddOrUpdate(x => x.Username, LoadContent<IEnumerable<UserInfo>>("Users.json").Select(x =>
             {
                 string salt;
                 string pwdHash = Helpers.PasswordHelpers.Generate(x.Password, out salt);
                 return new User()
                 {
                     PasswordSalt = salt,
                     PasswordHash = pwdHash,
                     DateRegistered = DateTime.UtcNow,
                     FirstName = x.FirstName,
                     LastName = x.LastName,
                     Username = x.Username,
                     AccountType = AccountType.Administrator
                 };
             }).ToArray());

            context.Merchants.AddOrUpdate(x => x.Name, LoadContent<IEnumerable<MerchantInfo>>("Merchants.json").Select(x => new Merchant()
            {
                Contact = x.Phone,
                DateCreated = DateTime.UtcNow,
                Location = x.Location,
                LocationLat = x.Lat,
                LocationLng = x.Lng,
                Name = x.Name,
                SupportedNetworks = x.SupportedNetworks,
                WorkingHours = x.WorkingHours
            }).ToArray());

        }

        protected override void Seed(AppModel context)
        {
            PopulateContext(context);
        }
    }
}