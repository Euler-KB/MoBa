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
        private bool isModal;

        private Action onComplete;

        public LoginPage(bool modal, Action complete = null)
        {
            //
            isModal = modal;
            onComplete = complete;

            //
            InitializeComponent();

            //
            BindingContext = new LoginPageViewModel(OnComplete);
        }

        private async void OnComplete()
        {
            if (isModal)
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await App.Current.MainPage.Navigation.PopAsync();
            }

            onComplete?.Invoke();
        }

        private void SetupToolbar()
        {
            if (isModal && !ToolbarItems.Any(x => x.Text == "Cancel"))
            {
                ToolbarItems.Add(new ToolbarItem("Cancel", null, OnComplete));
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
        private bool isModal;

        public string Username
        {
            get { return username; }
            set { Set(ref username, value); }
        }

        public bool LockedOut
        {
            get
            {
                if(LoginLockoutEndDate != null)
                {
                    if(DateTime.Now > LoginLockoutEndDate)
                    {
                        LoginLockoutEndDate = null;
                        LoginFailedCount = 0;
                        return false;
                    }

                    return true;
                }

                return false;
            }
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
                //
                UserDialogs.Instance.ShowLoading("Processing login...");

                var proxy = Factory.ProxyFactory.GetProxy();
                var response = await proxy.Authenticate(Username, Password);

                UserDialogs.Instance.HideLoading();

                if (response.Successful)
                {
                    onComplete?.Invoke();
                }
                else
                {
                    
                    if (response.Response != null && LoginFailedCount++ >= 3)
                    {

                        //  set time
                        LoginLockoutEndDate = DateTime.Now.AddMinutes(5);


                        //
                        NotifyPropertyChanged(nameof(LockedOut));

                        // show alert
                        ShowLoginLockoutAlert();


                    }
                    else
                    {
                        UserDialogs.Instance.Alert(response.FormattedMessage, "Login Failed");
                    }
                }
            }

        });

    }
}
