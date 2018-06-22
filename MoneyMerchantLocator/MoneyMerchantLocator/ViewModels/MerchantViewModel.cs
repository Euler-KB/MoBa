using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.ViewModels
{
    public class MerchantViewModel : ViewModelBase
    {
        Models.Merchant merchant;
        private string name;
        private string contact;
        private string networks;
        private string location;

        public Models.Merchant Model => merchant;

        public MerchantViewModel() { }

        public MerchantViewModel(Models.Merchant merchant)
        {
            this.merchant = merchant;

            //
            name = merchant.Name;
            contact = merchant.Contact;
            networks = merchant.SupportedNetworks;
            location = merchant.Location;
        }

        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }


        public string Contact
        {
            get { return contact; }
            set { Set(ref contact, value); }
        }

        public string Networks
        {
            get { return networks; }
            set { Set(ref networks, value); }
        }

        public string Location
        {
            get { return location; }
            set { Set(ref location, value); }
        }

    }
}
