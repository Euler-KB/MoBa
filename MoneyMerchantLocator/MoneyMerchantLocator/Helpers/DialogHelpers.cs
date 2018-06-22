using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMerchantLocator.Helpers
{
    public static class DialogHelpers
    {
        class DisposableAction : IDisposable
        {
            public Action enter, exit;

            public DisposableAction(Action exit)
            {
                this.exit = exit;
            }

            public DisposableAction(Action enter,Action exit)
            {
                this.enter = enter;
                this.exit = exit;

                //
                enter?.Invoke();
            }

            public void Dispose()
            {
                exit?.Invoke();
            }
        }
        
        public static IDisposable ShowProgress(string message = null)
        {
            return new DisposableAction(delegate 
            {
                UserDialogs.Instance.ShowLoading(message);
            },delegate
            {
                UserDialogs.Instance.HideLoading();
            });
        }
    }
}
