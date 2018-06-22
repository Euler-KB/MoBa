using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

namespace MoneyMerchantLocator.API.Filters
{
    public class EnsureValidModelState : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, actionContext.ModelState);

        }
    }
}