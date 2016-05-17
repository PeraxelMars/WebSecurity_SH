using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SH_WebSecurity.RoleContext
{
    public static class ApplicationRole
    {
        public const String Admin = "admin";
        public const String NormalUser = "normal";
    }

    public class Operator : IIdentity
    {
        public Operator(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; private set; }

        public string Name { get; private set; }
        public string AuthenticationType
        {
            get
            {
                return HttpContext.Current.User.Identity.AuthenticationType;
            }
        }
        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

    }


    public class SH_WebSecurityPricipal : IPrincipal
    {
        private Operator _operator;
        private List<string> _roles;

        private SH_WebSecurityPricipal()
        {
        }
        public SH_WebSecurityPricipal(Operator @operator, List<string> roles)
        {
            _roles = new List<string>();
            _operator = @operator;

            foreach (string role in roles)
            {
                _roles.Add(role);
            }
        }
        public IIdentity Identity
        {
            get
            {
                return _operator;
            }
        }

        public bool IsInRole(string roleName)
        {
            return _roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> Roles
        {
            get { return _roles; }
        }
    }
}
