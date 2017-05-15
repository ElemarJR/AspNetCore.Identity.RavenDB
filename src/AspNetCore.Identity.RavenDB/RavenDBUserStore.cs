using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Raven.Client;

namespace AspNetCore.Identity.RavenDB
{

    public class RavenDBUserStore<TUser> :
        IUserStore<TUser>,
        IUserLoginStore<TUser> 
        where TUser : RavenDBIdentityUser
    {
        private readonly Func<IAsyncDocumentSession> _getAsyncSessionFunc;

        public RavenDBUserStore(Func<IAsyncDocumentSession> getAsyncSessionFunc)
        {
            _getAsyncSessionFunc = 
                getAsyncSessionFunc ?? 
                throw new ArgumentNullException(nameof(getAsyncSessionFunc));
        }

        #region IUserStore
        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var session = _getAsyncSessionFunc())
            {
                await session.StoreAsync(user, cancellationToken = default(CancellationToken));
                await session.SaveChangesAsync(cancellationToken = default(CancellationToken));
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var session = _getAsyncSessionFunc())
            {
                await session.StoreAsync(user, cancellationToken = default(CancellationToken));
                await session.SaveChangesAsync(cancellationToken = default(CancellationToken));

                return IdentityResult.Success;
            }
        }

        public async Task<IdentityResult> DeleteAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            using (var session = _getAsyncSessionFunc())
            {
                session.Delete(user.Id);
                await session.SaveChangesAsync(cancellationToken = default(CancellationToken));
                return IdentityResult.Success;
            }
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var session = _getAsyncSessionFunc())
            {
                return session.LoadAsync<TUser>(userId, cancellationToken = default(CancellationToken));
            }
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var session = _getAsyncSessionFunc())
            {
                return session.Query<TUser>().FirstOrDefaultAsync(
                    u => u.NormalizedUserName == normalizedUserName, cancellationToken
                );
            }
        }
        #endregion

        #region IUserLoginStore
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            user.AddLogin(login);

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            user.RemoveLogin(loginProvider, providerKey);

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<UserLoginInfo>>(user.Logins.ToList());
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            using (var session = _getAsyncSessionFunc())
            {
                var query =
                    from user in session.Query<TUser>()
                    where user.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey)
                    select user;

                return query.FirstOrDefaultAsync(cancellationToken);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion

        
    }

    public class RavenDBUserStore : RavenDBUserStore<RavenDBIdentityUser>
    {
        public RavenDBUserStore(Func<IAsyncDocumentSession> getAsyncSessionFunc)
            : base(getAsyncSessionFunc)
        {
        }
    }
}
