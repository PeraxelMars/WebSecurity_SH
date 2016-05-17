using SH_WebSecurity.AppCode;
using SH_WebSecurity.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace SH_WebSecurity.RoleContext
{
    public static class RoleContextHandler
    {
        private static readonly Dictionary<string, SH_WebSecurityPricipal> _securityPricipalCache
            = new Dictionary<string, SH_WebSecurityPricipal>();

        public static SH_WebSecurityPricipal GetSecurityPricipal(IPrincipal user, string currentSessionId)
        {
            if (!user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("Not Authenticated");

            SH_WebSecurityPricipal context;

            lock (_securityPricipalCache)
            {
                if (!_securityPricipalCache.TryGetValue(currentSessionId, out context))
                    throw new UnauthorizedAccessException("No role context");
            }

            return context;
        }

        public static void LoginForms(LoginInfo loginInfo, Controller controller)
        {
            SH_WebSecurityPricipal securityPricipal = null;

            if (loginInfo.Username == null)
            {
                throw new AuthenticationFailedException(loginInfo.Username);
            }

            Operator @operator = null;

            // GET OPERATOR FROM DB BY LOGIN NAME. Not implemented here
            //Compare Encrypted Password
            bool passwordsAreTheSame = true;
            int idFromDb = 123;
            string operatorNameFromDb = "OperatorName";

            if (passwordsAreTheSame)
            {
                @operator = new Operator(idFromDb, operatorNameFromDb);
            }

            securityPricipal = Authenticate(@operator);

            //Set the cookie
            FormsAuthentication.SetAuthCookie(loginInfo.Username, false);

            lock (_securityPricipalCache)
            {
                _securityPricipalCache[controller.Session.SessionID] = securityPricipal;
            }
        }

        public static void LoginWindows(WindowsIdentity wi, Controller controller)
        {
            SH_WebSecurityPricipal securityPricipal = null;

            if (wi == null)
            {
                throw new AuthenticationFailedException("No user");
            }

            Operator @operator = null;
            // CHECK IF USER EXISTS IN DB/AD etc.. Not implemented here
            bool userExists = true;

            if (userExists)
            {
                @operator = new Operator(1, "OperatorName");
            }

            securityPricipal = Authenticate(@operator);

            lock (_securityPricipalCache)
            {
                _securityPricipalCache[controller.Session.SessionID] = securityPricipal;
            }

        }

        public static void LogOff(string operatorSessionId)
        {
            _securityPricipalCache.Remove(operatorSessionId);
        }

        private static SH_WebSecurityPricipal Authenticate(Operator @operator)
        {
            if (@operator == null)
                throw new AuthenticationFailedException(@operator.Name);

            //Get operator roles from db or ...
            List<string> operatorRoles = new List<string>();
            operatorRoles.Add(ApplicationRole.Admin);
            //operatorRoles.Add(Some other role);

            // Persist the operator
            SH_WebSecurityPricipal roleContext = new SH_WebSecurityPricipal(@operator, operatorRoles);

            HttpContext.Current.Session["SessionTest"] = DateTime.Now.AddMinutes(GetSessionTimeoutInMinutes());

            return roleContext;
        }

        private static int GetSessionTimeoutInMinutes()
        {
            System.Configuration.Configuration configuration =
                WebConfigurationManager.OpenWebConfiguration("~");
            // Get the external Authentication section.
            SessionStateSection sessionStateSection =
                (SessionStateSection)configuration.GetSection("system.web/sessionState");

            // Get the external Forms section .
            return sessionStateSection.Timeout.Minutes;
        }
    }
}