using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapsPage : ContentPage, ITabPage
    {
        private bool isLoaded;

        public static MapsPage Current { get; private set; }

        private ICollection<Models.Merchant> _merchants;

        private Geocoder geoCoder = new Geocoder();

        private Pin selectedPin;

        static BitmapDescriptor MapIconDescriptor;

        public MapsPage()
        {
            InitializeComponent();

            Current = this;

            this.Icon = ImageSource.FromResource("MoneyMerchantLocator.Images.MarkerIcon.png") as FileImageSource;

            MessagingCenter.Subscribe<MerchantsView>(this, "UpdateMerchants", async view =>
            {
                await LoadMerchants(false, false);
            });

            //
            //MapView.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(6.4441301, -1.9076104), 20);

            //
            MapIconDescriptor = BitmapDescriptorFactory.FromStream(typeof(MapsPage).GetTypeInfo().Assembly.GetManifestResourceStream("MoneyMerchantLocator.Images.ShopMapIcon.png"));
        }

        public void ShowMerchant(ViewModels.MerchantViewModel merchant)
        {
            if (merchant == null)
                return;

            var model = merchant.Model;
            MapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(model.LocationLat, model.LocationLng), Distance.FromMeters(450)));

            //
            var pin = MapView.Pins.FirstOrDefault(x => ((Models.Merchant)x.Tag).Id == model.Id);
            if (pin != null)
            {
                MapView.SelectedPin = pin;
                selectedPin = pin;
            }

            //  Set current page
            AppMainPage.Current.CurrentPage = this;
        }

        private void SetupMerchantsView(bool affectCurrentRegion = true)
        {
            MapView.Pins.Clear();
            foreach (var merchant in _merchants)
            {
                MapView.Pins.Add(new Pin()
                {
                    Address = merchant.Location,
                    Position = new Position(merchant.LocationLat, merchant.LocationLng),
                    Label = merchant.Name,
                    Type = PinType.Place,
                    Icon = MapIconDescriptor,
                    Tag = merchant,
                });
            }

            //
            if (affectCurrentRegion)
                MapView.MoveToRegion(MapSpan.FromPositions(_merchants.Select(x => new Position(x.LocationLat, x.LocationLng))));
        }

        public async Task<bool> LoadMerchants(bool indicator = true, bool affectCurrentRegion = true)
        {
            if (indicator)
                UserDialogs.Instance.ShowLoading("Fetching merchants...");

            var response = await MerchantStore.Instance.GetAll();

            if (indicator)
                UserDialogs.Instance.HideLoading();

            if (response.Successful)
            {
                _merchants = response.Data;
                SetupMerchantsView(affectCurrentRegion);
            }
            else
            {
                UserDialogs.Instance.Toast(new ToastConfig(ToastEvent.Warn, response.Message));
            }

            return response.Successful;
        }

        public async Task Load()
        {
            if (!isLoaded)
            {
                if (await LoadMerchants())
                {
                    isLoaded = true;
                }

            }
        }

        public Task Refresh()
        {
            return LoadMerchants();
        }

        private async void MapView_PinClicked(object sender, PinClickedEventArgs e)
        {
            if(selectedPin == e.Pin)
            {
                switch (await DisplayActionSheet("Select Action", "Cancel", null, new string[] { "Get Directions" }))
                {
                    case "Get Directions":
                        {
                            var merchant = e.Pin.Tag as Models.Merchant;
                            using (DialogHelpers.ShowProgress())
                            {
                                switch (Device.RuntimePlatform)
                                {
                                    case Device.Android:
                                        Device.OpenUri(new Uri($"google.navigation:q={merchant.LocationLat},{merchant.LocationLng}"));
                                        break;
                                    case Device.iOS:
                                        {
                                            if( Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationAvailable )
                                            {

                                                UserDialogs.Instance.ShowLoading("Processing...");

                                                try
                                                {

                                                    var location = await Plugin.Geolocator.CrossGeolocator.Current.GetPositionAsync();
                                                    Device.OpenUri(new Uri($"http://maps.apple.com/?saddr={location.Latitude},{location.Longitude}&daddr={merchant.LocationLat},{merchant.LocationLng}"));
                                                }
                                                catch
                                                {
                                                   await DisplayAlert("Directions", "Failed getting current location","Ok");
                                                }
                                                finally{
                                                    UserDialogs.Instance.HideLoading();
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }

            selectedPin = e.Pin;

        }

    }
}
