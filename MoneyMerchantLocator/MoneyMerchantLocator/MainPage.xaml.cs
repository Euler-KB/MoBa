using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MoneyMerchantLocator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //
            SetupInterface();

            base.OnAppearing();
        }

        private void SetupInterface()
        {
            if (UserManager.UserActive)
            {
                btnLogin.IsEnabled = false;
                //
                btnLogin.Style = (Style)App.Current.Resources["DisabledButtonStyle"];
                btnSignup.Style = (Style)App.Current.Resources["DisabledButtonStyle"];

                lbSignOut.IsVisible = true;
                lbWelcomeHeader.Text = $"Welcome, {UserManager.Current.User.FirstName} {UserManager.Current.User.LastName}";
            }
            else
            {
                btnLogin.IsEnabled = true;
                btnLogin.Style = (Style)App.Current.Resources["ActionButtonStyle"];
                btnSignup.Style = (Style)App.Current.Resources["ActionButtonStyle"];

                lbSignOut.IsVisible = false;
                lbWelcomeHeader.Text = "Welcome";
            }
        }

        private void OnSignUp(object sender, EventArgs e)
        {
            if (!UserManager.UserActive)
            {
                App.Current.MainPage.Navigation.PushAsync(new RegisterAdminPage(Models.AccountType.User, () =>
                {
                    App.Current.MainPage.Navigation.PushAsync(new AppMainPage());
                }));
            }
        }

        private void OnLogin(object sender, EventArgs e)
        {
            if (!UserManager.UserActive)
            {
                App.Current.MainPage.Navigation.PushAsync(new LoginPage(false, () =>
                {
                    App.Current.MainPage.Navigation.PushAsync(new AppMainPage());
                }));
            }
        }

        private void OnViewMerchants(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(new AppMainPage());
        }

        private void OnSignOut(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(new ConfirmConfig()
            {
                Message = "Are you sure you want to logout current user?",
                Title = "Sign Out",
                OkText = "Ok",
                OnConfirm = async (confirm) =>
                {
                    if (!confirm)
                        return;

                    //
                    await Factory.ProxyFactory.GetProxy().SignOut();

                    //  setup user interface
                    SetupInterface();

                    //
                    UserDialogs.Instance.Alert("Current user signout out successfully!", "Signed Out");


                }
            });
        }
    }


}
