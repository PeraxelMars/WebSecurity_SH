using SH_WebSecurity.Filters;
using SH_WebSecurity.RoleContext;
using System;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace SH_WebSecurity.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        public static DateTime? orgIssueDate;

        [MyAuthorizeFilter(Roles = "Admin")]
        public ActionResult Index()
        {
            SetViewTestData();
            return View();
        }

        [MyAuthorizeFilter(Roles = "Admin")]
        public ActionResult Access()
        {
            ViewBag.Roles = ""; // String.Join(", ", RoleContextHandler.GetSecurityPricipal(User, Session.SessionID).Roles);
            return View();
        }

        [MyAuthorizeFilter(Roles = "NotRightUser")]
        public ActionResult NoAccess()
        {
            return View();
        }

        private void SetViewTestData()
        {
            if (User.Identity is FormsIdentity)
            {

                orgIssueDate = orgIssueDate ?? ((FormsIdentity)User.Identity).Ticket.IssueDate;

                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;

                ViewBag.OrgIssuDate = orgIssueDate;

                DateTime issueDate = ticket.IssueDate;
                ViewBag.IssueDate = issueDate;

                DateTime expiration = ticket.Expiration;
                ViewBag.Expiration = expiration;

                // Get the Web application configuration.
                System.Configuration.Configuration configuration =
                    WebConfigurationManager.OpenWebConfiguration("~");

                // Get the external Authentication section.
                AuthenticationSection authenticationSection =
                    (AuthenticationSection)configuration.GetSection("system.web/authentication");

                // Get the external Forms section .
                FormsAuthenticationConfiguration formsAuthentication = authenticationSection.Forms;

                bool currentSlidingExpiration = formsAuthentication.SlidingExpiration;
                ViewBag.SlidingExpiration = currentSlidingExpiration;
            }
            string ip = Request.UserHostAddress;
            ViewBag.UserHostAddress = ip;
        }
    }
}
