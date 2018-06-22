using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MoneyMerchantLocator.API.Models
{
    public class Merchant
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string Location { get; set; }

        public string WorkingHours { get; set; }

        public double LocationLat { get; set; }

        public double LocationLng { get; set; }

        [Required]
        public string SupportedNetworks { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class CreateMerchantModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        public string Location { get; set; }

        public string WorkingHours { get; set; }

        public double LocationLat { get; set; }

        public double LocationLng { get; set; }

        [Required]
        public string SupportedNetworks { get; set; }
    }

    public class UpdateMerchantModel : CreateMerchantModel
    {
        public new double ? LocationLat { get; set; }

        public new double ? LocationLng { get; set; }
    }
}