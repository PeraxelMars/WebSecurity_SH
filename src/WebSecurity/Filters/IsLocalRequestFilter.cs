using System.Web.Mvc;

namespace AptusPortal.Filters
{
    public class IsLocalRequestAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var isLocal = filterContext.HttpContext.Request.IsLocal;
            if (!isLocal)
                filterContext.Result =
                    new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
        }
    }
}
