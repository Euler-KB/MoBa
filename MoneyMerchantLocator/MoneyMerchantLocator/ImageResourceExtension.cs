using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ImageSource.FromResource(Value);
        }
    }
}
