using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Models
{
    public class Merchant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Contact { get; set; }

        public string Location { get; set; }

        public string WorkingHours { get; set; }

        public double LocationLat { get; set; }

        public double LocationLng { get; set; }

        public string SupportedNetworks { get; set; }

        public DateTime DateCreated { get; set; }

    }

    public class CreateMerchantModel
    {
        public string Name { get; set; }

        public string Contact { get; set; }

        public string Location { get; set; }

        public string WorkingHours { get; set; }
    

        public double LocationLat { get; set; }

        public double LocationLng { get; set; }

        public string SupportedNetworks { get; set; }
    }

    public class UpdateMerchantModel : CreateMerchantModel
    {
        public new double? LocationLat { get; set; }

        public new double? LocationLng { get; set; }
    }
}
