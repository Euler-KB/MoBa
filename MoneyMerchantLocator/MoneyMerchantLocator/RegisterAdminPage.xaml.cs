using Acr.UserDialogs;
using MoneyMerchantLocator.Helpers;
using MoneyMerchantLocator.Models;
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
    public partial class RegisterAdminPage : ContentPage
    {
        public RegisterAdminPage(AccountType userAccountType, Action complete = null)
        {
            InitializeComponent();
            BindingContext = new RegisterAdminViewModel(userAccountType, complete);
        }

        class RegisterAdminViewModel : ViewModelBase
        {

            private string username;
            private string password;
            private string firstname;
            private string lastname;

            private AccountType userAccountType;
            private Action onComplete;

            public RegisterAdminViewModel(AccountType accountType, Action complete = null)
            {
                userAccountType = accountType;
                onComplete = complete;
            }

            public string Username
            {
                get { return username; }
                set { Set(ref username, value); }
            }

            public string Password
            {
                get { return password; }
                set { Set(ref password, value); }
            }

            public string FirstName
            {
                get { return firstname; }
                set { Set(ref firstname, value); }
            }

            public string LastName
            {
                get { return lastname; }
                set { Set(ref lastname, value); }
            }

            IEnumerable<string> Validate()
            {
                if (!username.IsValidString())
                {
                    yield return "Username required";
                }

                if (!password.IsValidString())
                {
                    yield return "Password required";
                }

                if (password.Length < 3)
                {
                    yield return "Password must be made of at least 3 letters";
                }

                if (!firstname.IsValidString())
                {
                    yield return "First name is required";
                }

                if (!lastname.IsValidString())
                {
                    yield return "Last name is required";
                }

            }

            public ICommand GoBackCommand => new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PopAsync();
            });

            public ICommand RegisterCommand => new Command(async () =>
            {
                var results = Validate();
                if (results.Count() > 0)
                {
                    UserDialogs.Instance.Alert(results.First(), "Invalid Input");
                }
                else
                {
                    using (DialogHelpers.ShowProgress("Registering, please hold on...."))
                    {
                        var response = await Factory.ProxyFactory.GetProxy().ExecuteAsync(Proxy.Endpoints.Users.UsersEndpoints.RegisterUser(new Models.RegisterUserModel()
                        {
                            FirstName = FirstName,
                            LastName = LastName,
                            Password = Password,
                            Username = Username,
                            AccountType = userAccountType
                        }));

                        if (response.Successful)
                        {
                            await App.Current.MainPage.DisplayAlert("Congratulations", "Your account has been registered succesfully!", "OK");
                            GoBackCommand.Execute(null);

                            //
                            onComplete?.Invoke();
                        }
                        else
                        {
                            UserDialogs.Instance.WarnToast(response.FormattedMessage, timeoutMillis: 2000);
                        }
                    }

                }
            });
        }
    }
}
