using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMainPage : TabbedPage
    {
        public static AppMainPage Current { get; private set; }

        public AppMainPage()
        {
            InitializeComponent();

            // 
            Current = this;

            //  Setup pages
            SetupPages();
        }


        public void SwitchPage(int index)
        {
            CurrentPage = this.Children[index];
        }

        protected override void OnAppearing()
        {
            //
            SetupToolbar();

            base.OnAppearing();
        }

        private void SetupPages()
        {
            CurrentPageChanged += (s, e) =>
            {
                (CurrentPage as ITabPage)?.Load();
            };

            (CurrentPage as ITabPage)?.Load();
        }

        private void SetupToolbar()
        {
            ToolbarItems.Clear();

            if (!UserManager.UserActive)
            {
                ToolbarItems.Add(new ToolbarItem("Login", null, async () =>
                {
                    await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new LoginPage(true)));

                }, ToolbarItemOrder.Secondary));

            }
            else
            {
                ToolbarItems.Add(new ToolbarItem("Add Vendor", null, async () =>
                {
                    await App.Current.MainPage.Navigation.PushAsync(new AddMerchantPage(merchant =>
                   {
                       Children.OfType<MapsPage>().FirstOrDefault()?.LoadMerchants(false, true);
                       Children.OfType<MerchantsView>().FirstOrDefault()?.Refresh();
                   }));

                }, ToolbarItemOrder.Primary));

                ToolbarItems.Add(new ToolbarItem("My Profile", null, async () =>
                {
                    await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new UserProfilePage()));

                }, ToolbarItemOrder.Secondary));
            }

            ToolbarItems.Add(new ToolbarItem("Refresh", null, () =>
            {
                (CurrentPage as ITabPage)?.Refresh();

            }, ToolbarItemOrder.Secondary));


            ToolbarItems.Add(new ToolbarItem("About", null, async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new AboutPage());

            }, ToolbarItemOrder.Secondary));
        }
    }
}
