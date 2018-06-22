using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator
{
    public interface ITabPage
    {
        /// <summary>
        /// Load tabs content once
        /// </summary>
        Task Load();

        /// <summary>
        /// Refresh tab view
        /// </summary>
        Task Refresh();
    }
}
