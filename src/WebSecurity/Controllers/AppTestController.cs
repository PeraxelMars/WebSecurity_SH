using AptusPortal.Filters;
using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SH_WebSecurity.Controllers
{
    [AllowAnonymous]
    [IsLocalRequest]
    public class AppTestController : Controller
    {

        public ActionResult Index()
        {
            return View(TestApplication());
        }

        private AppTestData TestApplication()
        {
            AppTestData hAppTestData = new AppTestData(User.Identity.Name);

            return hAppTestData;
        }

        public static int DotNet_4_5_Installed()
        {
            using (Microsoft.Win32.RegistryKey ndpKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                try
                {
                    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                    return releaseKey;
                }
                catch (NullReferenceException)
                { return -1; }
            }
        }

        public static string Get45DotVersion(int releaseKey)
        {
            //  https://msdn.microsoft.com/en-us/library/bb822049(v=vs.110).aspx
            if ((releaseKey >= 379893))
            {
                return "Yes (v. 4.5.2 or later).";
            }
            if ((releaseKey >= 378675))
            {
                return "Yes (v. 4.5.1 or later).";
            }
            if ((releaseKey >= 378389))
            {
                return "Yes (v. 4.5 or later).";
            }
            // This line should never execute. A non-null release key should mean 
            // that 4.5 or later is installed. 
            return "No (.Net v. 4.5 was not detected).";
        }
    }
}

namespace SH_WebSecurity
{
    public class AppTestData
    {
        public string LoggedOnUser { get; set; }
        public string AuthenticationMode { get; set; }
        public string CurrentUser { get; set; }
        public string DefaultIssuer { get; set; }
        public string CurrentDomain { get; set; }
        public string Version { get; set; }
        public string DotNet_4_5_installed { get; set; }
        public string AllowAnonymous { get; set; }
        public string AllowEveryone { get; set; }

        public AppTestData(string userIdentity)
        {
            LoggedOnUser = (userIdentity == String.Empty) ? "[Anonymous]" : userIdentity;
            CurrentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            AuthenticationMode = ((AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication")).Mode.ToString();

            AllowAnonymous = "[Default]";
            AllowEveryone = "[Default]";

            Configuration configuration =
                WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

            AuthorizationSection authorizationSection =
                (AuthorizationSection)configuration.GetSection("system.web/authorization");

            AuthorizationRuleCollection authorizationRuleCollection =
                authorizationSection.Rules;

            foreach (AuthorizationRule rule in authorizationRuleCollection)
            {
                try
                {
                    if (rule.Users.ToString() == "?")
                    {
                        AllowAnonymous = rule.Action.ToString();
                    }
                    else if (rule.Users.ToString() == "*")
                    {
                        AllowEveryone = rule.Action.ToString();
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            CurrentDomain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            int net45Version = Controllers.AppTestController.DotNet_4_5_Installed();
            DotNet_4_5_installed = Controllers.AppTestController.Get45DotVersion(net45Version);
        }
    }
}