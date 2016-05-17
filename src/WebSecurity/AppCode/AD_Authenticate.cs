using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace SH_WebSecurity
{
    public class AD_Authenticate
    {
        internal static readonly string AD_SEARCH_PATH_BASE = "LDAP://{0}/{1}";
        internal static readonly string AD_SEARCH_FILTER_BASE = "(cn={0})";
        internal static string AD_DOMAIN_NAME = "EREBUS_DMN";
        internal static string AD_ADMIN_GROUP = "APP_FOT_ROL_Admin";

        public static void IsAuthenticated(IPrincipal user)
        {
            if (user == null || string.IsNullOrEmpty(user.Identity.Name))
                throw new HttpException((int)HttpStatusCode.BadRequest, "Messages.USER_NOT_FOUND");

            var userInfo = user.Identity.Name.Split("\\".ToCharArray());
            if (AD_DOMAIN_NAME != userInfo[0])
                throw new HttpException((int)HttpStatusCode.BadRequest, "Messages.USER_NOT_AUTHORIZED");

            try
            {
                string softhouseAD = "SOFTHOUSEFS";
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, softhouseAD/*userInfo[0]*/))
                {

                    // find a user
                    UserPrincipal adUser = UserPrincipal.FindByIdentity(principalContext, userInfo[1]);
                    var path = string.Format(AD_SEARCH_PATH_BASE, userInfo[0], adUser.DistinguishedName);

                    string memberOf = "memberOf";
                    DirectorySearcher search = new DirectorySearcher(path);
                    search.Filter = string.Format(AD_SEARCH_FILTER_BASE, adUser.DisplayName);
                    search.PropertiesToLoad.Add(memberOf);

                    bool existsInGroup = false;
                    try
                    {
                        SearchResult result = search.FindOne();
                        int propertyCount = result.Properties[memberOf].Count;
                        string dn;
                        for (int i = 0; i < propertyCount; i++)
                        {
                            dn = (string)result.Properties[memberOf][i];

                            string groupName = GetGroupName(dn);
                            if (AD_ADMIN_GROUP == groupName)
                            {
                                existsInGroup = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        throw new HttpException((int)HttpStatusCode.InternalServerError, "Messages.GENERAL_ERROR");
                    }

                    if (!existsInGroup)
                    {
                        throw new HttpException((int)HttpStatusCode.BadRequest, "Messages.USER_NOT_AUTHORIZED");
                    }
                }
            }
            catch (PrincipalServerDownException)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "No contact with AD-server");
            }
        }

        private static string GetGroupName(string dn)
        {
            int equalsIndex = dn.IndexOf("=", 1);
            int commaIndex = dn.IndexOf(",", 1);
            return dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);
        }
    }
}