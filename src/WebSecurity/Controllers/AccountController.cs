using SH_WebSecurity.AppCode;
using SH_WebSecurity.Filters;
using SH_WebSecurity.Models;
using SH_WebSecurity.RoleContext;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SH_WebSecurity.Controllers
{
    [NoCache]
    public class AccountController : Controller
    {
        /// <summary>
        /// When using windows auth. use the following returnUrl format
        /// if U want to get redirekted to a Customer by name
        /// http://server/Account/Login?returnUrl=/Customer/Details?name=customerName
        /// or by id
        /// http://server/Account/Login?returnUrl=/Customer/Details/id
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="autoLoginOk"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginInfo model, string returnUrl)
        {
            // Reset - just for show
            HomeController.orgIssueDate = null;

            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                RoleContextHandler.LoginForms(model, this);

                return RedirectToLocal(returnUrl);
            }
            catch (AuthenticationFailedException e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        }

        public ActionResult LoginWindows()
        {
            WindowsIdentity wi = (WindowsIdentity)User.Identity;
            //// WindowsIdentity wi = Request.LogonUserIdentity;

            if (wi != null)
            {
                try
                {
                    //AD_Authenticate.IsAuthenticated(User);
                    RoleContextHandler.LoginWindows(wi, this);
                    return Redirect("~");
                }
                catch (HttpException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, ex.Message);
                }
            }

            return View("Login");
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            //Remove from persistent Dictionary
            RoleContextHandler.LogOff(Session.SessionID);
            //Remove cookie
            FormsAuthentication.SignOut();
            // Kill the session - and possibly clean up
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) && !returnUrl.ToLower().Contains("edit") && !returnUrl.ToLower().Contains("create"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
