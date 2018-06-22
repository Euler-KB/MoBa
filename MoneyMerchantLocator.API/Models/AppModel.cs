using System;
using System.Data.Entity;
using System.Linq;

namespace MoneyMerchantLocator.API.Models
{
    public class AppModel : DbContext
    {
        public AppModel()
            : base("name=AppModel")
        {

        }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<User> Users { get; set; }

    }

}