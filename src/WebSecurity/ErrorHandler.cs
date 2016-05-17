using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SH_WebSecurity
{
    public class ErrorHandler : HandleErrorAttribute 
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if (filterContext.Exception is UnauthorizedAccessException)
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}