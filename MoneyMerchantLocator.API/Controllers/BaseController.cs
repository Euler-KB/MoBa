using MoneyMerchantLocator.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MoneyMerchantLocator.API.Controllers
{
    public class BaseController : ApiController
    {
        private AppModel dbContext = new AppModel();

        public AppModel DB => dbContext;

        protected User CurrentUser
        {
            get
            {
                return (User)Request.Properties["CurrentUser"];
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                dbContext?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}