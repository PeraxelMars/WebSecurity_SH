using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SH_WebSecurity
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        void Application_BeginRequest(Object source, EventArgs e) { }

        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Get this version
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Application["theTitle"] = "WebSecurity " + version.Substring(0, version.LastIndexOf('.'));
        }

        protected void Session_Start(object sender, EventArgs e) { }

        protected void Session_End(object sender, EventArgs e) { }
    }
}