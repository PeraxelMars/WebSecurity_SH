using System;

namespace SH_WebSecurity.AppCode
{
    /// <summary>
    /// Exception for signaling an authentication failure.
    /// </summary>
    public sealed class AuthenticationFailedException : Exception
    {
        private readonly string _username;

        public AuthenticationFailedException(string username)
            : base(string.Format(
                "Inloggning misslyckades",
                username))
        {
            _username = username;
        }

        public string Username
        {
            get { return _username; }
        }
    }
}
