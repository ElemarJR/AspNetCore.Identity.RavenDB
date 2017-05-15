using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.RavenDB
{
    public class RavenDBIdentityUser
    {
        private readonly List<UserLoginInfo> _logins;

        public RavenDBIdentityUser(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            _logins = new List<UserLoginInfo>();
        }

        public string Id { get; internal set; }
        public string UserName { get; internal set; }
        public string NormalizedUserName { get; internal set; }
        public string PasswordHash { get; internal set; }

        internal IEnumerable<UserLoginInfo> Logins
        {
            get => _logins;
            set
            {
                if (value != null) _logins.AddRange(value);
            }
        }
        internal void AddLogin(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (_logins.Any(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey))
            {
                throw new InvalidOperationException("There is a login with the same provider already exists.");
            }
            
            _logins.Add(login);
        }

        internal void RemoveLogin(string loginProvider, string providerKey)
        {
            var loginToRemove = _logins.FirstOrDefault(l => 
                l.LoginProvider == loginProvider && 
                l.ProviderKey == providerKey
            );

            if (loginToRemove == null) return;

            _logins.Remove(loginToRemove);
        }
    }
}
