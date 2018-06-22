using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Helpers
{
    public static class Extensions
    {
        public static bool IsValidString(this string str)
        {
            return str != null && str.Trim().Length > 0;
        }
    }
}
