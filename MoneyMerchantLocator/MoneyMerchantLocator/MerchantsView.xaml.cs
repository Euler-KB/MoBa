using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Store;
using MoneyMerchantLocator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MerchantsView : ContentPage, ITabPage
    {
        private bool isLoaded;

        public MerchantsView()
        {
            InitializeComponent();
            BindingContext = new MerchantsListViewModel();
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
            => ((ListView)sender).SelectedItem = null;

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var merchant = (MerchantViewModel)e.SelectedItem;

            List<string> MerchantActions = new List<string>()
            {
                "Show On Map",
                "Call",
                "View Profile",
                "Get Directions"
            };

            if (UserManager.UserActive)
            {
                MerchantActions.AddRange(new string[]
                {
                    "Remove"
                });
            }

            //  Display action sheet
            var action = await DisplayActionSheet("What would you like to do?", "Cancel", null, MerchantActions.ToArray());

            switch (action)
            {
                case "Show On Map":
                    {
                        MapsPage.Current.ShowMerchant(merchant);
                    }
                    break;
                case "Get Directions":
                    {
                        if (Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationAvailable)
                        {

                            UserDialogs.Instance.ShowLoading("Processing...");
                            try
                            {

                                var location = await Plugin.Geolocator.CrossGeolocator.Current.GetPositionAsync();
                                Device.OpenUri(new Uri($"http://maps.apple.com/?saddr={location.Latitude},{location.Longitude}&daddr={merchant.Model.LocationLat},{merchant.Model.LocationLng}"));
                            }
                            catch
                            {
                                await DisplayAlert("Directions", "Failed getting current location", "Ok");
                            }
                            finally
                            {
                                UserDialogs.Instance.HideLoading();
                            }
                        }
                    }
                    break;
                case "Call":
                    Device.OpenUri(new Uri($"tel:{merchant.Model.Contact}"));
                    break;
                case "View Profile":
                    {
                        var model = merchant.Model;

                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine($"Name: {model.Name}");
                        sb.AppendLine($"Location: {model.Location}");
                        sb.AppendLine($"Contact: {model.Contact}");
                        sb.AppendLine($"Networks: {model.SupportedNetworks}");

                        if(model.WorkingHours.IsValidString())
                            sb.AppendLine($"Working Hours: {model.WorkingHours}");

                        UserDialogs.Instance.Alert(sb.ToString(), "Merchant Profile");
                    }
                    break;
                case "Remove":
                    UserDialogs.Instance.Confirm(new ConfirmConfig()
                    {
                        Title = "Remove Merchant",
                        Message = "Are you sure you want to remove the merchant?",
                        OkText = "Remove",
                        OnConfirm = async (confirmed) =>
                        {
                            if (confirmed)
                            {
                                if (await MerchantStore.Instance.Remove(merchant.Model.Id))
                                {
                                    var context = (BindingContext as MerchantsListViewModel);
                                    context.Items.Remove(context.Items.First(x => x.Model.Id == merchant.Model.Id));

                                    //
                                    MessagingCenter.Send(this, "UpdateMerchants");
                                }
                                else
                                {
                                    UserDialogs.Instance.Alert("Failed removing merchant!", "Failed");
                                }
                            }
                        }
                    });
                    break;
            }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        public Task Load()
        {
            if (!isLoaded)
            {
                UserDialogs.Instance.InfoToast("Getting merchants...", timeoutMillis: 400);

                (BindingContext as MerchantsListViewModel).RefreshDataCommand.Execute(true);
                isLoaded = true;
            }

            return Task.FromResult(0);
        }

        public Task Refresh()
        {
            (BindingContext as MerchantsListViewModel)?.RefreshDataCommand.Execute(null);
            return Task.FromResult(0);
        }
    }

    class MerchantsListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MerchantViewModel> Items { get; private set; }

        public MerchantsListViewModel()
        {
            RefreshDataCommand = new Command(async (args) => await RefreshData( (args is bool && ((bool)args) == true) ));
        }

        public ICommand RefreshDataCommand { get; }

        async Task RefreshData(bool considerLocal = false)
        {
            //
            IsBusy = true;

            List<Models.Merchant> _merchants = null;
            if (considerLocal && MerchantStore.Instance.LocalReady)
            {
                _merchants = MerchantStore.Instance.Local;
            }
            else
            {
                //Load Data Here
                var response = await MerchantStore.Instance.GetAll();
                if (response.Successful)
                {
                    _merchants = response.Data;
                }
            }

            if (_merchants != null)
            {
                Items = new ObservableCollection<ViewModels.MerchantViewModel>(_merchants.Select(x => new ViewModels.MerchantViewModel(x)));
                OnPropertyChanged(nameof(Items));
            }

            IsBusy = false;
        }

        bool busy;
        public bool IsBusy
        {
            get { return busy; }
            set
            {
                busy = value;
                OnPropertyChanged();
                ((Command)RefreshDataCommand).ChangeCanExecute();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
