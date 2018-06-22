using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Helpers
{
    public static class Validators
    {
        static string[] PhonePrefixes = new string[]
        {
            "020",
            "024",
            "023",
            "026",
            "050",
            "054",
            "055"
        };

        public static bool IsValidPhone(string phone)
        {
            return phone.IsValidString() && PhonePrefixes.Any(x => phone.StartsWith(x)) && phone.Length == 10;
        }

        public static bool IsValidHour(string hour)
        {
            if (string.IsNullOrEmpty(hour))
                return false;
            
            if(hour.EndsWith("am",StringComparison.OrdinalIgnoreCase) || hour.EndsWith("pm", StringComparison.OrdinalIgnoreCase))
            {

                int index = hour.IndexOf("am",StringComparison.OrdinalIgnoreCase);
                if(index < 0)
                {
                    index = hour.IndexOf("pm", StringComparison.OrdinalIgnoreCase);
                }

                string n = hour.Substring(0, index).Trim();
                return int.TryParse(n, out int val);

            }

            return false;
        }
    }
}
