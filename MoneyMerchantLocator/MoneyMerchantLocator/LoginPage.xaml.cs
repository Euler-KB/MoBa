using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
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
    public partial class LoginPage : ContentPage
    {
        public bool IsModal { get; }

        public LoginPage(bool modal, Action complete = null)
        {
            IsModal = modal;

            InitializeComponent();

            BindingContext = new LoginPageViewModel(async () =>
            {
                if (modal)
                {
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }

                complete?.Invoke();
            });

        }

        private void SetupToolbar()
        {
            ToolbarItems.Clear();

            if (IsModal)
            {
                ToolbarItems.Add(new ToolbarItem("Cancel", null, () =>
                  {
                      (BindingContext as LoginPageViewModel)?.GoBackCommand.Execute(null);
                  }));
            }
        }

        protected override void OnAppearing()
        {
            SetupToolbar();
            base.OnAppearing();
        }
    }

    class LoginPageViewModel : ViewModels.ViewModelBase
    {
        static DateTime? LoginLockoutEndDate = null;

        static int LoginFailedCount = 0;

        private Action onComplete;
        private string username;
        private string password;

        public string Username
        {
            get { return username; }
            set { Set(ref username, value); }
        }

        public bool LockedOut
        {
            get { return LoginLockoutEndDate != null && DateTime.Now > LoginLockoutEndDate; }
        }

        public string Password
        {
            get { return password; }
            set { Set(ref password, value); }
        }

        public LoginPageViewModel(Action complete)
        {
            this.onComplete = complete;
        }

        IEnumerable<string> Validate()
        {

            if (!username.IsValidString())
                yield return "Please enter your username";

            if (!password.IsValidString())
                yield return "Please enter your password";

        }

        private void ShowLoginLockoutAlert()
        {
            UserDialogs.Instance.Alert("Login has been disabled temporarily. Please try again in a few minutes", "Locked Out");
        }

        public ICommand LoginCommand => new Command(async () =>
        {
            if (LockedOut)
            {
                ShowLoginLockoutAlert();
                return;
            }

            var results = Validate();
            if (results.Count() > 0)
            {
                UserDialogs.Instance.InfoToast(results.First(), timeoutMillis: 1000);
            }
            else
            {
                UserDialogs.Instance.ShowLoading("Processing login...");

                var proxy = Factory.ProxyFactory.GetProxy();
                var response = await proxy.Authenticate(Username, Password);

                UserDialogs.Instance.HideLoading();

                if (response.Successful)
                {
                    GoBackCommand.Execute(null);
                    onComplete?.Invoke();
                }
                else
                {
                    if (LoginFailedCount++ >= 3)
                    {
                        ShowLoginLockoutAlert();
                        LoginLockoutEndDate = DateTime.Now.AddMinutes(5);
                        LoginFailedCount = 0;
                    }
                    else
                    {
                        UserDialogs.Instance.Alert(response.FormattedMessage, "Login Failed");
                    }
                }
            }

        });

        public ICommand GoBackCommand => new Command(() =>
        {
            App.Current.MainPage.Navigation.PopModalAsync();
        });
    }
}
