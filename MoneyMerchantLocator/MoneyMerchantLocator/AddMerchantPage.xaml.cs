using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Store;
using MoneyMerchantLocator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMerchantPage : ContentPage
    {
        public AddMerchantPage()
        {
            InitializeComponent();
        }

        public AddMerchantPage(Action<Models.Merchant> onAdded = null)
        {
            InitializeComponent();
            BindingContext = new AddMerchantViewModel(onAdded);
        }

        class AddMerchantViewModel : ViewModelBase
        {
            private Action<Models.Merchant> onAdded;

            public AddMerchantViewModel(Action<Models.Merchant> onAdded = null)
            {
                this.onAdded = onAdded;
            }

            public class NetworksViewModel : ViewModelBase
            {
                private bool vodafone = true;
                private bool mtn = true;
                private bool airtel = true;
                private bool tigo = true;

                public bool Vodafone
                {
                    get { return vodafone; }
                    set { Set(ref vodafone, value); }
                }

                public bool MTN
                {
                    get { return mtn; }
                    set { Set(ref mtn, value); }
                }

                public bool Airtel
                {
                    get { return airtel; }
                    set { Set(ref airtel, value); }
                }

                public bool Tigo
                {
                    get { return tigo; }
                    set { Set(ref tigo, value); }
                }

                public override string ToString()
                {
                    List<string> parts = new List<string>();

                    if (mtn)
                        parts.Add("MTN");

                    if (tigo)
                        parts.Add("Tigo");

                    if (airtel)
                        parts.Add("Airtel");

                    if (vodafone)
                        parts.Add("Vodafone");

                    return string.Join(",", parts);
                }
            }

            private string name;
            private string phone;
            private string location;
            private double locationLat = 0;
            private double locationLng = 0;
            private string workingHoursStart;
            private string workingHoursEnd;

            public NetworksViewModel Networks { get; set; } = new NetworksViewModel();

            public string Name
            {
                get { return name; }
                set { Set(ref name, value); }
            }

            public string Phone
            {
                get { return phone; }
                set { Set(ref phone, value); }
            }

            public string WorkingHoursStart
            {
                get{ return workingHoursStart; }
                set{ Set(ref workingHoursStart , value); }
            }

            public string WorkingHoursEnd
            {
                get { return workingHoursEnd; }
                set { Set(ref workingHoursEnd ,value); }
            }

            public string Location
            {
                get { return location; }
                set { Set(ref location, value); }
            }

            public double Lat
            {
                get { return locationLat; }
                set
                {
                    Set(ref locationLat, value);
                }
            }

            public double Lng
            {
                get { return locationLng; }
                set
                {
                    Set(ref locationLng, value);
                }
            }

            private IEnumerable<string> Validate()
            {
                if (!Name.IsValidString())
                {
                    yield return "Merchant name is required!";
                }

                if (double.TryParse(Name, out _))
                {
                    yield return "Merchant name cannot be numeric!";
                }

                if (!Location.IsValidString())
                {
                    yield return "Please enter merchant location name";
                }

                if (!Validators.IsValidPhone(Phone))
                {
                    yield return "Enter a valid phone number";
                }


                if(!Validators.IsValidHour(WorkingHoursStart))
                {
                    yield return "Invalid start working hour!";
                }

                if(!Validators.IsValidHour(WorkingHoursEnd))
                {
                    yield return "Invalid end working hour!";
                }

            }

            public ICommand AddMerchantCommand => new Command(async () =>
            {
                var results = Validate();
                if (results.Count() > 0)
                {
                    UserDialogs.Instance.Alert(results.First(), "Invalid Input");
                }
                else
                {
                    var status = await MerchantStore.Instance.AddMerchant(new Models.CreateMerchantModel()
                    {
                        Contact = Phone,
                        Location = Location,
                        LocationLat = Lat,
                        LocationLng = Lng,
                        Name = Name,
                        WorkingHours = $"{WorkingHoursStart}-{WorkingHoursEnd}",
                        SupportedNetworks = Networks.ToString()
                    });

                    if (status.Successful)
                    {
                        await App.Current.MainPage.Navigation.PopAsync();
                        onAdded?.Invoke(status.Data);
                    }
                    else
                    {
                        UserDialogs.Instance.InfoToast(status.Message, timeoutMillis: 1200);
                    }
                }

            });
        }
    }

}
