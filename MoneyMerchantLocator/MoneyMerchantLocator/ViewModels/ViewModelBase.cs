using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void Set<T>(ref T prop, T value, [CallerMemberName] string property = "")
        {
            if (!Equals(prop, value))
            {
                prop = value;
                NotifyPropertyChanged(property);
            }
        }

    }
}
