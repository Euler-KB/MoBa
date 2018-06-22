using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyMerchantLocator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : ContentPage, INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;

        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public UserProfilePage()
        {
            firstName = UserManager.Current.User.FirstName;
            lastName = UserManager.Current.User.LastName;

            InitializeComponent();
        }

        private void OnChangePassword(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new ChangePasswordPage()));
        }

        private IEnumerable<string> Validate()
        {
            if (FirstName?.Trim().Length == 0)
            {
                yield return "First name cannot be empty!";
            }

            if (LastName?.Trim().Length == 0)
            {
                yield return "Last name cannot be empty!";
            }
        }

        private async void OnGoBack(object sender, EventArgs e)
        {
            var user = UserManager.Current.User;
            if (FirstName != user.FirstName || LastName != user.LastName)
            {
                if (await UserDialogs.Instance.ConfirmAsync("Do you want to save the changes made?", "Save Changes"))
                {
                    var results = Validate();
                    if (results.Count() > 0)
                    {
                        UserDialogs.Instance.InfoToast(results.First());
                        return;
                    }
                    else
                    {
                        var response = await Factory.ProxyFactory.GetProxy().ExecuteAsync(Proxy.Endpoints.Users.UsersEndpoints.Update(user.Id, new Models.UpdateUserModel()
                        {
                            FirstName = FirstName,
                            LastName = LastName
                        }));

                        if (response.Successful)
                        {
                            UserManager.Current = new UserTicket()
                            {
                                Credentials = UserManager.Current.Credentials,
                                User = await response.GetDataAsync()
                            };

                        }
                        else
                        {
                            UserDialogs.Instance.InfoToast(response.FormattedMessage);
                            return;
                        }

                    }
                }

            }

            await App.Current.MainPage.Navigation.PopModalAsync();

        }

        private async void OnSignOut(object sender, EventArgs e)
        {
            if (await UserDialogs.Instance.ConfirmAsync("Are you sure you want to sign out?", "Sign Out", "Signout", "Cancel"))
            {
                //
                await Factory.ProxyFactory.GetProxy().SignOut();

                //
                UserDialogs.Instance.Alert("You have been signed out successfully!", "Signed Out");

                await App.Current.MainPage.Navigation.PopModalAsync();

            }
        }
    }
}
