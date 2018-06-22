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
    public partial class ChangePasswordPage : ContentPage
    {
        class ChangePasswordViewModel : ViewModels.ViewModelBase
        {
            private string originalPassword;
            private string currentPassword;
            private string confirmPassword;

            public string OriginalPassword
            {
                get { return originalPassword; }
                set { Set(ref originalPassword, value); }
            }

            public string NewPassword
            {
                get { return currentPassword; }
                set { Set(ref currentPassword, value); }
            }

            public string ConfirmPassword
            {
                get { return confirmPassword; }
                set { Set(ref confirmPassword, value); }
            }

            private IEnumerable<string> Validate()
            {
                if (!OriginalPassword.IsValidString())
                {
                    yield return "Please enter your original password";
                }

                if (!NewPassword.IsValidString())
                {
                    yield return "Please enter your new password";
                }

                if (NewPassword?.Length < 3)
                {
                    yield return "New password must be made up of at least 3 letters";
                }

                if (NewPassword != ConfirmPassword)
                {
                    yield return "Passwords do not match!";
                }
            }

            public ICommand ChangeCommand => new Command(async () =>
            {
                var results = Validate();
                if(results.Count() > 0)
                {
                    UserDialogs.Instance.Alert(results.First(),"Input Validation");
                }
                else
                {

                    using (DialogHelpers.ShowProgress())
                    {
                        var proxy = Factory.ProxyFactory.GetProxy();
                        var response = await proxy.ExecuteAsync(Proxy.Endpoints.Users.UsersEndpoints.ChangePassword(OriginalPassword, NewPassword));
                        if (response.Successful)
                        {
                            UserDialogs.Instance.Alert("Password changed successfully!");
                            await App.Current.MainPage.Navigation.PopModalAsync();
                        }
                        else
                        {
                            string message = "";
                            switch (response.StatusCode)
                            {
                                case System.Net.HttpStatusCode.Forbidden:
                                    message = "Invalid password. Please enter the correct original password!";
                                    break;
                                default:
                                    message = response.FormattedMessage;
                                    break;
                            }

                            UserDialogs.Instance.InfoToast(message);
                        }

                    }
                }


            });
        }

        public ChangePasswordPage()
        {
            InitializeComponent();
            BindingContext = new ChangePasswordViewModel();
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

    }
}
