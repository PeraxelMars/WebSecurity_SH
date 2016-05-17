using SH_WebSecurity.RoleContext;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;

namespace SH_WebSecurity.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MyAuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var roles = Roles.Split(',');

            SH_WebSecurityPricipal securityPricipal = null;

            try
            {
                securityPricipal =
                    RoleContextHandler.GetSecurityPricipal(filterContext.HttpContext.User, filterContext.HttpContext.Session.SessionID);
            }
            catch (UnauthorizedAccessException)
            {
                if (filterContext.HttpContext.User is WindowsPrincipal)
                {
                    string url = new UrlHelper(filterContext.Controller.ControllerContext.RequestContext).Action("LoginWindows", "Account");
                    filterContext.Result = new RedirectResult(url);
                    return;
                }
            }

            if (securityPricipal != null && roles.Any(r => securityPricipal.IsInRole(r)))
            {
                return;
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                filterContext.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new { controller = "ErrorPage", action = "AccessDenied", roles = String.Join(", ", roles) }));
                return;
                //base.OnAuthorization(filterContext); // Should this be called at all?
            }
        }
    }
}