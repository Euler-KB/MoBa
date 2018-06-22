using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MoneyMerchantLocator.Droid
{
    [Activity(Label = "Momo Merchant Locator", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static MainActivity()
        {
            void ShowException(Exception ex)
            {
                new Android.Support.V7.App.AlertDialog.Builder(Xamarin.Forms.Forms.Context)
                    .SetTitle("Unhandled Exception")
                    .SetMessage(ex.ToString())
                    .SetPositiveButton("OK", delegate { });
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                ShowException(e.ExceptionObject as Exception);
            };

            AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                ShowException(e.Exception);
                e.Handled = true;
            };

        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, null);
            Acr.UserDialogs.UserDialogs.Init(this);
            LoadApplication(new App());
        }
    }
}

