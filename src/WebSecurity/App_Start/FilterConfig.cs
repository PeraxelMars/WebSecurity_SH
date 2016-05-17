using System.Web.Mvc;

namespace SH_WebSecurity
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler());
            filters.Add(new AuthorizeAttribute()); // Instead of <deny users="?" />
        }
    }
}