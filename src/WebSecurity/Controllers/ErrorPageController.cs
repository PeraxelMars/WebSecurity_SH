using System.Web.Mvc;

namespace SH_WebSecurity.Controllers
{
    [AllowAnonymous]
    public class ErrorPageController : Controller
    {
        //
        // GET: /ErrorPage/

        public ActionResult AccessDenied(string roles)
        {
            //Response.StatusCode = 403;
            ViewBag.Roles = roles;
            return View();
        }

    }
}
